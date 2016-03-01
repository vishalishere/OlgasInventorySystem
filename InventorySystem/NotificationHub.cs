using Microsoft.AspNet.SignalR;

namespace InventorySystem
{
    public class NotificationHub : Hub
    {
        public string Activate()
        {
            return "Monitor Activated";
        }
    }
}