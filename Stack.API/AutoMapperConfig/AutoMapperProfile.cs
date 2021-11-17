using AutoMapper;
using Stack.DTOs.Models;
using Stack.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stack.API.AutoMapperConfig
{
    public class AutoMapperProfile : Profile
    {
        //Auto Mapper Configuration File . 
        public AutoMapperProfile()
        {

            CreateMap<ApplicationUser, ApplicationUserDTO>()
            .ReverseMap();

        }

    }
}
