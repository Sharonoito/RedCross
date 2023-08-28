
using System.Net.Http;
using System.Text;
using System;
using System.Threading.Tasks;

namespace RedCrossChat
{
    public static class ChatGptDialog
    {
        private static readonly string _apiKey = "25b0a86782e343d588f6e540285c7d9e";
        private static readonly string OpenAIEndpoint = "https://redcross-2023-connect-7abc-xyz.openai.azure.com/";

        public static async Task<string> getresponses(string prompt)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");

                var requestData = new
                {
                    prompt = prompt,
                    max_tokens = 50
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
