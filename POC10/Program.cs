using System;
using System.ComponentModel;
using System.IO;
using System.Threading;

namespace POC10
{
    class Program
    {
        static AppDomain _domain;
        static BackgroundWorker _backgroundWorker;
        static void Main(string[] args)
        {
            if (!Directory.Exists(Environment.CurrentDirectory + "\\updates"))
                Directory.CreateDirectory(Environment.CurrentDirectory + "\\updates");
            FileSystemWatcher watcher = new FileSystemWatcher(Environment.CurrentDirectory + "\\updates", "*.dll");

            //Copy the inital worker into the correct location
            InitializeWorker();

            //Build a worker thread to host the Foreman task in the alternate domain
            _backgroundWorker = new BackgroundWorker();
            _backgroundWorker.WorkerSupportsCancellation = true;
            _backgroundWorker.DoWork += new DoWorkEventHandler(backgroundWorker_DoWork);
            _backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorker_RunWorkerCompleted);

            while (true)
            {
                //start it working
                _backgroundWorker.RunWorkerAsync();

                watcher.WaitForChanged(WatcherChangeTypes.Created);

                ChangeWorker();
                //stop it
                _backgroundWorker.CancelAsync();
                //wait for the background thread to finish
                while (_backgroundWorker.IsBusy)
                {
                    Thread.Sleep(new TimeSpan(0, 0, 0, 0, 100));
                }
            }
        }

        static void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // Unload the application domain.
            AppDomain.Unload(_domain);
        }

        static void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            //Set up your new app domain, setting ShadowCopyFiles to true so that you can replace
            // the worker while running
            AppDomainSetup domaininfo = new AppDomainSetup();
            domaininfo.ApplicationBase = Environment.CurrentDirectory;
            domaininfo.ShadowCopyFiles = "true";

            // Create the application domain.
            _domain = AppDomain.CreateDomain("MyDomain", null, domaininfo);

            while (true)
            {
                //Get your foreman running
                _domain.ExecuteAssembly("Foreman.exe");
                //take a short nap
                Thread.Sleep(new TimeSpan(0, 0, 1));
                //check to see if there is a pending cancelation of this thread, and stop if there is
                if (_backgroundWorker.CancellationPending)
                    break;
            }
        }

        private static void InitializeWorker()
        {
            File.Copy("DoWorkOne.dll", "DoWork.dll", true);
        }

        private static void ChangeWorker()
        {
            string newWorker = Directory.GetFiles(Environment.CurrentDirectory + "\\updates", "*.dll")[0];
            //wait until the file has fully been stored on the disk
            while (true)
            {
                try
                {
                    using (File.OpenWrite(newWorker)) { }
                }
                catch
                {
                    continue;
                }
                break;
            }

            //copy and clear the update
            File.Copy(newWorker, "DoWork.dll", true);
            File.Delete(newWorker);
        }
    }
}
