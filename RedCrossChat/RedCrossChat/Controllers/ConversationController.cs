﻿using DataTables.AspNet.AspNetCore;
using DataTables.AspNet.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;
using RedCrossChat.Contracts;
using RedCrossChat.Entities.Auth;
using RedCrossChat.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using RedCrossChat.Objects;
using Sentry;
using Constants = RedCrossChat.Objects.Constants;
using System.Reflection.Metadata;
using Antlr4.Runtime.Misc;

namespace RedCrossChat.Controllers
{
    public class ConversationController : BaseController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<AppRole> _roleManager;

        private readonly IRepositoryWrapper _repository;

        public ConversationController(UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        RoleManager<AppRole> roleManager,
        IRepositoryWrapper repository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _repository = repository;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult List()
        {
            return View();
        }

        public async Task<IActionResult> Conversations(IDataTablesRequest dtRequest)
        {
            var data= await _repository.Conversation.FindAll()
                .Include(x=>x.Persona).ThenInclude(x=>x.Feeling)
                .Include(x=>x.Persona).ThenInclude(x=>x.AgeBand)
                .Include(x=>x.Persona).ThenInclude(x=>x.County)
                .Include(x=>x.Persona).ThenInclude(x=>x.Profession)
                .Include(x=>x.Persona).ThenInclude(x=>x.MaritalState)
                .ToListAsync();

            var filteredRows = data
                 .AsQueryable()
                 .FilterBy(dtRequest.Search, dtRequest.Columns);

            var pagedRows = filteredRows
                    .SortBy(dtRequest.Columns)
                    .Skip(dtRequest.Start)
                    .Take(dtRequest.Length);

            var response = DataTablesResponse.Create(dtRequest, data.Count(),
                filteredRows.Count(), pagedRows);

            return new DataTablesJsonResult(response);
        }

        [HttpPost]
        public async Task<IActionResult> DetailedConversation(Guid id)
        {
            var rawConversations=await _repository.RawConversation.FindByCondition(x=>x.ConversationId == id).ToListAsync();

            return Success("Fetched successfully", rawConversations);
        }

        [HttpPost]
        public async Task<IActionResult> EvaluateReport(IFormCollection formData)
        {

            var conversation = _repository.Conversation;


            string feeling = formData["Feeling"];

            string age = formData["AgeBand"];

            string gender = formData["Gender"];

            string county = formData["County"];

            string intention = formData["Intention"];

           // string county = formData["County"];

            
                
            IQueryable<Conversation> conversationObject=conversation.FindAll();

            
            if(feeling !="-1" && age !="-1" && county !="-1") {

                conversationObject = conversation.FindByCondition(x => x.Persona.FeelingId == Guid.Parse(feeling) && 
                
                x.Persona.AgeBandId==Guid.Parse(age) && x.Persona.CountyId==Guid.Parse(county));

            }else if(feeling != "-1" && age != "-1")
            {
                conversationObject = conversation.FindByCondition(x => 
                x.Persona.FeelingId == Guid.Parse(feeling) &&
                x.Persona.AgeBandId == Guid.Parse(age) );

            }else if(feeling != "-1"  && county != "-1")
            {
                conversationObject = conversation.FindByCondition(x =>
                x.Persona.FeelingId == Guid.Parse(feeling) &&
                x.Persona.CountyId == Guid.Parse(county));

            }else if( age != "-1" && county != "-1")
            {
                conversationObject = conversation.FindByCondition(x =>
                   x.Persona.AgeBandId == Guid.Parse(age) &&
                   x.Persona.CountyId == Guid.Parse(county));
            }
            else
            {
                if (feeling != "-1")
                {
                    conversationObject = conversation.FindByCondition(x => x.Persona.FeelingId == Guid.Parse(feeling));

                }

                if (age != "-1")
                {
                    conversationObject = conversation.FindByCondition(x => x.Persona.AgeBandId == Guid.Parse(age));
                }

                if (county != "-1")
                {
                    conversationObject = conversation.FindByCondition(x => x.Persona.CountyId == Guid.Parse(county));
                }
            }


            List<Conversation> conversations= await conversationObject.Include(x => x.Persona)
                    .ThenInclude(x => x.Profession).Include(x => x.Persona)
                    .ThenInclude(x => x.AgeBand).Include(x => x.Persona).ThenInclude(x => x.Feeling).Include(x => x.Persona).ThenInclude(x => x.MaritalState).ToListAsync();
 

            var handedOver=await conversation.FindByCondition(x=>x.HandedOver==true).ToListAsync();

            var items = new Dictionary<string, int>();

            foreach(var item in conversations)
            {
                var time= $"{item.DateCreated.Year}-{item.DateCreated.Month}-{item.DateCreated.Day} ";

                if (items.ContainsKey(time))
                {
                    items[time]++;
                }
                else
                {
                    items[time] = 1 ;
                }
               
            }


            var report = new DashboardReportVM
            {
                TotalVisits=conversations.Count,
                HandledByAgents=handedOver.Count,
                items=items,
               
            };

            return Success("Fetched SuccessFully", report);
        }

        public IActionResult Flow()
        {
            return View();
        }

        public IActionResult CreateQuestion()
        {
            return View("_Question");
        }

        [HttpPost]
        public async Task<IActionResult> SaveQuestion(IFormCollection formData)
        {

            if (!ModelState.IsValid)
                return Error("Validation error!, please check your data.");


            string value = formData["Value"];
            string type = formData["Type"];

             //var item = new Entities.Question { Value = value, Type = Int32.Parse(type) };


            // _repository.Question.Create(item);

             bool status = await _repository.SaveChangesAsync();

             if (status)
             {
                 return Success("Question Created successfully");
             }

            return Ok("Something Broke");
        }
        [HttpPost]
        public async Task<IActionResult> GetConversationFlows()
        {
            var questions=await _repository.Question.GetAll();

            var results=new List<QuestionResponseVm>();

            foreach (var question in questions)
            {
                List<QuestionOption> options = new List<QuestionOption>();

                if (question.Type == 1)
                {
                    options= await _repository.QuestionOption.FindByCondition(x => x.QuestionId == question.Id).ToListAsync();
                }
                results.Add(new QuestionResponseVm { question = question,options=options });
            }

            return Success("Fetched",results);
        }
        
        public async Task<IActionResult> GetConversationFlow(IFormCollection formData)
        {
            var id = formData["id"];

            var question =await _repository.Question.FindByCondition(x => x.Id == Guid.Parse(id)).FirstOrDefaultAsync();

            if (question != null)
            {
                var questionOptions=await _repository.QuestionOption.FindByCondition(x => x.QuestionId == question.Id).ToListAsync();

                 return Success("question", new QuestionResponseVm { question=question,options=questionOptions });
            }
   
            return Success("Question", new QuestionResponseVm { question = question });
        }

        public async Task<IActionResult> CheckHandOverRequests()
        { 
            var conversations=await _repository.HandOverRequest.FindByCondition(x=>x.HasBeenReceived==false).ToListAsync();

            //first in First out

            if (conversations.Count > 0)
            {
                return Success("fetched successfully", conversations);
            }

            return Error("No requests");
        }

        public async Task<IActionResult> SaveQuestionOption(IFormCollection formData)
        {

            string value = formData["value"];

            string type = formData["type"];

            string questionId = formData["questionId"];

            _repository.QuestionOption.Create(new QuestionOption { Value=value,Action=Int32.Parse(type),QuestionId=Guid.Parse(questionId) });

            var status =await _repository.SaveChangesAsync();

            if (status)
            {
                return Success("Created SuccessFully");
            }

            return Ok("Uploaded");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateHandOverRequest(Guid id)
        {

            if(id != Guid.Empty)
            {
                var request=await _repository.HandOverRequest.FindByCondition(x=>x.Id==id).FirstOrDefaultAsync();

                if (request != null)
                {                  
                    var conversation=  await _repository.Conversation.
                       
                        FindByCondition(x=>x.Id==request.ConversationId).Include(x=>x.Persona).FirstOrDefaultAsync();

                    if(conversation.AppUserId != null)
                    {

                        request.HasBeenReceived = true;

                        _repository.HandOverRequest.Update(request);

                        await _repository.SaveChangesAsync();

                        return Success("This Client is already being supported",request);
                    }
                    else
                    {
                        request.HasBeenReceived = true;

                        request.ResolvedAt = DateTime.Now;

                        conversation.AppUserId = Guid.Parse(User.FindFirst("UserId").Value);

                        if(conversation.Persona.ChatID == null)
                        {  
                            conversation.Persona.ChatID = await GetUserCode();

                            _repository.Persona.Update(conversation.Persona);
                        }

                        var chatMessages= await _repository.ChatMessage.FindByCondition(x => x.ConversationId == request.ConversationId).ToListAsync();

                        for(int i=0;i<chatMessages.Count; i++)
                        {

                            chatMessages[i].IsRead = true;
                        }

                        _repository.ChatMessage.UpdateRange(chatMessages);
                       
                        _repository.Conversation.Update(conversation);

                        _repository.HandOverRequest.Update(request);

                        var status = await _repository.SaveChangesAsync();


                        var data = await _repository.Conversation.FindByCondition(x => x.Id == request.ConversationId)             
                                   .Include(x => x.ChatMessages).ThenInclude(x=>x.Question)
                                   .Include(x=>x.Persona)
                                   .FirstOrDefaultAsync();

                        if (status)
                            return Success("Updated Successfully",data);
                    }  
                }

            }

            return Error("Invalid Guid");

        }


        public async Task<string> GetUserCode()
        {

            var lastest = await _repository.Persona.FindAll().ToListAsync();

            var date_created=lastest.Last().DateCreated;

            var conversations=await _repository.Conversation.FindAll().ToListAsync();

            decimal count = decimal.Parse(conversations.Count.ToString());

            decimal counter = count/ 10000;

            

            string[] item = counter.ToString().Split('.');

            if (counter > 1)
            {
                double power = 10000 * Math.Pow(10, item[0].Length);

                counter = count / decimal.Parse(power.ToString());

                item=counter.ToString().Split(".");
            }


            string code = $"{date_created.Year}{date_created.Month}{date_created.Day}{item[1]}" ;

            return code;
        }

        [HttpPost]
        public async Task<IActionResult> GetMyConversations()
        {
            var conversations=await _repository.Conversation.
                FindByCondition(x=>x.AppUserId== Guid.Parse(User.FindFirst("UserId").Value)).
                Include(x=>x.Persona).
                Include(x=>x.ChatMessages)
                .ThenInclude(x=>x.Question)
                .OrderByDescending(x=>x.DateCreated).
                ToListAsync();

            return Success("Items fetched Successfully", conversations);
        }
        [HttpGet]
        public async Task<IActionResult> GetConversation(Guid id)
        {

            var conv = await _repository.Conversation.FindByCondition(x=>x.Id==id).Include(x=>x.RawConversations).FirstOrDefaultAsync();

           //= await _repository.RawConversation.FindByCondition(x => x.ConversationId == id).ToListAsync();

            return Success("Fetched Successfully", conv);
        }

        [HttpPost]
        public async Task<IActionResult> CreateResponse(RawConversationVm rawConversation)
        {

            var request = await _repository.HandOverRequest.FindByCondition(x => x.ConversationId == rawConversation.ConversationId)
                         .FirstOrDefaultAsync();

            var chatMessagesFromUser= await _repository.ChatMessage.FindByCondition(x=>x.ConversationId==rawConversation.ConversationId 
            & x.IsRead==false & x.Type==Constants.User).ToListAsync();


            foreach (var conv in chatMessagesFromUser)
            {
                conv.IsRead = true;
            }

            ChatMessage chat = new ChatMessage()
            {
                Message= rawConversation.Question,
                ConversationId=rawConversation.ConversationId,
                Type=Constants.Bot
            };

            request.HasResponse = true;
            request.LastChatMessage = chat;

            _repository.ChatMessage.Create(chat);

            _repository.ChatMessage.UpdateRange(chatMessagesFromUser);

            _repository.HandOverRequest.Update(request);

            bool status= await _repository.SaveChangesAsync();

            if (status)
                return Success("Client Notified Successfully");
            
            
            return Error("There was an error updating this");
        }

        [HttpPost]
        public async Task<IActionResult> GetRawConversations(Guid id)
        {
            var chatMessages =await _repository.ChatMessage.FindByCondition(x => x.ConversationId == id)
                .Include(x=>x.Question)
                .ToListAsync();

            ArrayList<ChatMessage> chat = new ArrayList<ChatMessage>();

            foreach(var chatMessage in chatMessages)
            {

                if(chatMessage.IsRead == false)
                {
                    chat.Add(chatMessage);
                };
            }

            foreach(var chatMessage in chat)
            {
                chatMessage.IsRead = true;
            }

            _repository.ChatMessage.UpdateRange(chatMessages);

            await _repository.SaveChangesAsync();

            return Success("success", chatMessages);
        }
        [HttpPost]
        public async Task<IActionResult> GetMyConversationIncludingHandOverRequests(String Id="")
        {
            var userId = Guid.Parse(User.FindFirst("UserId").Value);

            var handOverRequests = await _repository.HandOverRequest
                .FindByCondition(x => x.HasBeenReceived == false)
                .Include(x=>x.Conversation)
                .ThenInclude(x=>x.Persona)
                .Include(x=>x.Conversation)
                .ThenInclude(x=>x.Feeling)
                .Include(x=>x.Conversation)
                .ThenInclude(x=>x.Intention)
                .Include(x=>x.Conversation)
                .ThenInclude(x=>x.SubIntention)
                .ToListAsync();

            var myHandOverRequests = handOverRequests
                .Where(x => x.AppUserId == userId)
                .ToList();

            var conversationObject= _repository.Conversation
                .FindByCondition(x => x.AppUserId == Guid.Parse(User.FindFirst("UserId").Value) & x.IsActive);

            if(Id != "")
            {

               Guid activeConversationId = Guid.Parse(Id);

               var messages=await _repository.ChatMessage.FindByCondition(x => x.ConversationId==activeConversationId).ToListAsync();
                

                ArrayList<ChatMessage> messagesList = new ArrayList<ChatMessage>();

                foreach(var message in messages)
                {
                    if (message.Type == 3)
                    {
                        message.IsRead = true;

                        messagesList.Add(message);
                    }
                }

                if(messagesList.Count > 0)
                {
                    _repository.ChatMessage.UpdateRange(messagesList);

                    bool updated = await _repository.SaveChangesAsync();
                }
            }



            var myConversations= await conversationObject.Include(x => x.Persona)
                .Include(x => x.ChatMessages)
                .ThenInclude(x => x.Question)
                .OrderByDescending(x => x.DateCreated)
                .ToListAsync();

            return Success("Response", new ChatResponseVm
            {
                handOverRequests = handOverRequests,
                myConversations = myConversations,
                myHandOverRequests = myHandOverRequests
            });
        }

        public async Task<IActionResult> GetHistory(Guid id)
        {
            var conversations =await _repository.Conversation.FindByCondition(x=>x.PersonaId == id)
                .Include(x=>x.ChatMessages)
                .ThenInclude(x=>x.Question)
                .ToListAsync();

            return Success("closer", conversations);
        }
    }
}
