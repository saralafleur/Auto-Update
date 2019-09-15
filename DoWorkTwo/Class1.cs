using AutoUpdateLibrary;
using System;
using System.ComponentModel.Composition;

namespace DoWorkOne
{
    [Export(typeof(IDoWork))]
    public class Class1 : IDoWork
    {
        public void DoThat(string one)
        {
            Console.WriteLine(string.Format("2: Do This {0}", one));
        }

        public void DoThis(string one)
        {
            Console.WriteLine(string.Format("2: Do That {0}", one));
        }
    }
}
