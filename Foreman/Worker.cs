using AutoUpdateLibrary;
using System.ComponentModel.Composition;

namespace Foreman
{
    public class Worker
    {
        [Import(typeof(IDoWork))]
        public IDoWork CurrentWorker { get; set; }
    }
}
