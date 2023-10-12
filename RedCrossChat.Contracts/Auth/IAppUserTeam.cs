using RedCrossChat.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedCrossChat.Contracts

{
    public interface IAppUserTeam : IRepositoryBase<AppUserTeam>
    {
        Task<IEnumerable<AppUserTeam>> GetAll();

        Task<IEnumerable<AppUserTeam>> GetAllAsync();

    }
}





