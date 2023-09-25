using System;
using System.IO;
using System.IO.Ports;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Dalamud.Logging;

namespace PlaydateFishing;

public class PlaydateSerial : IDisposable {
    private const int BaudRate = 115200;
    private const int BaseReconnectionTime = 5 * 1000;
    private const int MaxReconnectionTime = 60 * 1000;
    private static Regex LineRegex = new Regex("\\[playdate-fishing\\]\\[(\\w+)\\] (.*)");

    private SerialPort port;
    private CancellationTokenSource ct;

    // Shitty exponential backoff impl
    private int reconnectionRetries = 0;
    private int reconnectionPow = 1;

    public PlaydateSerial(string line) {
        this.ct = new();

        this.port = new();
        this.port.PortName = line;
        this.port.BaudRate = BaudRate;
    }

    public void Connect() {
        if (this.ct.IsCancellationRequested) return;

        try {
            this.port.Open();

            // Write a newline in case someone typed an invalid command and forgot to send it
            this.port.Write("\n");
            Task.Run(this.Read, this.ct.Token);

            Plugin.LogToChat("Connected to your Playdate!");
            this.reconnectionRetries = 0;
            this.reconnectionPow = 1;
        } catch (Exception e) {
            PluginLog.Error(e, "Error connecting to Playdate");
            Plugin.LogToChat("Failed to connect to your Playdate. Is it plugged in and unlocked?", error: true);
            this.AttemptReconnection();
        }
    }

    private void AttemptReconnection() {
        this.reconnectionRetries++;
        if (this.reconnectionRetries < 30 + 1) {
            this.reconnectionPow <<= 1;
        }

        var backoff = BaseReconnectionTime * (this.reconnectionPow) / 2;
        backoff = Math.Min(backoff, MaxReconnectionTime);

        var seconds = (int) Math.Ceiling(backoff / 1000f);
        Plugin.LogToChat($"Attempting to reconnect in {seconds} seconds...");

        Task.Run(() => {
            Task.Delay(backoff).Wait();
            this.Connect();
        }, this.ct.Token);
    }

    private void Read() {
        while (!this.ct.IsCancellationRequested && this.port.IsOpen) {
            try {
                var line = this.port.ReadLine();
                //PluginLog.Debug(line);

                // [playdate-fishing][type] message
                var match = LineRegex.Match(line);
                if (match.Success) {
                    var type = match.Groups[1].Value;
                    var content = match.Groups[2].Value;
                    Plugin.PlaydateState.HandleMessage(type, content);
                }
            } catch (Exception e) {
                PluginLog.Error(e, "Error handling serial line");
            }
        }

        // if we broke out of the loop but cancellation is not requested, we got disconnected by the playdate
        if (!this.ct.IsCancellationRequested) this.AttemptReconnection();
    }

    public void Evaluate(string hex) {
        var payload = Convert.FromHexString(hex);
        var command = $"eval {payload.Length}\n";

        using var ms = new MemoryStream();
        using var br = new BinaryWriter(ms);

        // Can't use C# string writing because it's Different:tm:
        br.Write(Encoding.UTF8.GetBytes(command));
        br.Write(payload);

        var bytes = ms.ToArray();
        this.port.Write(bytes, 0, bytes.Length);
    }

    public void Dispose() {
        try {
            this.ct.Cancel();
            this.ct.Dispose();
        } catch (Exception e) {
            PluginLog.Warning(e, "Error disposing cancellation token");
        }

        try {
            this.port.Close();
            this.port.Dispose();
        } catch (Exception e) {
            PluginLog.Warning(e, "Error disposing port");
        }
    }
}
