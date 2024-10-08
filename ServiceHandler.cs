using BSS.Logging;
using System;
using System.ServiceProcess;
using System.Threading;

#pragma warning disable CS0618

namespace Wrapper
{
    internal sealed class Service : ServiceBase
    {
        internal static readonly Thread MainThread = new(Worker.Run) { Name = "MainWorkerThread" };

        public Service()
        {
            AutoLog = false;
            CanHandlePowerEvent = false;
            CanPauseAndContinue = false;
            CanShutdown = true;
            CanStop = true;
            ServiceName = "Nginx Wrapper";
        }

        protected override void OnStart(String[] args) => MainThread.Start();

        protected override void OnStop() => Shutdown();

        protected override void OnShutdown() => Shutdown();

        internal static void Shutdown()
        {
            Log.FastLog("Received Win32 shutdown signal", LogSeverity.Info, "ServiceBase");

            MainThread.Resume();
            MainThread.Join();

            Log.FastLog("Successful shut down, exiting", LogSeverity.Info, "ServiceBase");
        }
    }
}