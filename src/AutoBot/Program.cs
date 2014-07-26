using AutoBot.Core.Engine;
using Castle.Windsor;
using Castle.Windsor.Installer;
using log4net;
using System;
using System.ServiceProcess;
using System.Threading;

namespace AutoBot
{

    internal sealed class Program
    {

        private static readonly ILog Logger = LogManager.GetLogger(typeof(Program));
        
        private static void Main(string[] args)
        {
            if (args.Length > 0 && args[0].Equals("service", StringComparison.CurrentCultureIgnoreCase))
            {
                ServiceBase.Run(new ServiceBase[] { new Service() });
            }
            else
            {
                Logger.Info("Starting Autobot in console mode");
                try
                {
                    using (var container = new WindsorContainer())
                    {
                        container.Install(Configuration.FromAppConfig());
                        var botEngine = container.Resolve<AutoBotEngine>();
                        botEngine.Start();
                        // "start" is synchronous so we'll use a manual reset event
                        // to pause this thread forever. client events will continue to
                        // fire but we won't have to worry about setting up an idle "while" loop.
                        var waiter = new ManualResetEvent(false);
                        waiter.WaitOne();
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error("ERROR!:", ex);
                }
            }

        }

     }

}
