
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;

using RedCrossChat.Objects;
using System;
using System.Threading.Tasks;

namespace RedCrossChat
{
    public static class DialogExtensions
    {
        public static async Task UpdateDialogQuestion(
            string Question,
            
            WaterfallStepContext stepContext, IStatePropertyAccessor<ResponseDto> _userProfileAccessor, UserState _userState)
        {
            var responseDto = await _userProfileAccessor.GetAsync(stepContext.Context, () => new ResponseDto());

            responseDto.Question = Question;
            responseDto.QuestionTimeStamp=DateTime.Now;
            

            await _userProfileAccessor.SetAsync(stepContext.Context, responseDto);
            await _userState.SaveChangesAsync(stepContext.Context);
        }

        public static async Task UpdateDialogConversationId(
           
           Guid ConversationId,
           WaterfallStepContext stepContext, IStatePropertyAccessor<ResponseDto> _userProfileAccessor, UserState _userState)
        {
            var responseDto = await _userProfileAccessor.GetAsync(stepContext.Context, () => new ResponseDto());

            
            responseDto.ConversationId = ConversationId;

            await _userProfileAccessor.SetAsync(stepContext.Context, responseDto);
            await _userState.SaveChangesAsync(stepContext.Context);
        }

        public static async Task UpdateDialogAnswer(string answer,string nextQuestion, WaterfallStepContext stepContext, IStatePropertyAccessor<ResponseDto> _userProfileAccessor, UserState _userState)
        {
            var responseDto = await _userProfileAccessor.GetAsync(stepContext.Context, () => new ResponseDto());


            responseDto.Message = answer;
            responseDto.NextQuestion = nextQuestion;
            responseDto.ResponseTimeStamp=DateTime.Now;

            await _userProfileAccessor.SetAsync(stepContext.Context, responseDto);
            await _userState.SaveChangesAsync(stepContext.Context);
        }
    }
}
