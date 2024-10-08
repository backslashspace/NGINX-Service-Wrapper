using BSS.Logging;
using System;
using System.Diagnostics;
using System.IO;

#pragma warning disable CS0618

namespace Wrapper
{
    internal static partial class Worker
    {
        internal static void Run()
        {
            String nginxPath = Path.Combine(_assemblyPath, "nginx.exe");

            if (!File.Exists(nginxPath))
            {
                Log.FastLog("Failed to start nginx, file not found: \n" + nginxPath, LogSeverity.Error, "Worker");
                Environment.Exit(-1);
            }

            Process nginx = new();
            nginx.StartInfo.FileName = nginxPath;
            nginx.StartInfo.WorkingDirectory = _assemblyPath;
            nginx.StartInfo.CreateNoWindow = true;
            nginx.StartInfo.UseShellExecute = false;

            try
            {
                nginx.Start();
                Log.FastLog("Started nginx", LogSeverity.Info, "Worker");
                nginx.Dispose();
            }
            catch (Exception exception)
            {
                Log.FastLog("Failed to start nginx:\n" + exception.Message, LogSeverity.Error, "Worker");
                Environment.Exit(-1);
            }

            Log.FastLog("Running GC and entering sleep (suspending thread)", LogSeverity.Info, "Worker");
            GC.Collect(5, GCCollectionMode.Forced, true, true);
            Service.MainThread.Suspend();
            Log.FastLog("Woke up from sleep (resumed thread)", LogSeverity.Info, "Worker");

            if (!File.Exists(nginxPath))
            {
                Log.FastLog("Failed to start nginx, file not found: \n" + nginxPath, LogSeverity.Error, "Worker");
                Environment.Exit(-1);
            }

            nginx = new();
            nginx.StartInfo.FileName = nginxPath;
            nginx.StartInfo.Arguments = "-s quit";
            nginx.StartInfo.WorkingDirectory = _assemblyPath;
            nginx.StartInfo.CreateNoWindow = true;
            nginx.StartInfo.UseShellExecute = false;
            nginx.StartInfo.RedirectStandardError = true;

            try
            {
                nginx.Start();
                Log.FastLog($"Send quit signal to nginx", LogSeverity.Info, "Worker");

                String errOut = nginx.StandardError.ReadToEnd();

                nginx.WaitForExit();
                nginx.Dispose();

                if (errOut != "")
                {
                    Log.FastLog($"An error occurred trying signal a quit to nginx, error was:'\n{errOut}", LogSeverity.Error, "Worker");
                    Environment.Exit(-1);
                }
            }
            catch (Exception exception)
            {
                Log.FastLog($"An error occurred trying to start nginx -s quit'\n{exception.Message}", LogSeverity.Error, "Worker");
                nginx.Dispose();
                Environment.Exit(-1);
            }

            Log.FastLog($"Successfully signaled quit to nginx", LogSeverity.Info, "Worker");
        }
    }
}