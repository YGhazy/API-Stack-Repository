using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Stack.DAL;
using Stack.Entities.Enums;
using Stack.Entities.Models;
using Stack.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stack.Core.Managers
{
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {

        //public DbSet<ApplicationUser> Users;

        public ApplicationUserManager(ApplicationDbContext _context, IUserStore<ApplicationUser> store, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<ApplicationUser> passwordHasher, IEnumerable<IUserValidator<ApplicationUser>> userValidators, IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<ApplicationUser>> logger) : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {



        }

        public async Task<ApplicationUser> GetCurrentUserAsync(string username)
        {
            return await FindByNameAsync(username);
        }



    }
}
