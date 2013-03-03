using System;
using System.ServiceProcess;
using log4net;
using AutoBot.Core.Engine;
using AutoBot.Core.Chat;
using AutoBot.ChatClients.HipChat;

namespace AutoBot
{

    class Program
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
                    IChatSession session = new HipChatSession(LogManager.GetLogger(typeof(HipChatSession)));
                    BotEngine botEngine = new BotEngine(session);
                    session.MessageReceived += botEngine.ProcessMessage;
                    botEngine.Connect();
                }
                catch (Exception ex)
                {
                    Logger.Error("ERROR!:", ex);
                }
            }

        }

     }

}
