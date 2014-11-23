using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logging
{
    public interface ILogHandler
    {
        void HandleMessage(string tag, string message);
    }

    public static class Log
    {
        private static List<ILogHandler> mHandlers = new List<ILogHandler>();

        public static void RegisterHandler(ILogHandler handler)
        {
            if (!mHandlers.Contains(handler))
                mHandlers.Add(handler);
        }

        public static void UnregisterHandler(ILogHandler handler)
        {
            mHandlers.Remove(handler);
        }

        public static void LogMessage(string tag, string message, params object[] args)
        {
            for (int i = 0; i < mHandlers.Count; ++i)
                mHandlers[i].HandleMessage(tag, String.Format(message, args));
        }
    }
}
