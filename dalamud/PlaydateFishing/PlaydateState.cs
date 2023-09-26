using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.Game;

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
    private Option? hoveredOption;

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
                    GameFunctions.UseAction(this.CrankDocked ? Constants.Quit : Constants.Cast);
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

            case "option": {
                var option = (Option) int.Parse(content);
                this.HandleOption(option);
                break;
            }

            case "hover": {
                var option = (Option) int.Parse(content);
                this.hoveredOption = option;
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
                        GameFunctions.UseHook(isPowerful, this.hoveredOption);
                    }
                });
            }
        }
    }

    private void HandleOption(Option option) {
        switch (option) {
            case Option.Cast: {
                GameFunctions.UseAction(Constants.Cast);
                break;
            }

            case Option.Hook: {
                GameFunctions.UseAction(Constants.Hook);
                break;
            }

            case Option.Quit: {
                GameFunctions.UseAction(Constants.Quit);
                break;
            }

            case Option.Patience: {
                GameFunctions.UseAction(Constants.Patience);
                break;
            }

            case Option.PatienceII: {
                GameFunctions.UseAction(Constants.PatienceII);
                break;
            }

            case Option.PrizeCatch: {
                GameFunctions.UseAction(Constants.PrizeCatch);
                break;
            }

            case Option.IdenticalCast: {
                GameFunctions.UseAction(Constants.IdentitalCast);
                break;
            }

            case Option.SurfaceSlap: {
                GameFunctions.UseAction(Constants.SurfaceSlap);
                break;
            }

            case Option.ThaliaksFavor: {
                GameFunctions.UseAction(Constants.ThaliaksFavor);
                break;
            }

            case Option.HiCordial: {
                GameFunctions.UseAction(Constants.HiCordial, ActionType.Item);
                break;
            }

            case Option.Mooch: {
                GameFunctions.UseAction(Constants.Mooch);
                break;
            }

            case Option.MoochII: {
                GameFunctions.UseAction(Constants.MoochII);
                break;
            }

            case Option.DoubleHook: {
                GameFunctions.UseAction(Constants.DoubleHook);
                break;
            }

            case Option.TripleHook: {
                GameFunctions.UseAction(Constants.TripleHook);
                break;
            }

            default:
                throw new ArgumentOutOfRangeException(nameof(option), option, null);
        }
    }
}
