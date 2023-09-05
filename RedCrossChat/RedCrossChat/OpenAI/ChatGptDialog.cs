
using System.Net.Http;
using System.Text;
using System;
using System.Threading.Tasks;
using Azure.AI.OpenAI;
using Azure;

namespace RedCrossChat
{
    public static class ChatGptDialog
    {
        private static readonly string _apiKey = "6faaad33f68d445c9d8f7f32afe041bc";
         private static readonly string AzureOpenAIEndpoint = "https://redcross-2023-connect-7abc-xyz.openai.azure.com/";
        private static readonly string OpenAIEndpoint = "https://api.openai.com/v1/chat/completions";

        public static async Task<string> getresponses(string prompt)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");

                var requestData = new
                {
                    prompt = prompt,
                    max_tokens = 50,
                    modelName= "gpt-35-turbo"
                };

                var json = Newtonsoft.Json.JsonConvert.SerializeObject(requestData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(OpenAIEndpoint, content);
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    dynamic jsonResponse = Newtonsoft.Json.JsonConvert.DeserializeObject(responseContent);
                    string generatedText = jsonResponse.choices[0].text;
                    return generatedText;
                }
                else
                {
                    throw new Exception($"OpenAI request failed with status code {response.StatusCode}");
                }
            }
        }
        

        public static async Task<string> GetChatGPTResponses(string prompt)
        {
            var openAiClient = new OpenAIClient(
                new Uri(AzureOpenAIEndpoint),
                new AzureKeyCredential(_apiKey)
            );

            var chatCompletionsOptions = new ChatCompletionsOptions
            {
                MaxTokens = 400,
                Temperature = 1f,
                FrequencyPenalty = 0.0f,
                PresencePenalty = 0.0f,
                NucleusSamplingFactor = 0.95f,
                Messages = {
                    new ChatMessage(ChatRole.System, "You are a Counselor helping people with mental and social issues."),
                    new ChatMessage(ChatRole.User, "Introduce yourself."),
                 }
            };

            while (true)
            {

                ChatMessage userGreetingMessage = new(ChatRole.User, prompt);

                chatCompletionsOptions.Messages.Add(userGreetingMessage);


                ChatCompletions response = await openAiClient.GetChatCompletionsAsync("redcrosss-chat-gpt", chatCompletionsOptions);

                ChatMessage assistantResponse = response.Choices[0].Message;

                return assistantResponse.Content;
            }

            return prompt;
        }
    }
}
