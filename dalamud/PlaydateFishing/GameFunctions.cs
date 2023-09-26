using System.Linq;
using FFXIVClientStructs.FFXIV.Client.Game;

namespace PlaydateFishing;

public static unsafe class GameFunctions {
    public static void UseAction(uint id, ActionType type = ActionType.Spell) {
        if (Plugin.ClientState.LocalPlayer == null) return;
        if (Plugin.ClientState.LocalPlayer.ClassJob.Id != Constants.Fisher) return;
        ActionManager.Instance()->UseAction(type, id);
    }

    public static void UseHook(bool powerful, Option? hoveredOption) {
        if (Plugin.ClientState.LocalPlayer == null) return;
        var action = Constants.Hook;

        var isPatience = Plugin.ClientState.LocalPlayer.StatusList
            .Any(x => x.StatusId == Constants.InefficientHooking);

        if (isPatience) {
            action = powerful ? Constants.PowerfulHookset : Constants.PrecisionHookset;
        }

        if (Plugin.Configuration.HoverToUse) {
            action = hoveredOption switch {
                Option.DoubleHook => Constants.DoubleHook,
                Option.TripleHook => Constants.TripleHook,
                _ => action
            };
        }

        UseAction(action);
    }
}
