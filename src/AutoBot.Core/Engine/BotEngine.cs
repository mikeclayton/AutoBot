using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Management.Automation;
using System.Threading;
using AutoBot.Core.Chat;
using log4net;

namespace AutoBot.Core.Engine
{

    public sealed class BotEngine : MarshalByRefObject
    {

        private readonly IChatSession Session;
        private ManualResetEvent _mWaiter;
        private Thread _thread;
        private bool _serviceStarted = false;

        private readonly ILog Logger = LogManager.GetLogger(typeof(BotEngine));

        public BotEngine(IChatSession session)
        {
            this.Session = session;
            _thread = new Thread(delegate()
                                     {
                                         while (_serviceStarted)
                                         {
                                             //TODO Add background task implementation here
                                             Thread.Sleep(1000);
                                         }
                                         Thread.CurrentThread.Abort();
                                     }
                                );
        }

        public void Connect()
        {
            Session.Connect();
            _serviceStarted = true;
            _thread.Start();
            // connect. this is synchronous so we'll use a manual reset event
            // to pause this thread forever. client events will continue to
            // fire but we won't have to worry about setting up an idle "while" loop.
            _mWaiter = new ManualResetEvent(false);
            _mWaiter.WaitOne();

        }

        public void Disconnect()
        {
            Session.Disconnect();
            _serviceStarted = false;
            _thread.Join(5000);
        }


        public void ProcessMessage(object sender, MessageReceivedEventArgs e)
        {
            // check if the message is worth processing
            var chatText = e.Message.CommandText;
            if (string.IsNullOrEmpty(chatText) || string.IsNullOrEmpty(chatText.Trim()))
            {
                return;
            }
            // intercept some special "commands"
            string[] words = chatText.Split(' ');
            switch (words[0])
            {
                case "coolio":
                case "superb":
                    chatText = "Get-RandomImage " + chatText;
                    break;
                default:
                    break;
            }
            // execute the command
            PowerShellCommand powerShellCommand = BuildPowerShellCommand(chatText);
            var runner = new PowerShellRunner(this.Logger, e.Response);
            Collection<PSObject> psObjects = runner.RunPowerShellModule(powerShellCommand.CommandText,
                                                                            powerShellCommand.ParameterText);            
            SendResponse(e.Response, psObjects);
        }

        private static PowerShellCommand BuildPowerShellCommand(string chatText)
        {
            string[] chatTextArgs = chatText.Split(' ');
            string command = string.Empty;
            string parameters = string.Empty;

            command = chatTextArgs[0];

            for (int i = 0 + 1; i < chatTextArgs.Count(); i++)
                parameters += chatTextArgs[i] + " ";

            return new PowerShellCommand(command, parameters);
        }

        private void SendResponse(IChatResponse response, Collection<PSObject> psObjects)
        {
            foreach (var psObject in psObjects)
            {
                Logger.Info(psObject.ImmediateBaseObject.GetType().FullName);
                string message = string.Empty;
                
                // the PowerShell (.NET) return types we are supporting
                if (psObject.BaseObject.GetType() == typeof(string))
                    message = psObject.ToString();
                
                else if (psObject.BaseObject.GetType() == typeof(Hashtable))
                {
                    Hashtable hashTable = (Hashtable)psObject.BaseObject;

                    foreach (DictionaryEntry dictionaryEntry in hashTable)
                        message += string.Format("{0} = {1}\n", dictionaryEntry.Key, dictionaryEntry.Value);
                }
                response.Write(message);
            }
        }

    }

}
