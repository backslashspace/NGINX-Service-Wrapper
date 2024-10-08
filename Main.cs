using BSS.Logging;
using System;
using System.ServiceProcess;

#pragma warning disable IDE0079
#pragma warning disable CS0618
#pragma warning disable CS8618

namespace Wrapper
{
    internal static partial class Worker
    {
        internal static String _assemblyPath;

        private static void Main(String[] args)
        {
            _assemblyPath = Log.Initialize();

#if DEBUG
            Log.FastLog("Starting in debug mode", LogSeverity.Debug, "Main");
            Console.CancelKeyPress += (s, e) =>
            {
                e.Cancel = true;
                Service.MainThread.Resume();
                Service.MainThread.Join();
                Environment.Exit(0);
            };
            Log.FastLog("Starting nginx", LogSeverity.Debug, "Main");
            Service.MainThread.Start();
#else
            Log.FastLog("Starting service base", LogSeverity.Info, "Main");
            ServiceBase.Run(new Service());
#endif
        }
    }
}