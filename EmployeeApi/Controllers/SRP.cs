using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SRP : ControllerBase
    {
        public readonly IUserService _userService;
        public readonly IEmailService _emailService;
        public SRP(IUserService userService,IEmailService emailService)
        {
            _userService = userService;
            _emailService = emailService;
        }
        [HttpGet("{id}")]
        public IActionResult Get(int id) 
        { 
            var user=_userService.GetUser(id);
            _emailService.SendEmail("","","");
            return Ok(user);
        }
    }
    public interface IUserService
    {
        int GetUser(int id);
    }
    public interface IEmailService
    {
        void SendEmail(string to, string subject, string body);
    }
    public class UserService : IUserService
    {
        public int GetUser(int id)
        {
            return 0;
        }
    }
    public class EmailService : IEmailService
    {
        public void SendEmail(string to, string subject, string body)
        {
        }
    }

    
}
