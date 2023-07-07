using BarclaysInterBankApp.Application.Response;
using BarclaysInterBankApp.Domain.Models.Paystack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarclaysInterBankApp.Application.Contract
{
    public interface ITransferRepository
    {
        Task<TransferResponse> MakeTransfer(PaystackTransferModel paystackTransfer, string PinHash);
    }
}
