using RedCrossChat.Entities;
using RedCrossChat.Entities.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedCrossChat.Contracts
{
    public interface IAuthLoginLog
    : IRepositoryBase<AuthLoginLog>
    {

        Task<IEnumerable<AuthLoginLog>> GetAllAsync();
    }
}
