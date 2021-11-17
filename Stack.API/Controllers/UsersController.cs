using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stack.API.Controllers.Common;
using Stack.DTOs.Models;
using Stack.DTOs.Requests;
using Stack.ServiceLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using Stack.Entities.Models;
using System.Threading.Tasks;
using Stack.DTOs.Requests.Users;

namespace Stack.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Require Authorization to access API endpoints . 
    public class UsersController : BaseResultHandlerController<UsersService>
    {

        public UsersController(UsersService _service) : base(_service)
        {

        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> LoginAsync(LoginModel model)
        {
            return await GetResponseHandler(async () => await service.LoginAsync(model));
        }

        [AllowAnonymous]  
        [HttpPost("CreateSuperAdministratorAccount")]
        public async Task<IActionResult> CreateSuperAdministratorAccount(UserRegistrationModel model)
        {
            return await AddItemResponseHandler(async () => await service.CreateSuperAdministratorAccount(model));
        }

        [AllowAnonymous] 
        [HttpPost("InitializeSystemRoles")]
        public async Task<IActionResult> InitializeSystemRoles()
        {
            return await AddItemResponseHandler(async () => await service.InitializeSystemRoles());
        }


        [AllowAnonymous]
        [HttpPost("EditSuperAdministratorPassword")]
        public async Task<IActionResult> EditSuperAdministratorPassword(EditSuperAdminPasswordModel id)
        {
            return await EditItemResponseHandler(async () => await service.EditSuperAdministratorPassword(id));
        }

        [AllowAnonymous]
        [HttpPost("EditSuperAdministratorAccountDetails")]
        public async Task<IActionResult> EditSuperAdministratorAccountDetails(EditSuperAdminAccountModel id)
        {
            return await EditItemResponseHandler(async () => await service.EditSuperAdministratorAccountDetails(id));
        }

    }

}


 
