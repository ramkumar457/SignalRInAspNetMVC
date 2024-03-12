using AspCoreMvcSingnalR.DatabaseEntity;
using AspCoreMvcSingnalR.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace AspCoreMvcSingnalR.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        ChatDbContext _chatDbContext;
        public HomeController(ILogger<HomeController> logger, ChatDbContext chatDbContext)
        {
            _logger = logger;
            _chatDbContext = chatDbContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        //Here write your own code for login 
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            // Check if username and password are correct
            var user = _chatDbContext.Users.FirstOrDefault(a => a.Email == model.Email && a.Password == model.Password);
            if (user != null)
            {
                // Redirect to chat page 
                var claims = new List<Claim>() {
                                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                                    new Claim(ClaimTypes.Name,user.FullName)
                                    };
                //Initialize a new instance of the ClaimsIdentity with the claims and authentication scheme    
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                //Initialize a new instance of the ClaimsPrincipal with ClaimsIdentity    
                var principal = new ClaimsPrincipal(identity);

                //SignInAsync is a Extension method for Sign in a principal for the specified scheme.    
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties()
                {
                    IsPersistent = false
                });
                return RedirectToAction("Chat", "UserChat");
            }
            else
            {
                // If login fails, return to the login page with an error message
                ModelState.AddModelError(string.Empty, "Invalid username or password");
                return View("Index",model);
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}