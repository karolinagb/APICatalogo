using APICatalogo.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace APICatalogo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public ActionResult<string> Get()
        {
            return "AccountController :: Acessado em  : " + DateTime.Now.ToLongDateString();
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] UserViewModel userViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.Values.SelectMany(x => x.Errors));
            }

            var user = new IdentityUser
            {
                UserName = userViewModel.Email,
                Email = userViewModel.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, userViewModel.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            await _signInManager.SignInAsync(user, false);

            return Ok();
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] UserViewModel userViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.Values.SelectMany(x => x.Errors));
            }

            //Verifica as credenciais do usuário e retorna um valor:
            var result = await _signInManager.PasswordSignInAsync(userViewModel.Email, userViewModel.Password,
                isPersistent: false, lockoutOnFailure: false);
            //lockoutOnFailure: false = se tentar mais de 3 vezes não vou bloquear


            if (result.Succeeded)
            {
                return Ok();
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Login Inválido...");
                return BadRequest(ModelState);
            }
        }
    }
}
