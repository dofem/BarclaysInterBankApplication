using BarclaysInterBankApp.Application.Contract;
using BarclaysInterBankApp.Application.Request;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BarclaysInterBankApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionRepository _transactionRepository;

        public TransactionController(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }



        [HttpPost]
        [Route("TopUpAccountBalance")]
        [ProducesResponseType(StatusCodes.Status201Created)] // Success
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // Failure
        [ProducesResponseType(StatusCodes.Status500InternalServerError)] // Exception
        public async Task<IActionResult> TopUpYourBalance([FromBody] TopUpRequest topUp, decimal amount)
        {
            var response =await _transactionRepository.PerformTopUp(topUp, amount);
            if(response != null)
            { 
                if(response.IsSuccess)
                {
                    return StatusCode(201,response.Message);
                }
                else
                { 
                    return BadRequest(response.Message);
                }
            }
            else
            { 
               return StatusCode(500, "Balance Top Up Failed,Try Again Later!!!!");   
            }
        }
    }
}
