using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Management.Automation;
using AutoBot.HipChat;
using jabber;
using jabber.protocol.client;
using log4net;
using System.Threading;

namespace AutoBot
{
    public class BotEngine : MarshalByRefObject
    {

        private readonly HipChatSession Session = new HipChatSession(LogManager.GetLogger(typeof(HipChatSession)));
        private ManualResetEvent _mWaiter;
        private Thread _thread;
        private bool _serviceStarted = false;

        private readonly ILog Logger = LogManager.GetLogger(typeof(BotEngine));

        public BotEngine()
        {
            Session.Server = ConfigurationManager.AppSettings["HipChatServer"];
            Session.UserName = ConfigurationManager.AppSettings["HipChatUsername"];
            Session.Password = ConfigurationManager.AppSettings["HipChatPassword"];
            Session.Resource = ConfigurationManager.AppSettings["HipChatResource"];
            Session.MentionName = ConfigurationManager.AppSettings["HipChatBotMentionName"];
            Session.NickName = ConfigurationManager.AppSettings["HipChatBotNickName"];
            Session.SubscribedRooms = ConfigurationManager.AppSettings["HipChatRooms"];
            Session.OnMessageReceived += Session_OnMessageReceived;
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

        private void Session_OnMessageReceived(object sender, Message message)
        {
            if (message.Body == null && message.X == null)
                return;

            string chatText = message.Body == null ? message.X.InnerText.Trim() : message.Body.Trim();

            if (string.IsNullOrEmpty(chatText) || chatText == " ")
                return;
            
            JID responseJid = new JID(message.From.User, message.From.Server, message.From.Resource);
            
            // intercept a handful of messages not directly for AutoBot
            if (message.Type == MessageType.groupchat && !chatText.Trim().StartsWith(Session.MentionName))
            {
                chatText = RemoveMentionFromMessage(chatText);
                SendRandomResponse(responseJid, chatText, message.Type);
                return;
            }

            // ensure the message is intended for AutoBot
            chatText = RemoveMentionFromMessage(chatText);
            PowerShellCommand powerShellCommand = BuildPowerShellCommand(chatText);

            var runner = new PowerShellRunner(Session, message.Type, responseJid);
            Collection<PSObject> psObjects = runner.RunPowerShellModule(powerShellCommand.CommandText,
                                                                            powerShellCommand.ParameterText);
            SendResponse(responseJid, psObjects, message.Type);
        }

        private string RemoveMentionFromMessage(string chatText)
        {
            //TODO: Remove all @'s
            return chatText.Replace(Session.MentionName, string.Empty).Trim();
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

        private void SendResponse(JID replyTo, Collection<PSObject> psObjects, MessageType messageType)
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

                Session.SendMessage(messageType, replyTo, message);
            }
        }

        private void SendRandomResponse(JID replyTo, string chatText, MessageType messageType)
        {
            string[] chatTextWords = chatText.Split(' ');
            string message = string.Empty;
            switch (chatTextWords[0])
            {
                case "coolio":
                case "superb":
                    message = "Get-RandomImage " + chatText;
                    break;
                default:
                    break;
            }

            if (message != string.Empty)
            {
                PowerShellCommand powerShellCommand = BuildPowerShellCommand(message);
                var runner = new PowerShellRunner(Session, messageType, replyTo);
                Collection<PSObject> psObjects = runner.RunPowerShellModule(powerShellCommand.CommandText,
                                                                                powerShellCommand.ParameterText);
                SendResponse(replyTo, psObjects, messageType);
            }
            return;
        }

    }

}
