using DataTables.AspNet.AspNetCore;
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
            var data= await _repository.Conversation.FindAll().Include(x=>x.Persona).ToListAsync();

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

        
        public async Task<IActionResult> EvaluateReport(GraphManagerVm managerVm)
        {

            var conversation = _repository.Conversation;


            var conversations=await conversation.FindAll().ToListAsync();

            return Success("Fetched SuccessFully",conversations);
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

             var item = new Entities.Question { Value = value, Type = Int32.Parse(type) };


             _repository.Question.Create(item);

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
    }
}
