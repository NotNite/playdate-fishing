using System;
using System.Text;
using System.Text.Json;
using Dalamud.Logging;

namespace PlaydateFishing;

// I could do this properly writing Lua bytecode, or I can do what ericlewis did and stuff strings into hex arrays
// Thanks: https://gist.github.com/ericlewis/43d07016275308de11a5519466deea85
public class InputCompiler {
    private static byte[] Pre = {
        0x1B, 0x4C, 0x75, 0x61,                   // magic
        0x54,                                     // version (lua 5.4)
        0x00, 0x19, 0x93, 0x0D, 0x0A, 0x1A, 0x0A, // more magic?
        0x04,                                     // instruction_size
        0x04,                                     // integer_format
        0x04,                                     // float_format
        0x78, 0x56, 0x00, 0x00,                   // test integer (0x5678)
        0x00, 0x40, 0xB9, 0x43,                   // test float (370.5)
        0x01,                                     // to be honest idk man

        // the filename ("main.lua")
        0x8A, 0x40,                                     // string size
        0x6D, 0x61, 0x69, 0x6E, 0x2E, 0x6C, 0x75, 0x61, // string data

        0x80, // linedefined
        0x80, // lastlinedefined
        0x00, // numparams
        0x01, // is_varag
        0x02, // max_stacksize

        // I lost track after this because I was too lazy to figure out how to
        // parse packed numbers in 010 Editor
        0x85, 0x4F, 0x00, 0x00, 0x00, 0x09, 0x00, 0x00, 0x00, 0x83, 0x80, 0x00,
        0x00, 0x42, 0x00, 0x02, 0x01, 0x44, 0x00, 0x01, 0x01, 0x82, 0x04
    };

    private static byte[] Post = {
        0x81, 0x01, 0x00, 0x00, 0x80, 0x85, 0x01, 0x00, 0x00, 0x00, 0x00, 0x80,
        0x80, 0x81, 0x85, 0x5F, 0x45, 0x4E, 0x56
    };

    private static string StringToHex(string str) {
        var bytes = Encoding.UTF8.GetBytes(str);

        var sizeStr = (129 + bytes.Length).ToString("X2");
        var bytesStr = Convert.ToHexString(bytes);

        return sizeStr + bytesStr;
    }

    public static string Build(string content) {
        var preStr = Convert.ToHexString(Pre);
        var postStr = Convert.ToHexString(Post);

        var functionHex = StringToHex("input");
        var contentHex = StringToHex(content);

        var complete = preStr
                       + functionHex
                       + "04"
                       + contentHex
                       + postStr;

        PluginLog.Log(complete);
        return complete;
    }

    public static string Build(object obj) {
        var json = JsonSerializer.Serialize(obj, new JsonSerializerOptions {
            IncludeFields = true
        });
        return Build(json);
    }
}
