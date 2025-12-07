namespace StreamerbotHelper;

using System;
using System.Text;
using System.Threading;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Collections.Generic;

using Streamer.bot.Plugin.Interface;
using Streamer.bot.Plugin.Interface.Enums;
using Streamer.bot.Plugin.Interface.Model;
using Streamer.bot.Common.Events;
using Streamer.bot.Common;

/*----- Class name should match <filename>.cs -----*/
#if EXTERNAL_EDITOR
public class OpenAIToFile : CPHInlineBase
#else
public class CPHInline
#endif
/*--------------------------------------------*/
{
    public class OpenAIMessage
    {
        public string role {get; set;}
        public string content {get; set;}
        public OpenAIMessage(string prompt)
        {
            role = "user";
            content = prompt;
        }
    }
    private static readonly HttpClient _httpClient = new HttpClient{Timeout = TimeSpan.FromSeconds(30)};

    public void Init() 
    {
        // Ensure we are working with a clean slate
        _httpClient.DefaultRequestHeaders.Clear();
    }

    public void Dispose() 
    {
        // Free up allocations
        _httpClient.Dispose();
    }
	public bool Execute()
	{
        CPH.TryGetArg("user", out string userName);
        string filePath = CPH.GetGlobalVar<string>("TwitchRewardTextFile",true);
        string apiKeyFilePath = CPH.GetGlobalVar<string>("OpenAIKey",true);
        string chatGPTModel = "gpt-5-nano";
        int characterLimit = CPH.GetGlobalVar<int>("OpenAIResponseCharLimit",true);
        string chatEndPoint = "https://api.openai.com/v1/chat/completions";
        string prompt = $"generate an urban legend about user {userName}. limit to {characterLimit} characters";
		// Grabbing api key
		string apiKey = File.ReadAllText(apiKeyFilePath);
        // Adding api key to Bearer token
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        List<OpenAIMessage> messageList = [];
        OpenAIMessage message = new(prompt);
        messageList.Add(message);
        // Creating request
        var requestBody = new
        {
            model = chatGPTModel,
            messages = messageList
        };
        string json = JsonSerializer.Serialize(requestBody);
        StringContent content = new(json, Encoding.UTF8, "application/json");

        HttpResponseMessage response = _httpClient.PostAsync(chatEndPoint, content).GetAwaiter().GetResult();
		string responseContent = response.Content.ReadAsStringAsync().Result;

        JObject aiResponse = JObject.Parse(responseContent);

        string messageContent = (string)aiResponse["choices"][0]["message"]["content"];
		
		File.WriteAllText(filePath, messageContent);

        string targetScene = CPH.GetGlobalVar<string>("TwitchRewardScrollingTextScene",true);
        string targetSource = CPH.GetGlobalVar<string>("TwitchRewardScrollingTextSource",true);
        int visibilitySeconds = CPH.GetGlobalVar<int>("TwitchRewardScrollingSeconds",true);

        CPH.ObsSetSourceVisibility(targetScene,targetSource,true);
        Thread.Sleep(TimeSpan.FromSeconds(visibilitySeconds));
        CPH.ObsSetSourceVisibility(targetScene,targetSource,false);

		return true;
	}
}