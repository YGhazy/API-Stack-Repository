using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stack.DAL;
using Stack.DTOs.Models;
using Stack.Entities.Models;
using Stack.Repository;

namespace Stack.Core.Managers
{
    //public class BarberLoginsManager : Repository<BarberLogin, ApplicationDbContext>
    //{
    //    public BarberLoginsManager(ApplicationDbContext _context) : base(_context)
    //    {

    //    }


    //    public async Task<int> GetSignedInbarbers()
    //    {

    //        return await Task.Run(() =>
    //        {
    //            DateTime myday = DateTime.Now;

    //            int Result = context.BarberLogins.Where(A => A.LoginDateTime.Value.Date == myday.Date).Distinct().Count();
    //            Result = Result + 0;

    //            return Result;

    //        });
    //    }

    //}
}
