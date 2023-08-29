
using System.Net.Http;
using System.Text;
using System;
using System.Threading.Tasks;

namespace RedCrossChat
{
    public static class ChatGptDialog
    {
        private static readonly string _apiKey = "6faaad33f68d445c9d8f7f32afe041bc";
      //  private static readonly string OpenAIEndpoint = "https://redcross-2023-connect-7abc-xyz.openai.azure.com/";
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
        
    }
}
