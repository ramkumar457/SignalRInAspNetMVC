using AspCoreMvcSingnalR.DatabaseEntity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AspCoreMvcSingnalR.Controllers
{
    [Authorize]
    public class UserChatController : Controller
    {

        ChatDbContext _chatDbContext;
        public UserChatController(ChatDbContext chatDbContext)
        {
            _chatDbContext = chatDbContext;
        }
        public IActionResult Chat()
        {
            UserChatViewModel userChatViewModel = new UserChatViewModel();

            //Get logged in user detail from Claim
            Guid userId = new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier));
            string name = User.FindFirstValue(ClaimTypes.Name);

            userChatViewModel.LoggedInUser = new User { UserId = userId, FullName = name };

            //Get usera for chat exclude logged In user from the list
            userChatViewModel.Users = _chatDbContext.Users.Where(a => a.UserId != userId).ToList();
            return View(userChatViewModel);
        }
        public ActionResult GetChatCobversion(Guid userIdToLoadChat)
        {
            Guid loginUserId = new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var chatHistories = _chatDbContext.UserChatHistory.Include("SenderUser")
                    .Include("ReceiverUser").Where(a => (a.ReceiverUserId == loginUserId && a.SenderUserId == userIdToLoadChat)
                   || (a.ReceiverUserId == userIdToLoadChat && a.SenderUserId == loginUserId)).OrderByDescending(a => a.CreatedAt).ToList();
            ViewData["loginUserId"] = loginUserId;
            return PartialView("_ChatConversion", chatHistories);
        }
    }
    public class UserChatViewModel
    {
        public User LoggedInUser { get; set; }
        public List<User> Users { get; set; }// Users avialable for Chat
    }
}
