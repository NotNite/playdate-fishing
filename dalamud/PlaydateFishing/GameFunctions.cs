using System.Linq;
using FFXIVClientStructs.FFXIV.Client.Game;

namespace PlaydateFishing;

public static unsafe class GameFunctions {
    public static void UseAction(uint id, ActionType type = ActionType.Spell) {
        if (Plugin.ClientState.LocalPlayer == null) return;
        if (Plugin.ClientState.LocalPlayer.ClassJob.Id != Constants.Fisher) return;
        ActionManager.Instance()->UseAction(type, id);
    }

    public static void UseHook(bool powerful) {
        if (Plugin.ClientState.LocalPlayer == null) return;

        var isPatience = Plugin.ClientState.LocalPlayer.StatusList
            .Any(x => x.StatusId == Constants.InefficientHooking);

        if (isPatience) {
            UseAction(powerful ? Constants.PowerfulHookset : Constants.PrecisionHookset);
        } else {
            UseAction(Constants.Hook);
        }
    }
}
