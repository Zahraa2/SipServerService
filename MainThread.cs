using System;
using SIPServer;

namespace SipServer
{
    class MainThread
    {
        public static bool ThreadStarted = false;
        static Thread ProccessThread;
        static readonly object writeLock = new object();

        static MainThread()
        {

        }

        public static Boolean Start()
        {
            lock (writeLock)
            {
                if (ThreadStarted)
                    return true;

                try
                {
                    ProccessThread = new Thread(new ThreadStart(ThreadProc));
                    ProccessThread.Start();
                    ThreadStarted = true;

                }
                catch (ArgumentNullException e)
                {
                    //
                }
            }
            return true;
        }
        public static Boolean End()
        {
            //if (!ThreadStarted)
            //  return true;

            try
            {
                // Create a TcpClient.   
                if (ProccessThread.IsAlive)
                    ProccessThread.Abort();

            }
            catch (ArgumentNullException e)
            {
                //
            }

            return true;
        }

        static async void ThreadProc()
        {

            Thread.Sleep(5 * 1000);
            Server _server = new Server();
            
            _server.Start();

            ThreadsManager.CreateThreads();
            UserReactionThread.Start();

            while (true)
            {
                Thread.Sleep(60 * 1000);
            }
        }

    }
}
