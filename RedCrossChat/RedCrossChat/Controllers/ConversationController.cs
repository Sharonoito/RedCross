using DataTables.AspNet.AspNetCore;
using DataTables.AspNet.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;
using RedCrossChat.Contracts;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RedCrossChat.Controllers
{
    public class ConversationController : BaseController
    {
        private readonly IRepositoryWrapper _repository ;


        public ConversationController(IRepositoryWrapper repository)
        {
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
            var data= await _repository.Conversation.FindAll().Include(x=>x.Client).ToListAsync();

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
    }
}
