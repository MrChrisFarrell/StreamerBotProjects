using System;

/*----- Class name should match <filename>.cs -----*/
#if EXTERNAL_EDITOR
public class UniqueClassName : CPHInlineBase
#else
public class CPHInline
#endif
/*--------------------------------------------*/
{
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
