using Microsoft.AspNet.SignalR;

namespace InventorySystem
{
    public static class Notification
    {
        public static void SendMessage(string message)
        {
            GlobalHost.ConnectionManager.GetHubContext<NotificationHub>().Clients.All.sendMessage(message);
        }
    }
}