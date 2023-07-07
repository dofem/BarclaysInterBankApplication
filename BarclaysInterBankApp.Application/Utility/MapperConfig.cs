using AutoMapper;
using BarclaysInterBankApp.Application.Request;
using BarclaysInterBankApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarclaysInterBankApp.Application.Utility
{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {
              CreateMap<Account,AccountCreateRequest>().ReverseMap();
              CreateMap<Account, TopUpRequest>().ReverseMap();
            CreateMap<Account,ChangePinRequest>().ReverseMap();
        }
    }
}
