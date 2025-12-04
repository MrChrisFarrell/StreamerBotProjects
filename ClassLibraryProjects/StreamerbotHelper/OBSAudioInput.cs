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
        //
        CPH.TryGetArg("requestedInputName", out string requestedInputName);
        //OBSAudioController controller = new(CPH);
        //controller.MuteAudioInput(requestedInputName);
		//OBSAudioInput input = new OBSAudioInput(requestedInputName);
        OBSAudioInput input = new(requestedInputName);
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
        return true;
    }
}
