using System.Threading.Tasks;
using Dalamud.Game;
using Dalamud.Game.ClientState;
using Dalamud.Game.Gui;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.IoC;
using Dalamud.Plugin;

namespace PlaydateFishing;

public sealed class Plugin : IDalamudPlugin {
    public string Name => "Playdate Fishing";

    [PluginService] public static DalamudPluginInterface PluginInterface { get; private set; } = null!;
    [PluginService] public static ChatGui ChatGui { get; private set; } = null!;
    [PluginService] public static Framework Framework { get; private set; } = null!;
    [PluginService] public static ClientState ClientState { get; private set; } = null!;

    public static Configuration Configuration = null!;
    public static PlaydateState PlaydateState = null!;
    public static PlaydateSerial PlaydateSerial = null!;

    public Plugin() {
        Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new();

        PlaydateState = new();
        PlaydateSerial = new(Configuration.SerialLine);

        PlaydateSerial.Connect();

        Framework.Update += this.FrameworkUpdate;
    }

    public static void LogToChat(string message, bool error = false) {
        var se = new SeStringBuilder()
            .AddUiForeground("[Playdate Fishing] ", 559);

        se = error
                 ? se.AddUiForeground(message, 17)
                 : se.AddText(message);

        ChatGui.Print(se.Build());
    }

    private void FrameworkUpdate(Framework framework) {
        // TODO send GP to playdate
    }

    public void Dispose() {
        PlaydateSerial.Dispose();
        Framework.Update -= this.FrameworkUpdate;
    }
}
