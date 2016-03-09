using System;

namespace InventorySystem.Controllers
{
    public class KnownException : Exception
    {
        public string ErrorMessage;
        public Exception InternalException;
        public KnownException(Exception e)
        {
            InternalException = e;
            ErrorMessage = e.Message;
        }
        public KnownException(string m)
        {
            ErrorMessage = m;
        }
    }
}