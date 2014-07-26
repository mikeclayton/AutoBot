using System.Globalization;
using Castle.Core.Logging;
using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Management.Automation.Host;

namespace AutoBot.Agents.PowerShell.Host
{

    /// <summary>
    /// Defines the custom host functionality that is used to perform dialog-oriented
    /// and line-oriented interaction, such as writing to, prompting for, and reading
    /// from user input. 
    /// </summary>
    /// <remarks>
    /// See http://msdn.microsoft.com/en-us/library/windows/desktop/system.management.automation.host.pshostuserinterface%28v=vs.85%29.aspx
    /// </remarks>
    internal sealed class UserInterface : PSHostUserInterface
    {

        #region Fields

        private PSHostRawUserInterface _rawUi;

        #endregion

        #region Events

        public delegate void WriteHandler(object sender, string value);
        public event WriteHandler OnWrite;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the UserInterface class.
        /// </summary>
        /// <param name="logger"></param>
        public UserInterface(ILogger logger)
        {
            this.Logger = logger;
        }

        #endregion

        #region Properties

        private ILogger Logger
        {
            get;
            set;
        }

        #endregion

        #region PSHostUserInterface Members

        #region Input Methods

        // it's a bot - we don't support input

        /// <summary>
        /// Prompts the user for input.
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="message"></param>
        /// <param name="descriptions"></param>
        /// <exception cref="NotImplementedException"></exception>
        /// <returns></returns>
        public override Dictionary<string, PSObject> Prompt(string caption, string message, System.Collections.ObjectModel.Collection<FieldDescription> descriptions)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Provides a set of choices that enable the user to choose a
        /// single option from a set of options.
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="message"></param>
        /// <param name="choices"></param>
        /// <param name="defaultChoice"></param>
        /// <exception cref="NotImplementedException"></exception>
        /// <returns></returns>
        public override int PromptForChoice(string caption, string message, System.Collections.ObjectModel.Collection<ChoiceDescription> choices, int defaultChoice)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Prompts the user for credentials for a specified target. The variants of this
        /// method can be used to prompt the user with a specified prompt window caption,
        /// prompt message, user and target name, credential types allowed to be returned,
        /// and user interface (UI) behavior options.
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="message"></param>
        /// <param name="userName"></param>
        /// <param name="targetName"></param>
        /// <param name="allowedCredentialTypes"></param>
        /// <param name="options"></param>
        /// <exception cref="NotImplementedException"></exception>
        /// <returns></returns>
        public override PSCredential PromptForCredential(string caption, string message, string userName, string targetName, PSCredentialTypes allowedCredentialTypes, PSCredentialUIOptions options)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Prompts the user for credentials for a specified target. The variants of this
        /// method can be used to prompt the user with a specified prompt window caption,
        /// prompt message, user and target name, credential types allowed to be returned,
        /// and user interface (UI) behavior options.
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="message"></param>
        /// <param name="userName"></param>
        /// <param name="targetName"></param>
        /// <returns></returns>
        public override PSCredential PromptForCredential(string caption, string message, string userName, string targetName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets an instance of the PSHostRawUserInterface interface that is implemented
        /// by the host application. This interface defines the low-level functionality
        /// of the custom host.
        /// </summary>
        public override PSHostRawUserInterface RawUI
        {
            get
            {
                if (_rawUi == null)
                {
                    _rawUi = new RawUserInterface();
                }
                return _rawUi;
            }
        }

        /// <summary>
        /// Reads characters that are entered by the user until a newline (carriage return) 
        /// character is encountered.
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        /// <returns></returns>
        public override string ReadLine()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Reads characters entered by the user until a newline (carriage return) character
        /// is encountered and returns the characters as a secure string.
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        /// <returns></returns>
        public override System.Security.SecureString ReadLineAsSecureString()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Output Methods

        /// <summary>
        /// Writes characters to the output display of the host.
        /// </summary>
        /// <param name="value"></param>
        public override void Write(string value)
        {
            this.Write(this.RawUI.ForegroundColor, this.RawUI.BackgroundColor, value);
        }

        /// <summary>
        /// Writes characters to the output display of the host
        /// with possible foreground and background colors.
        /// </summary>
        /// <param name="foregroundColor"></param>
        /// <param name="backgroundColor"></param>
        /// <param name="value"></param>
        public override void Write(ConsoleColor foregroundColor, ConsoleColor backgroundColor, string value)
        {
            this.Logger.Info(value);
            this.OnWrite(this, value);
        }

        /// <summary>
        /// Writes characters to the output display of the host.
        /// </summary>
        public override void WriteLine()
        {
            this.WriteLine(string.Empty);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public override void WriteLine(string value)
        {
            this.WriteLine(this.RawUI.ForegroundColor, this.RawUI.BackgroundColor, value);
        }

        /// <summary>
        /// Writes characters to the output display of the host
        /// with possible foreground and background colors.
        /// </summary>
        /// <param name="foregroundColor"></param>
        /// <param name="backgroundColor"></param>
        /// <param name="value"></param>
        public override void WriteLine(ConsoleColor foregroundColor, ConsoleColor backgroundColor, string value)
        {
            this.Logger.Info(value);
            this.OnWrite(this, value);
        }

        /// <summary>
        /// Writes a progress report to be displayed to the user.
        /// </summary>
        /// <param name="sourceId"></param>
        /// <param name="record"></param>
        public override void WriteProgress(long sourceId, ProgressRecord record)
        {
            this.Write(record.PercentComplete.ToString(CultureInfo.InvariantCulture));
        }

        #endregion

        #region Logging Methods

        /// <summary>
        /// Displays a debug message to the user.
        /// </summary>
        /// <param name="message"></param>
        public override void WriteDebugLine(string message)
        {
            this.Logger.Debug(message);
        }

        /// <summary>
        /// Writes a line to the error display of the host.
        /// </summary>
        /// <param name="value"></param>
        public override void WriteErrorLine(string value)
        {
            this.Logger.Error(value);
        }

        /// <summary>
        /// Writes a verbose line to be displayed to the user.
        /// </summary>
        /// <param name="message"></param>
        public override void WriteVerboseLine(string message)
        {
            this.Logger.Info(message);
        }

        /// <summary>
        /// Writes a warning line to be displayed to the user.
        /// </summary>
        /// <param name="message"></param>
        public override void WriteWarningLine(string message)
        {
            this.Logger.Warn(message);
        }

        #endregion

        #endregion

    }

}
