using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;
using Stack.DAL;
using Stack.Entities;
using Stack.Repository;
using Stack.DTOs.Enums;
using Stack.Entities.Models;

namespace Stack.Core.Managers
{
    public class ConnectionIdsManager : Repository<ConnectionId, ApplicationDbContext>
    {
        public ConnectionIdsManager(ApplicationDbContext _context) : base(_context)
        {

        }
     
    }
}
