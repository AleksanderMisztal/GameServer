using System;
using System.Collections.Generic;

namespace GameServer.Utils
{
    public static class ThreadManager
    {
        private static readonly List<Action> ToExecuteOnMainThread = new List<Action>();
        private static readonly List<Action> ExecuteCopiedOnMainThread = new List<Action>();
        private static bool _actionToExecuteOnMainThread;

        public static void ExecuteOnMainThread(Action action)
        {
            if (action == null)
            {
                return;
            }

            lock (ToExecuteOnMainThread)
            {
                ToExecuteOnMainThread.Add(action);
                _actionToExecuteOnMainThread = true;
            }
        }

        public static void UpdateMain()
        {
            if (!_actionToExecuteOnMainThread) return;
            ExecuteCopiedOnMainThread.Clear();
            lock (ToExecuteOnMainThread)
            {
                ExecuteCopiedOnMainThread.AddRange(ToExecuteOnMainThread);
                ToExecuteOnMainThread.Clear();
                _actionToExecuteOnMainThread = false;
            }

            foreach (Action t in ExecuteCopiedOnMainThread) t();
        }
    }
}
