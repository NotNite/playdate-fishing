using System;
using Dalamud.Configuration;
using Newtonsoft.Json;

namespace PlaydateFishing;

[Serializable]
public class Configuration : IPluginConfiguration {
    public int Version { get; set; } = 0;

    [JsonProperty] public string SerialLine = "COM5";
    [JsonProperty] public bool AutomaticCastQuit = true;
    [JsonProperty] public bool HoverToUse = true;
}
