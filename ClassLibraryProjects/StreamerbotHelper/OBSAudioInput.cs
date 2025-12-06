namespace StreamerbotHelper;
using System.Text.Json;
using Newtonsoft.Json.Linq;
using Streamer.bot.Plugin.Interface;

public class CPHInline: CPHInlineBase
{
    public class OBSAudioInput
    {
        public string inputName { get; set; }
        public bool inputMuted {get; set;}
        public OBSAudioInput(string name)
        {
            inputName = name;
        }
    }
    //ToggleMute
    public bool Execute()
    {
        // Get required arg
        CPH.TryGetArg("requestedInputName", out string requestedInputName);

        // Example of getting optional arg and doing different things
        // CPH.TryGetArg("invalidArgument", out string invalidArgument);
        // if (invalidArgument == null)
        // {
        //     CPH.LogInfo($"invalidArgument is null");
        // }
        // else
        // {
        //     CPH.LogInfo($"invalidArgument is not null");
        // }

        // Creating new instance of OBSAudioInput class
        OBSAudioInput input = new(requestedInputName);

        // Stringifying OBSAudioInput object to send via api
        string serializedRequest = JsonSerializer.Serialize(input);

        //Getting the current mute status from OBS
        string getInputMuteResponse = CPH.ObsSendRaw("GetInputMute", serializedRequest, 0);

        // Parse the string Json into a JObject
        JObject inputMuteJObject = JObject.Parse(getInputMuteResponse);
        //Get the value out of the inputMuted property and converting it to a bool datatype
        input.inputMuted = !(bool)inputMuteJObject["inputMuted"];

        serializedRequest = JsonSerializer.Serialize(input);
        //Setting the mute status
        CPH.ObsSendRaw("SetInputMute",serializedRequest,0);

        //This return means nothing on it's own
        return true;
    }

    //SetMute
    public bool Execute()
    {
        // Get required arg name
        //grabbing rawInput, which is the text after the command used to trigger the sub-action
        CPH.TryGetArg("rawInput", out string requestedInputName);

        // Creating new instance of OBSAudioInput class
        OBSAudioInput input = new(requestedInputName)
        {
            // Setting desired mute value
            inputMuted = true
        };

        // Stringifying OBSAudioInput object to send via api
        string serializedRequest = JsonSerializer.Serialize(input);

        //Setting the mute status
        CPH.ObsSendRaw("SetInputMute",serializedRequest,0);

        //This return means nothing on it's own
        return true;
    }
}
