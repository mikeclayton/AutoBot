using AutoBot.Core.Chat;
using Castle.Core.Logging;
using jabber;
using jabber.client;
using jabber.connection;
using jabber.protocol;
using jabber.protocol.client;
using jabber.protocol.iq;
using System;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Xml;

namespace AutoBot.ChatClients.HipChat
{

    public sealed class HipChatSession : IChatSession
    {

        #region Events

        public event EventHandler<MessageReceivedEventArgs> MessageReceived;

        #endregion

        #region Fields

        private JabberClient _jabberClient;
        private DiscoManager _discoManager;
        private PresenceManager _presenceManager;

        #endregion

        #region Constructors

        public HipChatSession(ILogger logger)
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

        public string Server
        {
            get;
            set;
        }

        public string UserName
        {
            get;
            set;
        }

        public string Password
        {
            get;
            set;
        }
        public string Resource
        {
            get;
            set;
        }

        public string MentionName
        {
            get;
            set;
        }

        public string NickName
        {
            get;
            set;
        }

        public string SubscribedRooms
        {
            get;
            set;
        }

        #endregion

        #region JabberClient Event Handlers

        private void jabber_OnAfterPresenceOut(object sender, Presence pres)
        {
            this.Logger.Info("jabber_OnAfterPresenceOut");
        }

        private void jabber_OnAuthenticate(object o)
        {
            this.Logger.Info("jabber_OnAuthenticate - Authenticated");
            _discoManager.BeginFindServiceWithFeature(URI.MUC, discoManager_FindServiceWithFeature, new object());
        }

        private void jabber_OnAuthError(object sender, XmlElement rp)
        {
            this.Logger.Info("jabber_OnAuthError");
        }

        private void jabber_OnBeforePresenceOut(object sender, Presence pres)
        {
            this.Logger.Info("jabber_OnBeforePresenceOut");
        }

        private void jabber_OnConnect(object o, StanzaStream s)
        {
            this.Logger.Info("jabber_OnConnect - Connecting");
            var client = (JabberClient)o;
        }

        private void jabber_OnDisconnect(object sender)
        {
            this.Logger.Info("jabber_OnDisconnect - Disconnecting");
        }

        private void jabber_OnError(object o, Exception ex)
        {
            this.Logger.Info("jabber_OnError");
            this.Logger.Error("ERROR!:", ex);
            throw ex;
        }

        private bool jabber_OnInvalidCertificate(object o, X509Certificate cert, X509Chain chain, SslPolicyErrors errors)
        {
            // the current jabber server has an invalid certificate,
            // but override validation and accept it anyway.
            this.Logger.Info("jabber_OnInvalidCertificate");
            this.Logger.Info("Validating certificate");
            return true;
        }

        private void jabber_OnIQ(object sender, IQ iq)
        {
            //this.Logger.Info("jabber_OnIQ");
            if (iq.Type == IQType.error)
            {
                this.Logger.Error(string.Format("IQ  : {0}", iq.OuterXml));
            }
        }

        private void jabber_OnLoginRequired(object sender)
        {
            this.Logger.Info("jabber_OnLoginRequired");
        }

        private void jabber_OnMessage(object sender, Message msg)
        {
            this.Logger.Debug(string.Format("RECV From: {0}@{1} : {2}", msg.From.User, msg.From.Server, msg.Body));
            this.OnMessageReceived(msg);
        }

        private void jabber_OnPresence(object sender, Presence pres)
        {
            //this.Logger.Info("jabber_OnPresence");
            if (pres.Type == PresenceType.error)
            {
                this.Logger.Error(string.Format("PRES: {0}", pres.OuterXml));
            }
        }

        private void jabber_OnProtocol(object sender, XmlElement rp)
        {
            //this.Logger.Info("jabber_OnProtocol");
        }

        private void jabber_OnReadText(object sender, string text)
        {
            //this.Logger.Info("jabber_OnReadText");
            // ignore keep-alive spaces
            if (text == " ")
            {
                this.Logger.Debug("RECV: Keep alive");
                return;
            }
            this.Logger.Debug(string.Format("RECV: {0}", text));
        }

        private void jabber_OnRegistered(object sender, IQ iq)
        {
            this.Logger.Info("jabber_OnRegistered");
            _jabberClient.Login();
        }

