using BarclaysInterBankApp.Application.Request;
using BarclaysInterBankApp.Application.Response;
using BarclaysInterBankApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarclaysInterBankApp.Application.Contract
{
     public interface IAccountRepository
    {
        Task<AccountCreateResponse> CreateAccount(AccountCreateRequest accountRequest);
        Task<PinChangeResponse> ChangePin(ChangePinRequest changePinRequest, string newPin);
    }
}
