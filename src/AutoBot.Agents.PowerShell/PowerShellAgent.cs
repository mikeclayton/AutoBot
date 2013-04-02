using System;
using System.Collections;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Reflection;
using AutoBot.Core.Chat;
using AutoBot.Core.Engine;
using Castle.Core.Logging;

namespace AutoBot.Agents.PowerShell
{

    public sealed class PowerShellAgent : IAgent
    {

        #region Constructors

        public PowerShellAgent(ILogger logger)
        {
            // copy the parameters locally so the OnWrite handler can access them
            this.Logger = logger;
            this.ScriptPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Scripts");
        }

        #endregion

        #region Properties

        private ILogger Logger
        {
            get;
            set;
        }

        private string ScriptPath
        {
            get;
            set;
        }

        #endregion

        #region IAgent Interface

        public void Execute(IChatMessage message, IChatResponse response)
        {
            // parse the command so we know what to run
            var command = this.ParseCommand(message.CommandText);
            if (command == null)
            {
                response.Write("Erk! Not sure what to say to that.");
                return;
            }
            // check the script module exists
            var modulePath = this.GetFullModulePath(command.Command);
            if (!File.Exists(modulePath))
            {
                response.Write("Unknown command! Try \"@autobot Get-Help\" instead.");
                return;
            }
            // initialise the host
            var host = new Host(this.Logger);
            // add a handler for OnWrite events so we can bubble them up to the chat session
            var hostUI = (host.UI as UserInterface);
            if (hostUI != null)
            {
                hostUI.OnWrite += delegate(object sender, string value) { response.Write(value); };
            }
            // create a new initial state with the script module loaded
            var state = InitialSessionState.CreateDefault();
            state.ImportPSModule(new string[] { modulePath });
            // run the script inside the host
            using (var runspace = RunspaceFactory.CreateRunspace(host, state))
            {
                runspace.Open();
                using (var invoker = new RunspaceInvoke(runspace))
                {
                    try
                    {
                        // execute the PowerShell function with the same name as the module 
                        IList errors;
                        var psObjects = invoker.Invoke(string.Format("{0} {1}", command.Command, command.Parameters), null, out errors);
                        // handle any errors
                        if ((errors != null) && (errors.Count > 0))
                        {
                            var errorString = new System.Text.StringBuilder();
                            foreach (var error in errors)
                            {
                                errorString.AppendLine(error.ToString());
                            }
                            this.Logger.Error(string.Format("ERROR!: {0}", errorString.ToString()));
                            response.Write(string.Format("OOohhh, I got an error running {0}. It looks like this:", command));
                            response.Write(errorString.ToString());
                            return;
                        }
                        // write the result
                        foreach (var psObject in psObjects)
                        {
                            response.Write(this.SerializePSObject(psObject));
                        }
                    }
                    catch (Exception ex)
                    {
                        this.Logger.Error("ERROR!: ", ex);
                        response.Write(string.Format("Urghhh!, that didn't taste nice!  There's a problem with me running the {0} script.", command));
                        response.Write(string.Format("Check you are calling the script correctly by using \"@autobot get-help {0}\"", command));
                        response.Write("If all else fails ask your administrator for the event/error log entry.");
                    }
                }
            }
        }

        #endregion

        #region Helpers

        private PowerShellCommand ParseCommand(string chatText)
        {
            // check if the chat text is worth processing
            chatText = chatText == null ? null : chatText.Trim();
            if (string.IsNullOrEmpty(chatText)) { return null; }
            // process the chat text
            var words = chatText.Split(' ');
            var command = words[0];
            switch (command)
            {
                case "coolio":
                case "superb":
                    // intercept some special keywords
                    return new PowerShellCommand("Get-RandomImage", command);
                default:
                    // extract the first word as the command, and the rest as parameters
                    var parameters = (words.Length < 2) ? string.Empty : string.Join(" ", words, 1, words.Length - 1);
                    return new PowerShellCommand(command, parameters);
            }
        }

        private string GetFullModulePath(string filenameWithoutExtension)
        {
            return Path.Combine(this.ScriptPath, filenameWithoutExtension + ".psm1");
        }

        private string SerializePSObject(PSObject psObject)
        {
            // handle some trivial cases
            if ((psObject == null) || (psObject.BaseObject == null))
            {
                return null;
            }
            // convert the base object to a string based on its type
            var baseType = psObject.BaseObject.GetType();
            if (baseType == typeof(Hashtable))
            {
                var value = new System.Text.StringBuilder();
                foreach (DictionaryEntry dictionaryEntry in (Hashtable)psObject.BaseObject)
                {
                    value.AppendFormat("{0} = {1}\n", dictionaryEntry.Key, dictionaryEntry.Value);
                }
                return value.ToString();
            }
            else
            {
                return psObject.ToString();
            }
        }

        #endregion

    }   

}