        private bool jabber_OnRegisterInfo(object sender, Register register)
        {
            this.Logger.Info("jabber_OnRegisterInfo");
            return true;
        }

        private void jabber_OnStreamInit(object o, ElementStream elementStream)
        {
            //this.Logger.Info("jabber_OnStreamInit");
            var client = (JabberClient)o;
            _discoManager.Stream = client;
        }

        private void jabber_OnStreamError(object sender, XmlElement rp)
        {
            // event handled in jabber_OnMessage
            this.Logger.Info("jabber_OnStreamError");
            //this.Logger.Error(string.Format("STER: {0}", rp.OuterXml));
        }

        private void jabber_OnStreamHeader(object sender, XmlElement rp)
        {
            // event handled in jabber_OnMessage
            //this.Logger.Info("jabber_OnStreamHeader");
            //this.Logger.Debug(string.Format("STHD: {0}", rp.OuterXml));
        }

        private void jabber_OnWriteText(object sender, string text)
        {
            //this.Logger.Info("jabber_OnWriteText");
            // ignore keep-alive spaces
            if (text == " ")
            {
                this.Logger.Debug("RECV: Keep alive");
                return;
            }
            this.Logger.Debug(string.Format("SEND: {0}", text));
        }

        #endregion

        #region DiscoManager Event Handlers

        private void discoManager_OnStreamChanged(object sender)
        {
            this.Logger.Info("discoManager_OnStreamChanged");
        }

        private void discoManager_FindServiceWithFeature(DiscoManager sender, DiscoNode node, object state)
        {
            if (node == null)
            {
                return;
            }
            if (node.Name == "Rooms")
            {
                _discoManager.BeginGetItems(node, discoManager_SubscribeToRooms, new object());
            }
        }

        private void discoManager_SubscribeToRooms(DiscoManager sender, DiscoNode node, object state)
        {
            if (node == null)
            {
                return;
            }
            if (node.Children != null && SubscribedRooms == "@all")
            {
                foreach (DiscoNode dn in node.Children)
                {
                    Logger.Info(string.Format("Subscribing to: {0}:{1}", dn.JID, dn.Name));
                    // hipchat no longer supports Groupchat 1.0 Protocol to enter rooms,
                    // but jabber.net uses Groupchat to join rooms so we have to create
                    // a Basic MUC Protocol message by hand instead.
                    //
                    // see http://help.hipchat.com/knowledgebase/articles/64377-xmpp-jabber-support-details
                    //     http://xmpp.org/extensions/xep-0045.html#enter-gc
                    //     http://xmpp.org/extensions/xep-0045.html#enter-muc
                    var presenceMessage = new XmlDocument();
                    presenceMessage.LoadXml("<presence from='{0}' id='{1}' to='{2}'>" +
                                            "  <x xmlns='http://jabber.org/protocol/muc'/>" +
                                            "</presence>");
                    // set the "from" value
                    var presenceFrom = presenceMessage.SelectSingleNode("presence/@from");
                    if (presenceFrom == null)
                    {
                        throw new InvalidOperationException();
                    }
                    presenceFrom.InnerText = new JID(this.UserName, dn.JID.Server, this.Resource);
                    // set the "to" value
                    var presenceTo = presenceMessage.SelectSingleNode("presence/@to");
                    if (presenceTo == null)
                    {
                        throw new InvalidOperationException();
                    }
                    presenceTo.InnerText = new JID(dn.JID.User, dn.JID.Server, this.NickName); ;
                    // write the message
                    _jabberClient.Write(presenceMessage.DocumentElement);
                }
            }
        }

        #endregion

        #region PresenceManager Event Handlers

        private void presenceManager_OnPrimarySessionChange(object sender, JID bare)
        {
            this.Logger.Info("presenceManager_OnPrimarySessionChange");
            if (bare.Bare.Equals(_jabberClient.JID.Bare, StringComparison.InvariantCultureIgnoreCase))
            {
                return;
            }
        }

        private void presenceManager_OnStreamChanged(object sender)
        {
            this.Logger.Info("presenceManager_OnStreamChanged");
        }

        #endregion

        #region IChatSession Interface

