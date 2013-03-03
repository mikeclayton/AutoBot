using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using AutoBot.Core.Chat;
using AutoBot.Core.Host;
using Castle.Core.Logging;

namespace AutoBot.Core.Engine
{

    public sealed class PowerShellRunner
    {
        private readonly string _scriptsPath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "Scripts");

        internal PowerShellRunner(ILogger logger, IChatResponse response)
        {
            // copy the parameters locally so the OnWrite handler can access them
            this.Logger = logger;
            this.Response = response;
        }

        private ILogger Logger
        {
            get;
            set;
        }

        private IChatResponse Response
        {
            get;
            set;
        }

        internal Collection<PSObject> RunPowerShellModule(string scriptName, string command)
        {

            if (!File.Exists(GetPath(scriptName)))
            {
                return new Collection<PSObject>
                                            {
                                                new PSObject("Unknown command!, try \"@autobot Get-Help\" instead")
                                            };
            }

            // initialise the host
            var host = new AutoBotHost(this.Logger);
            // add a handler for OnWrite events so we can bubble them up to the hipchat session
            var hostUI = (host.UI as AutoBotUserInterface);
            if (hostUI != null)
            {
                hostUI.OnWrite += AutoBotUserInterface_OnWrite;
            }
            // run the script inside the host
            using (var runspace = RunspaceFactory.CreateRunspace(host))
            {
                runspace.Open();
                using (var invoker = new RunspaceInvoke(runspace))
                {
                    string scriptPath = GetPath(scriptName);
                    Collection<PSObject> psObjects;

                    invoker.Invoke(string.Format("Import-Module {0}", scriptPath));

                    try
                    {
                        // execute the PowerShell Function with the same name as the module 
                        IList errors;
                        psObjects = invoker.Invoke(string.Format("{0} {1}", scriptName, command), null, out errors);
                        if (errors.Count > 0)
                        {
                            string errorString = string.Empty;
                            foreach (var error in errors)
                                errorString += error.ToString();

                            this.Logger.Error(string.Format("ERROR!: {0}", errorString));
                            return new Collection<PSObject>
                                            {
                                                new PSObject(string.Format("OOohhh, I got an error running {0}.  It looks like this: \r\n{1}.", scriptName, errorString))
                                            };
                        }
                        return psObjects;

                    }
                    catch (Exception ex)
                    {
                        this.Logger.Error("ERROR!:", ex);
                        string errorText = string.Format("Urghhh!, that didn't taste nice!  There's a problem with me running the {0} script. \r\n", scriptName);
                        errorText += String.Format("Check you are calling the script correctly by using \"@autobot get-help {0}\" \r\n", scriptName);
                        errorText += "If all else fails ask your administrator for the event/error log entry.";

                        return new Collection<PSObject>
                                            {
                                                new PSObject(errorText)
                                            };
                    }
                    finally
                    {
                        invoker.Invoke(string.Format("Remove-Module {0}", scriptName));
                    }

                }
            }

        }

        internal void AutoBotUserInterface_OnWrite(object sender, string value)
        {
            this.Response.Write(value);
        }

        internal string GetPath(string filenameWithoutExtension)
        {
            string path = string.Empty;
            if (!(_scriptsPath.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal) || _scriptsPath.EndsWith(Path.AltDirectorySeparatorChar.ToString(), StringComparison.Ordinal)))
            {
                path = _scriptsPath;
                path += Path.DirectorySeparatorChar;
            }
            
            return path + filenameWithoutExtension + ".psm1";
        }

    }   

    public class PowerShellCommand
    {
        public string CommandText { get; set; }
        public string ParameterText { get; set; }

        public PowerShellCommand(string scriptName, string parameter)
        {
            CommandText = scriptName;
            ParameterText = parameter;
        }
    }

}
