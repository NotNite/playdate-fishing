using System.Linq;
using FFXIVClientStructs.FFXIV.Client.Game;

namespace PlaydateFishing;

public static unsafe class GameFunctions {
    public const uint Fisher = 18;

    public const uint Cast = 289;
    public const uint Hook = 296;
    public const uint Quit = 299;

    public const uint PrecisionHookset = 4179;
    public const uint PowerfulHookset = 41093;
    public const uint InefficientHooking = 764;

    public static void UseAction(uint id, ActionType type = ActionType.Spell) {
        if (Plugin.ClientState.LocalPlayer == null) return;
        if (Plugin.ClientState.LocalPlayer.ClassJob.Id != Fisher) return;
        ActionManager.Instance()->UseAction(type, id);
    }

    public static void UseHook(bool powerful) {
        if (Plugin.ClientState.LocalPlayer == null) return;
        var isPatience = Plugin.ClientState.LocalPlayer.StatusList.Any(x => x.StatusId == InefficientHooking);
        if (isPatience) {
            UseAction(powerful ? PowerfulHookset : PrecisionHookset);
        } else {
            UseAction(Hook);
        }
    }
}