        public void Connect()
        {
            _jabberClient = new JabberClient
            {
                Server = this.Server,
                User = this.UserName,
                Password = this.Password,
                Resource = this.Resource,
                AutoStartTLS = true,
                PlaintextAuth = true,
                AutoPresence = true,
                AutoRoster = false,
                AutoReconnect = -1,
                AutoLogin = true
            };
            _presenceManager = new PresenceManager
            {
                Stream = _jabberClient
            };
            _discoManager = new DiscoManager();
            // add presence manager event handlers
            _presenceManager.OnPrimarySessionChange += presenceManager_OnPrimarySessionChange;
            _presenceManager.OnStreamChanged += presenceManager_OnStreamChanged;
            _presenceManager.OnPrimarySessionChange += presenceManager_OnPrimarySessionChange;
            // add jabber client event handlers
            _jabberClient.OnAfterPresenceOut += jabber_OnAfterPresenceOut;
            _jabberClient.OnAuthenticate += jabber_OnAuthenticate;
            _jabberClient.OnAuthError += jabber_OnAuthError;
            _jabberClient.OnBeforePresenceOut += jabber_OnBeforePresenceOut;
            _jabberClient.OnConnect += jabber_OnConnect;
            _jabberClient.OnDisconnect += jabber_OnDisconnect;
            _jabberClient.OnError += jabber_OnError;
            _jabberClient.OnInvalidCertificate += jabber_OnInvalidCertificate;
            _jabberClient.OnIQ += jabber_OnIQ;
            _jabberClient.OnLoginRequired += jabber_OnLoginRequired;
            _jabberClient.OnMessage += jabber_OnMessage;
            _jabberClient.OnPresence += jabber_OnPresence;
            _jabberClient.OnProtocol += jabber_OnProtocol;
            _jabberClient.OnReadText += jabber_OnReadText;
            _jabberClient.OnRegistered += jabber_OnRegistered;
            _jabberClient.OnRegisterInfo += jabber_OnRegisterInfo;
            _jabberClient.OnStreamError += jabber_OnStreamError;
            _jabberClient.OnStreamHeader += jabber_OnStreamHeader;
            _jabberClient.OnStreamInit += jabber_OnStreamInit;
            _jabberClient.OnWriteText += jabber_OnWriteText;
            // add discovery manager event handlers
            _discoManager.OnStreamChanged += discoManager_OnStreamChanged;
            // connect to the HipChat server
            this.Logger.Info(string.Format("Connecting to '{0}'", _jabberClient.Server));
            _jabberClient.Connect();
            var retryCountLimit = 10;
            while (!_jabberClient.IsAuthenticated && retryCountLimit > 0)
            {
                this.Logger.Info(string.Format("Waiting..."));
                retryCountLimit--;
                Thread.Sleep(1000);
            }
            if (_jabberClient.IsAuthenticated)
            {
                this.Logger.Info(string.Format("Authenticated as '{0}'", _jabberClient.User));
            }
        }

        public void Disconnect()
        {
            this.Logger.Info(string.Format("Disconnecting from '{0}'", _jabberClient.Server));
            _jabberClient.Close();
        }

        #endregion

        #region Methods

        private void OnMessageReceived(Message message)
        {
            // take a local copy of the event so we don't get a race condition further down
            var handler = this.MessageReceived;
            if (handler == null)
            {
                return;
            }
            // skip blank messages
            if (message.Body == null && message.X == null)
            {
                return;
            }
            // skip messages from the bot
            if (message.From.Resource == this.NickName)
            {
                return;
            }
            // extract the chat text
            var commandText = (message.Body == null) ? message.X.InnerText.Trim() : message.Body.Trim();
            if ((message.Type == MessageType.groupchat) && commandText.StartsWith(this.MentionName))
            {
                commandText = this.RemoveMentionsFromMessage(commandText);
            }
            // build the chat message and response to pass to the event handler
            var chatMessage = new HipChatMessage(message.Type, message.Body, commandText);
            var responseJid = new JID(message.From.User, message.From.Server, message.From.Resource);
            var chatResponse = new HipChatResponse(this, responseJid, message.Type);
            var args = new MessageReceivedEventArgs(chatMessage, chatResponse);
            // call the event handler
            handler(this, args);
        }

        internal void SendResponse(MessageType messageType, string replyTo, string message)
        {
            _jabberClient.Message(messageType, replyTo, message);
        }

        private string RemoveMentionsFromMessage(string chatText)
        {
            // TODO: Remove all @mentions
            return chatText.Replace(this.MentionName, string.Empty).Trim();
        }

        #endregion

    }

}
