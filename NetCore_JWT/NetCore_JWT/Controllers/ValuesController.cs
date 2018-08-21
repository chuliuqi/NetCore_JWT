using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace NetCore_JWT.Controllers
{
    [Route("api/[controller]/[action]")]
    public class ValuesController : Controller
    {
        private JwtSettings setting;
        public ValuesController(IOptions<JwtSettings> options)
        {
            setting = options.Value;
        }

        [HttpPost]
         public IActionResult Login([FromBody]LoginViewModel login)
         {
             if (ModelState.IsValid)
             {
                 if (login.UserName == "wangshibang" && login.Password == "123456")
                 {
                     var claims = new Claim[] {
                         new Claim(ClaimTypes.Name, login.UserName),
                         new Claim(ClaimTypes.Role, "admin, Manage")
                     };
                     var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(setting.SecretKey));
                     var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                     var token = new JwtSecurityToken(
                         setting.Issuer,
                         setting.Audience,
                         claims,
                         DateTime.Now,
                         DateTime.Now.AddMinutes(30),
                         creds);
                     return Ok(new { Token = new JwtSecurityTokenHandler().WriteToken(token) });
                 }
             }
             return BadRequest();
         }
 
         [HttpGet]
         public IActionResult NoValidate()
         {
             return Ok();
        }

         // GET api/values
         [Authorize(Roles = "Manage")]
         [HttpGet]
         public IEnumerable<string> Get()
         {
             return new string[] { "value1", "value2" };
         }
      
    }
}
