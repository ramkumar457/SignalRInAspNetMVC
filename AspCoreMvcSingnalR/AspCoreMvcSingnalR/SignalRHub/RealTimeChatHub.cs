using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.SignalR;
using System.Diagnostics;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Reflection.Metadata;
using System.Xml.Linq;
using System.Xml;
using AspCoreMvcSingnalR.DatabaseEntity;
using Microsoft.EntityFrameworkCore;

namespace AspCoreMvcSingnalR.SignalRHub
{
    public class RealTimeChatHub:Hub
    {
        public override Task OnDisconnectedAsync(Exception exception)
        {
            Debug.WriteLine("Client disconnected: " + Context.ConnectionId);
            return base.OnDisconnectedAsync(exception);
        }
        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }
        //Create a group for each user to chat separately for private conversations.
        public void CreateUserChatGroup(string userId)
        {
            var id = Context.ConnectionId;
            Groups.AddToGroupAsync(Context.ConnectionId, userId);
            //Adds the client associated with the current connection ID to a specified group.In this case, we are taking `userId` as a parameter for this function,
            //Where `userId` represents the name of the group for loggedin user.
            //For each user in our database, we create a unique group with ConnectionId
            //This function is used to add a client to a specific chat group, enabling them to participate in conversations within that group. 

            //We have written this logic, to  ensuring that each user only gets messages for their group
            //identified by `userId`. that means logged-in users will only receive messages
            //sent to their groups, enhancing privacy and ensuring that users do not receive messages for other users to make sure private chat.
        }
        //Send message to user Group
        public async Task SendMessageToUserGroup(string senderUserId, string senderName, string receiverUserId, string message)
        {
            //Insert message to database then send it to the Client
            var optionsBuilder = new DbContextOptionsBuilder<ChatDbContext>();
            var _chatDbContext = new ChatDbContext();
            UserChatHistory chatHistory = new UserChatHistory();
            chatHistory.Message = message;
            chatHistory.CreatedAt = DateTime.UtcNow;
            chatHistory.SenderUserId = new Guid(senderUserId);
            chatHistory.ReceiverUserId = new Guid(receiverUserId);
            await _chatDbContext.UserChatHistory.AddAsync(chatHistory);
            await _chatDbContext.SaveChangesAsync();
            await Clients.Group(receiverUserId).SendAsync("ReceiveMessage", senderUserId, senderName, message);
            //"Send the message to all users in the specified group. We take the sender's user ID and the receiver's user ID, and then send the message to
            //the group of the receiver's user ID to ensure that only users within the specified receiver group receive the message.
            //This allows for private communication between users in a one-to-one chat system."
        }
    }
}
