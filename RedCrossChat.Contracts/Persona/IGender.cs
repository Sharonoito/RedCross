using RedCrossChat.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedCrossChat.Contracts
{
    public interface IGender : IRepositoryBase<Gender>
    {
        Task<IEnumerable<Gender>> GetAll();
    }
}
