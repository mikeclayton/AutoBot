using System.ServiceProcess;
using AutoBot.Core.Engine;
using Castle.Windsor;
using Castle.Windsor.Installer;
using log4net;

namespace AutoBot
{
    partial class Service : ServiceBase
    {

        private static readonly ILog Logger = LogManager.GetLogger(typeof(Service));
        private AutoBotEngine _botEngine;

        public Service()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Logger.Info("Starting AutoBot Windows service");
            using (var container = new WindsorContainer())
            {
                container.Install(Configuration.FromAppConfig());
                _botEngine = container.Resolve<AutoBotEngine>();
                _botEngine.Start();
            }
        }


        protected override void OnStop()
        {
            Logger.Info("Stopping AutoBot Windows service");
            _botEngine.Stop();
        }
    }
}
