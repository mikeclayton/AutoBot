using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Host;
using System.Text;
using log4net;

namespace AutoBot.Host
{

    internal class AutoBotUserInterface: PSHostUserInterface
    {

        #region Fields

        private PSHostRawUserInterface m_RawUI;

        #endregion

        #region Events

        public delegate void WriteHandler(object sender, string value);
        public event WriteHandler OnWrite;

        #endregion

        #region Constructors

        public AutoBotUserInterface(ILog logger)
        {
            this.Logger = logger;
        }

        #endregion

        #region Properties

        private ILog Logger
        {
            get;
            set;
        }

        #endregion

        #region PSHostUserInterface Members

        #region Input Methods

        // it's a bot - we don't support input

        public override Dictionary<string, PSObject> Prompt(string caption, string message, System.Collections.ObjectModel.Collection<FieldDescription> descriptions)
        {
            throw new NotImplementedException();
        }

        public override int PromptForChoice(string caption, string message, System.Collections.ObjectModel.Collection<ChoiceDescription> choices, int defaultChoice)
        {
            throw new NotImplementedException();
        }

        public override PSCredential PromptForCredential(string caption, string message, string userName, string targetName, PSCredentialTypes allowedCredentialTypes, PSCredentialUIOptions options)
        {
            throw new NotImplementedException();
        }

        public override PSCredential PromptForCredential(string caption, string message, string userName, string targetName)
        {
            throw new NotImplementedException();
        }

        public override PSHostRawUserInterface RawUI
        {
            get
            {
                if (m_RawUI == null)
                {
                    m_RawUI = new AutoBotRawUserInterface();
                }
                return m_RawUI;
            }
        }

        public override string ReadLine()
        {
            throw new NotImplementedException();
        }

        public override System.Security.SecureString ReadLineAsSecureString()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Output Methods

        public override void Write(ConsoleColor foregroundColor, ConsoleColor backgroundColor, string value)
        {
            this.Logger.Info(value);
            this.OnWrite(this, value);
        }

        public override void Write(string value)
        {
            this.Write(this.RawUI.ForegroundColor, this.RawUI.BackgroundColor, value);
        }

        public override void WriteLine(string value)
        {
            this.Write(value);
        }

        public override void WriteProgress(long sourceId, ProgressRecord record)
        {
            this.Write(record.PercentComplete.ToString());
        }

        #endregion

        #region Logging Methods

        public override void WriteDebugLine(string message)
        {
            this.Logger.Debug(message);
        }

        public override void WriteErrorLine(string value)
        {
            this.Logger.Error(value);
        }

        public override void WriteVerboseLine(string message)
        {
            this.Logger.Info(message);
        }

        public override void WriteWarningLine(string message)
        {
            this.Logger.Warn(message);
        }

        #endregion

        #endregion

    }
        
}
