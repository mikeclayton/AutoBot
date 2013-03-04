using System;
using System.Threading;
using AutoBot.Core.Chat;
using Castle.Core.Logging;

namespace AutoBot.Core.Engine
{

    public sealed class BotEngine : MarshalByRefObject
    {

        #region Constructors

        public BotEngine(ILogger logger, IChatSession session)
        {
            this.Logger = logger;
            this.Session = session;
            this.Thread = new Thread(delegate()
                                     {
                                         while (this.IsRunning)
                                         {
                                             //TODO Add background task implementation here
                                             Thread.Sleep(1000);
                                         }
                                         Thread.CurrentThread.Abort();
                                     }
                                );
        }

        #endregion

        #region Properties

        private ILogger Logger
        {
            get;
            set;
        }

        private IChatSession Session
        {
            get;
            set;
        }

        private Thread Thread
        {
            get;
            set;
        }

        public bool IsRunning
        {
            get;
            private set;
        }

        #endregion

        #region Status Methods

        public void Start()
        {
            this.Session.Connect();
            this.Session.MessageReceived += this.Session_OnMessageReceived;
            this.IsRunning = true;
            this.Thread.Start();
        }

        public void Stop()
        {
            this.Session.MessageReceived -= this.Session_OnMessageReceived;
            this.Session.Disconnect();
            this.IsRunning = false;
            this.Thread.Join(5000);
        }

        #endregion

        #region Event Handlers

        public void Session_OnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            // execute the command
            var runner = new PowerShellRunner(this.Logger, e.Message, e.Response);
            runner.Execute();
        }

        #endregion

    }

}
