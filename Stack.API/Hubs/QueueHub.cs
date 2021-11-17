using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Stack.Core;
using Stack.DTOs;
using Stack.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stack.API.Hubs
{

    [Authorize]
    public class QueueHub : Hub
    {

        private readonly UnitOfWork unitOfWork;
        private readonly IMapper mapper;
        public QueueHub(UnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        //Add a connection record for the user.
        public async override Task OnConnectedAsync()
        {
            var username = Context.User.Identity.Name;
            var user = await unitOfWork.ApplicationUserManager.FindByNameAsync(username);

            ConnectionId conId = new ConnectionId
            {
                Id = Context.ConnectionId,
                UserId = user.Id
            };

            await unitOfWork.ConnectionIdsManager.CreateAsync(conId);
            await unitOfWork.SaveChangesAsync();

        }

        //Remove user's connection record on disconnecting from the hub.
        public async override Task OnDisconnectedAsync(Exception exception)
        {

            var conId = await unitOfWork.ConnectionIdsManager.GetByIdAsync(Context.ConnectionId);
            await unitOfWork.ConnectionIdsManager.RemoveAsync(conId);
            await unitOfWork.SaveChangesAsync();

        }

        ////Example broadcast method. 
        //public async Task<ApiResponse<bool>> TriggerQueueUpdate() 
        //{
        //    ApiResponse<bool> result = new ApiResponse<bool>();

        //    try
        //    {

        //        //  connected admins will get an alert to update queue infomartion. 
        //        var connectionIds = await unitOfWork.ConnectionIdsManager.GetAsync();
        //        await Clients.Clients(connectionIds.Select(c => c.Id).ToList()).SendAsync("UpdateQueue");

        //        result.Succeeded = true;
        //        result.Data = true;
        //        return result;

        //    }
        //    catch (Exception ex)
        //    {
        //        result.Succeeded = false;
        //        result.Errors.Add(ex.Message);
        //        return result;
        //    }

        //}

    }

}
