using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dalamud.Logging;

namespace PlaydateFishing;

public class PlaydateState {
    private const int PrecisionHooksetSpeed = 45;
    private const int PowerfulHooksetSpeed = 70;
    private const int HooksetReleaseSpeed = 10;

    public bool Initialized;
    public float CrankPosition;
    public bool CrankDocked;
    public bool InHookSpeed;

    private Queue<float> lastCranks = new();

    public void HandleMessage(string type, string content) {
        switch (type) {
            case "hello": {
                PluginLog.Debug("Received hello!");
                this.Initialized = true;
                break;
            }

            case "crank": {
                this.CrankPosition = float.Parse(content);
                break;
            }

            case "crankDock": {
                this.CrankDocked = content == "true";
                if (Plugin.Configuration.AutomaticCastQuit && this.Initialized) {
                    GameFunctions.UseAction(this.CrankDocked ? GameFunctions.Quit : GameFunctions.Cast);
                }
                break;
            }

            case "crankMove": {
                var parts = content.Split(" ");
                var change = float.Parse(parts[0]);
                var accelChange = float.Parse(parts[1]);
                this.HandleCrank(change, accelChange);
                break;
            }

            default: {
                PluginLog.Warning("Unknown message type {Type}: {Content}", type, content);
                break;
            }
        }
    }

    private void HandleCrank(float change, float accelChange) {
        this.CrankPosition += change;

        if (this.InHookSpeed) {
            this.lastCranks.Enqueue(accelChange);

            if (accelChange <= HooksetReleaseSpeed) {
                this.InHookSpeed = false;
            }
        } else {
            this.lastCranks.Clear();

            if (accelChange >= PrecisionHooksetSpeed) {
                this.InHookSpeed = true;

                Task.Run(() => {
                    // Give the player a bit to warm up their speed
                    Task.Delay(500).Wait();

                    if (this.InHookSpeed) {
                        var current = this.lastCranks.Average();
                        var isPowerful = current >= PowerfulHooksetSpeed;
                        PluginLog.Debug($"Call Hook, speed: {current}, powerful: {isPowerful}");
                        GameFunctions.UseHook(isPowerful);
                    }
                });
            }
        }
    }
}
