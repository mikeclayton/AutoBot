using System;
using System.Management.Automation.Host;
using System.Threading;
using Castle.Core.Logging;

namespace AutoBot.Agents.PowerShell
{

    /// <summary>
    /// Contains the functionality for creating a custom host. A host provides
    /// communications between the Windows PowerShell engine and the user.
    /// </summary>
    /// <remarks>
    /// See http://msdn.microsoft.com/en-gb/library/system.management.automation.host.pshost%28v=vs.85%29.aspx
    /// See http://msdn.microsoft.com/en-gb/library/windows/desktop/ee706559%28v=vs.85%29.aspx
    /// </remarks>
    internal sealed class Host : PSHost
    {

        #region Fields

        private Guid m_InstanceId;
        private PSHostUserInterface m_UI;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the Host class.
        /// </summary>
        /// <param name="logger"></param>
        public Host(ILogger logger)
            : base()
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

        #region PSHost Members

        /// <summary>
        /// Gets the culture that the runspace uses to set the current culture on new threads.
        /// </summary>
        public override System.Globalization.CultureInfo CurrentCulture
        {
            get
            {
                return Thread.CurrentThread.CurrentCulture;
            }
        }

        /// <summary>
        /// Gets the UI culture that the runspace and cmdlets use to load resources.
        /// </summary>
        public override System.Globalization.CultureInfo CurrentUICulture
        {
            get
            {
                return Thread.CurrentThread.CurrentUICulture;
            }
        }

        /// <summary>
        /// Instructs the host to interrupt the currently running pipeline and start a new nested input loop.
        /// </summary>
        public override void EnterNestedPrompt()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Instructs the host to exit the currently running input loop.
        /// </summary>
        public override void ExitNestedPrompt()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the identifier that uniquely identifies this instance of the host.
        /// </summary>
        public override Guid InstanceId
        {
            get
            {
                if (m_InstanceId == Guid.Empty)
                {
                    m_InstanceId = Guid.NewGuid();
                }
                return m_InstanceId;
            }
        }

        /// <summary>
        /// Gets the user friendly name of the host.
        /// </summary>
        public override string Name
        {
            get
            {
                return "AutoBotHost";
            }
        }

        /// <summary>
        /// Notifies the host that the Windows PowerShell runtime is about to execute a legacy
        /// command-line application. A legacy application is defined as a console-mode
        /// executable that can perform any of the following operations: read from stdin,
        /// write to stdout, write to stderr, or use any of the Windows console functions.
        /// </summary>
        public override void NotifyBeginApplication()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Notifies the host that the Windows PowerShell engine has completed the execution
        /// of a legacy command. A legacy application is defined as a console-mode executable
        /// that can perform any of the following operations: read from stdin, write to stdout,
        /// write to stderr, or use any of the Windows console functions.
        /// </summary>
        public override void NotifyEndApplication()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Requests to end the current runspace. The Windows PowerShell engine calls this
        /// method to request that the host application shut down and terminate the host
        /// root runspace.
        /// </summary>
        /// <param name="exitCode"></param>
        public override void SetShouldExit(int exitCode)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the implementation of the PSHostUserInterface class that defines user
        /// interaction for this host.
        /// </summary>
        public override PSHostUserInterface UI
        {
            get
            {
                if (m_UI == null)
                {
                    m_UI = new UserInterface(this.Logger);
                }
                return m_UI;
            }
        }

        /// <summary>
        /// Gets the version number of the host.
        /// </summary>
        public override Version Version
        {
            get
            {
                return new Version(1, 0);
            }
        }

        #endregion

    }

}
