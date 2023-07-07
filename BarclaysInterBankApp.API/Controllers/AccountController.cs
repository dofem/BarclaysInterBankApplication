using BarclaysInterBankApp.Application.Contract;
using BarclaysInterBankApp.Application.Request;
using BarclaysInterBankApp.Application.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BarclaysInterBankApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountRepository _accountRepository;

        public AccountController(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        [HttpPost]
        [Route("CreateNewAccount")]
        [ProducesResponseType(StatusCodes.Status201Created)] // Success
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // Failure
        [ProducesResponseType(StatusCodes.Status500InternalServerError)] // Exception
        public async Task<IActionResult> CreateNewAccount([FromBody]AccountCreateRequest request)
        {
            var response =await _accountRepository.CreateAccount(request);
            if (response != null)
            {
                if (response.IsSuccess)
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
                return StatusCode(500, "Account Creation Failed");
            }
        }



        [HttpPost]
        [Route("ChangeYourPin")]
        [ProducesResponseType(StatusCodes.Status201Created)] // Success
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // Failure
        [ProducesResponseType(StatusCodes.Status500InternalServerError)] // Exception
        public async Task<IActionResult> ChangePin([FromBody] ChangePinRequest changePinRequest, string newPin)
        {
            var response = await _accountRepository.ChangePin(changePinRequest, newPin);
            if (response != null)
            {
                if (response.IsSuccess)
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
                return StatusCode(500, "Pin Update Failed");
            }
        }
    }
}
