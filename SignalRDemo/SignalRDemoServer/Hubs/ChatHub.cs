using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRDemoServer.Hubs
{
    public class ChatHub : Hub
    {
        /// <summary>
        /// 进入
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task Login(string name)
        {
            await Clients.AllExcept(Context.ConnectionId)
                .SendAsync("online", $"{name} 进入了群聊！");
        }

        /// <summary>
        /// 离开
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task SignOut(string name)
        {
            await Clients.AllExcept(Context.ConnectionId)
                .SendAsync("online", $"{name} 离开了群聊！");
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="user"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("message", user, message);
        }

        /// <summary>
        /// 发送消息（系统通知）
        /// </summary>
        /// <param name="user"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task SendMessageByServer(string user, string message)
        {
            await Clients.All.SendAsync("message", user, message + "(系统通知)");
        }
    }
}
