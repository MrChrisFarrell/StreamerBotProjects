namespace StreamerbotHelper;
using System.Text.Json.Serialization;

using Streamer.bot.Plugin.Interface;
using Streamer.bot.Plugin.Interface.Enums;
using Streamer.bot.Plugin.Interface.Model;
using Streamer.bot.Common.Events;
using Streamer.bot.Common;



/*----- Class name should match <filename>.cs -----*/
#if EXTERNAL_EDITOR
public class OBSScene : CPHInlineBase
#else
public class CPHInline
#endif
/*--------------------------------------------*/
{
    public class OBSSource
    {
        public string sourceName {get; set;}
        [JsonIgnore]
        public bool? visible {get; set;}
        public OBSSource(string name)
        {
            sourceName = name;
        }
    }
    public class OBSScene
    {
        public string sceneName {get; set;}
        [JsonIgnore]
        public bool isCurrentScene {get; set;} = false;
        [JsonIgnore]
        public Dictionary<int,OBSSource>? sources {get; set;}
        public OBSScene(string name)
        {
            sceneName = name;
        }
    }
    public bool Execute()
    {
        try
        {
            // Example: Setting an OBS scene based on chat input
            CPH.TryGetArg("rawInputEscaped", out string sceneName); // Get the raw input from the command

            CPH.LogInfo($"sceneName: {sceneName}");

            string sceneRequest = "{\"sceneName\":\"" + sceneName + "\"}";

            CPH.LogInfo($"sceneRequest: {sceneRequest}");

            CPH.ObsSendRaw("SetCurrentProgramScene", sceneRequest, 0);
            
            return true;
        }
        catch (Exception ex)
        {
            CPH.LogInfo($"Error: {ex}");
            return false;
        }
    }
}