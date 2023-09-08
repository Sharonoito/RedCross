
using System.Net.Http;
using System.Text;
using System;
using System.Threading.Tasks;
using Azure.AI.OpenAI;
using Azure;
using System.Collections.Generic;
using RedCrossChat.Entities;

namespace RedCrossChat
{
    public static class ChatGptDialog
    {
        //connecting the chatbot to open ai

        private static readonly string _apiKey = "6faaad33f68d445c9d8f7f32afe041bc";
        
        private static readonly string AzureOpenAIEndpoint = "https://redcross-2023-connect-7abc-xyz.openai.azure.com/";
        
      //  private static readonly string OpenAIEndpoint = "https://api.openai.com/v1/chat/completions";

        public static async Task<string> GetChatGPTResponses(string prompt,List<AiConversation> aiConversations)
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
                Messages =
                {
                    new ChatMessage(ChatRole.System, "You are a Counselor helping people with mental and social issues."),
                    new ChatMessage(ChatRole.User, "Introduce yourself."),
                },
            };

            while (true)
            {

                foreach (AiConversation conversation in aiConversations)
                {
                   
                    chatCompletionsOptions.Messages.Add(new ChatMessage(ChatRole.System, conversation.Response));
                    chatCompletionsOptions.Messages.Add(new ChatMessage(ChatRole.User, conversation.Question));
                }

                ChatMessage userGreetingMessage = new(ChatRole.User, prompt);

                chatCompletionsOptions.Messages.Add(userGreetingMessage);

                ChatCompletions response = await openAiClient.GetChatCompletionsAsync("redcrosss-chat-gpt", chatCompletionsOptions);

                ChatMessage assistantResponse = response.Choices[0].Message;

                return assistantResponse.Content;
            }

            ///return prompt;
        }
    }
}
