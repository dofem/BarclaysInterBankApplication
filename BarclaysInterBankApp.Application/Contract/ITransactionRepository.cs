using BarclaysInterBankApp.Application.Request;
using BarclaysInterBankApp.Application.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarclaysInterBankApp.Application.Contract
{
    public interface ITransactionRepository
    {
        Task<TopUpResponse> PerformTopUp(TopUpRequest topUp,decimal amount);
    }
}
