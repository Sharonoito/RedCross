﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio CoreBot v4.18.1

using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using RedCrossChat.Contracts;
using RedCrossChat.Dialogs;
using RedCrossChat.Objects;
using System.Threading;
using System.Threading.Tasks;

namespace RedCrossChat.Bots
{

    public  class DialogBot<T> : ActivityHandler
        where T : Dialog
    {
        protected readonly Dialog Dialog ;

        protected readonly DialogSet dialogSet;
        protected readonly BotState ConversationState ;
        protected readonly BotState UserState ;
        protected readonly ILogger Logger ;

        private readonly IRepositoryWrapper _repository;
        protected readonly DialogSet _dialog;
        private readonly IStatePropertyAccessor<ResponseDto> _userProfileAccessor;

        //calling the dialog context's cancel all dialogs

        public DialogBot(ConversationState conversationState, UserState userState, T dialog, ILogger<DialogBot<T>> logger, IRepositoryWrapper repository)
        {
            ConversationState = conversationState;
            UserState = userState;
            Dialog = dialog;
            Logger = logger;
            _repository = repository;
            _userProfileAccessor = userState.CreateProperty<ResponseDto>(DialogConstants.ProfileAssesor);

            this.dialogSet = new DialogSet(conversationState.CreateProperty<DialogState>("DialogState"));

            
        }

        public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default)
        {
           await base.OnTurnAsync(turnContext, cancellationToken);

          

            // Save any state changes that might have occured during the turn.
            await ConversationState.SaveChangesAsync(turnContext, false, cancellationToken);
            await UserState.SaveChangesAsync(turnContext, false, cancellationToken);


        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var userInput = turnContext.Activity.Text.ToLowerInvariant();

            var responseDto = await GetUserProfile(turnContext, cancellationToken);

            await Dialog.RunAsync(turnContext, ConversationState.CreateProperty<DialogState>("DialogState"), cancellationToken);

            await SaveResponse(turnContext, responseDto, cancellationToken);
        }

        private async Task<ResponseDto> GetUserProfile(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var userProfile = await _userProfileAccessor.GetAsync(turnContext, () => new ResponseDto(), cancellationToken);
            return userProfile;
        }

        private async Task SaveResponse(ITurnContext<IMessageActivity> turnContext, ResponseDto responseDto, CancellationToken cancellationToken)
        {
            //this saves the question asked to the database

            if (!string.IsNullOrEmpty(responseDto.Question) && !string.IsNullOrEmpty(responseDto.Message))
            {
                _repository.RawConversation.Create(new Entities.RawConversation
                { Question = responseDto.Question, 
                    ConversationId = responseDto.ConversationId,
                    Message = turnContext.Activity.Text,QuestionTimeStamp=responseDto.QuestionTimeStamp,ResponseTimeStamp=responseDto.ResponseTimeStamp });

                bool response=await _repository.SaveChangesAsync();

                if(response)
                {
                    responseDto.Question=responseDto.NextQuestion;
                    responseDto.Message = null;

                    await _userProfileAccessor.SetAsync(turnContext, responseDto, cancellationToken);
                    await UserState.SaveChangesAsync(turnContext);
                }
            }
        }
    }
}
