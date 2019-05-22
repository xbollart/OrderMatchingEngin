using System;
using System.Threading;
using Serilog;

namespace OrderMatchingEngine
{
    class Program
    {
        static void Main(string[] args)
        {   
            var logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File("logs\\log.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();        
            
            Engine btcusdEngine = new Engine(logger);

            if (btcusdEngine.Init(1,100,1000))
            {
                Thread btcusdEngineThread = new Thread(() => btcusdEngine.Start());
                btcusdEngineThread.Start();
            }
        }
    }
}