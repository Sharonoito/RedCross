﻿
using System.Net.Http;
using System.Text;
using System;
using System.Threading.Tasks;
using Azure.AI.OpenAI;
using Azure;
using System.Collections.Generic;
using RedCrossChat.Entities;
using ChatMessage = Azure.AI.OpenAI.ChatMessage;

namespace RedCrossChat
{
    public static class ChatGptDialog
    {
        //connecting the chatbot to open ai

        private static readonly string _apiKey = "6faaad33f68d445c9d8f7f32afe041bc";
        
        private static readonly string AzureOpenAIEndpoint = "https://redcross-2023-connect-7abc-xyz.openai.azure.com/";
       
        public static async Task<string> GetChatGPTResponses(string prompt,Conversation conversation,bool language=true)
        {

            List<AiConversation> aiConversations = conversation.AiConversations;    


            var openAiClient = new OpenAIClient(
                new Uri(AzureOpenAIEndpoint),
                new AzureKeyCredential(_apiKey)
            );

            string personal = "someone who is";
            
            string preText = "You are a Counselor based in Kenya redcross who's name is ChatCare,you are helping people with mental and social issues." + (!language ? " respond in swahili" : "");

            if(conversation.Feeling != null)
            {
                personal += " feeling " + conversation.Feeling.Name;
            }
            else
            {
                personal += " feeling " + conversation.FeelingDetail;
            }

           /* if(conversation.Persona.AgeBand != null)
            {
               // personal 
            }*/



            var chatCompletionsOptions = new ChatCompletionsOptions
            {
                MaxTokens = 400,
                Temperature = 1.8f,
                FrequencyPenalty = 0.0f,
                PresencePenalty = 0.0f,
                NucleusSamplingFactor = 0.95f,
                Messages =
                {
                    new ChatMessage(ChatRole.System, preText),
                  
                },
            };

            chatCompletionsOptions.Messages.Add(new ChatMessage(ChatRole.User, "Who are you"));
            chatCompletionsOptions.Messages.Add(new ChatMessage(ChatRole.System, "My name is ChatCare , i am an open ai model trained to help people with mental health"));


            chatCompletionsOptions.Messages.Add(new ChatMessage(ChatRole.User, "Where can I find several helplines and organizations that offer support for mental health issues in Kenya"));
            chatCompletionsOptions.Messages.Add(new ChatMessage(ChatRole.System, "KNH provides support and information on mental health issues. You can call them at +254 722 998 767"));

            chatCompletionsOptions.Messages.Add(new ChatMessage(ChatRole.User, "How do you approach counseling for children and adolescents in Kenya, considering their unique needs?"));
            chatCompletionsOptions.Messages.Add(new ChatMessage(ChatRole.System, "Counseling for children and adolescents in Kenya requires an age-appropriate approach. I use play therapy, art therapy, and engage in age-specific conversations to make counseling relatable and effective"));

            chatCompletionsOptions.Messages.Add(new ChatMessage(ChatRole.User, " What are some common mental health challenges that people in Kenya face, and how do you address them in your counseling sessions?"));
            chatCompletionsOptions.Messages.Add(new ChatMessage(ChatRole.System, "People in Kenya, like anywhere else, face various mental health challenges, including anxiety, depression, and trauma. I address these challenges by providing a safe and non-judgmental space for clients to talk about their experiences. I use evidence-based therapeutic approaches and work collaboratively with clients to develop coping strategies and resilience."));

            while (true)
            {
                if(aiConversations !=null)
                foreach (AiConversation conv in aiConversations)
                {
                   
                    chatCompletionsOptions.Messages.Add(new ChatMessage(ChatRole.System, conv.Response));
                    chatCompletionsOptions.Messages.Add(new ChatMessage(ChatRole.User, conv.Question));
                }

                ChatMessage userGreetingMessage = new(ChatRole.User, prompt);

                chatCompletionsOptions.Messages.Add(userGreetingMessage);

                ChatCompletions response = await openAiClient.GetChatCompletionsAsync("redcrosss-chat-gpt", chatCompletionsOptions);

                ChatMessage assistantResponse = response.Choices[0].Message;

                return assistantResponse.Content;
            }

            
        }
    }
}
