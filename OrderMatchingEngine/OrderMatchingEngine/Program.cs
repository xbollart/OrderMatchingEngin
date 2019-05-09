using System;
using System.Threading;

namespace OrderMatchingEngine
{
    class Program
    {
        static void Main(string[] args)
        {   
            Engine btcusdEngine = new Engine();

            if (btcusdEngine.Init(1,100,1000))
            {
                Thread btcusdEngineThread = new Thread(() => btcusdEngine.Start());
                btcusdEngineThread.Start();
            }
        }
    }
}