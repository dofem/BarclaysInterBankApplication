using BarclaysInterBankApp.Application.Contract;
using BarclaysInterBankApp.Application.Implementation;
using BarclaysInterBankApp.Application.Request;
using BarclaysInterBankApp.Domain.Models.Paystack;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BarclaysInterBankApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransferController : ControllerBase
    {
        private readonly ITransferRepository _transferRepository;

        public TransferController(ITransferRepository transferRepository)
        {
            _transferRepository = transferRepository;
        }


        [HttpPost]
        [Route("TransferToOtherBank")]
        [ProducesResponseType(StatusCodes.Status201Created)] // Success
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // Failure
        [ProducesResponseType(StatusCodes.Status500InternalServerError)] // Exception
        public async Task<IActionResult> TransferToOtherBank([FromBody] PaystackTransferModel paystackTransfer, string PinHash)
        {
            var response = await _transferRepository.MakeTransfer(paystackTransfer,PinHash);
            if (response != null)
            {
                if (response.Status)
                {
                    return StatusCode(201, response.Message);
                }
                else
                {
                    return BadRequest(response.Message);
                }
            }
            else
            {
                return StatusCode(500, "Transfer To Other Bank Failed,Try Again Later!!!!");
            }
        }

    }
}
