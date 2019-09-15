using AutoUpdateLibrary;
using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;

namespace Foreman
{
    /// 
    /// This is the Foreman class that acts as a proxy for the individual worker DLLs.
    /// 
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("starting");
            IDoWork worker = GetWorker();
            worker.DoThis("hello world");
            worker.DoThat("Hi again");
            Console.WriteLine("finished");
        }
        static IDoWork GetWorker()
        {
            Assembly asm = Assembly.LoadFrom("DoWork.dll");

            var catalog = new AssemblyCatalog(asm);

            var worker = new Worker();
            var container = new CompositionContainer(catalog);

            container.ComposeParts(worker);

            if (worker.CurrentWorker != null)
            {
                return worker.CurrentWorker;
            }
            return null;
        }
    }
}
