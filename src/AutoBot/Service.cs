using System;
using System.ServiceProcess;
using log4net;
using AutoBot.Core.Chat;
using AutoBot.Core.Engine;
using Castle.Windsor;
using Castle.Windsor.Installer;

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
            using (var container = new WindsorContainer())
            {
                container.Install(Configuration.FromAppConfig());
                _botEngine = container.Resolve<BotEngine>();
                _botEngine.Connect();
            }
        }


        protected override void OnStop()
        {
            Logger.Info("Stopping AutoBot Windows service");

            _botEngine.Disconnect();
        }
    }
}
