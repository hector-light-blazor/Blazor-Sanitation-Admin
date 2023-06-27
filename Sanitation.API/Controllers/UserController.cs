using Microsoft.AspNetCore.Mvc;
using SanitationPortal.Models.DTOs;
using SanitationPortal.Models.Requests;
using SanitationPortal.Service.Services.Interfaces;

namespace Sanitation.API.Controllers
{

   


    [ApiController]
    [Route("/api/v1/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IAccountServices _accountService;
        public UserController(IAccountServices accountService)
        {
                _accountService = accountService;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> RegisterAsync(UserRegisterRequest request)
        {
            try
            {
                var response = await _accountService.RegisterAccount(request);

                return Ok(response);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }


        [HttpPost("Login")]
        public async Task<IActionResult> LoginAsync(UserLoginRequest account)
        {
            try
            {
                var response = await _accountService.Login(account);

                return Ok(response);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
    }
}
