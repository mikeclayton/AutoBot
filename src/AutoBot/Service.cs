using System;
using System.ServiceProcess;
using log4net;
using AutoBot.Core.Chat;
using AutoBot.Core.Engine;
using AutoBot.ChatClients.HipChat;

namespace AutoBot
{
    partial class Service : ServiceBase
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(Service));

        public Service()
        {
            InitializeComponent();
        }

        private BotEngine _botEngine;

        protected override void OnStart(string[] args)
        {
            Logger.Info("Starting AutoBot Windows service");

            IChatSession session = new HipChatSession(LogManager.GetLogger(typeof(HipChatSession)));
            BotEngine botEngine = new BotEngine(session);
            session.MessageReceived += botEngine.ProcessMessage;
            _botEngine.Connect();
            _botEngine.Connect();
        }


        protected override void OnStop()
        {
            Logger.Info("Stopping AutoBot Windows service");

            _botEngine.Disconnect();
        }
    }
}
