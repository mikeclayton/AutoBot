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

        private void jabber_OnMessage(object sender, Message msg)
        {
            Logger.Debug(string.Format("RECV From: {0}@{1} : {2}", msg.From.User, msg.From.Server, msg.Body));
            this.OnMessageReceived(msg);
        }

        private bool jabber_OnRegisterInfo(object sender, Register register)
        {
            return true;
        }

        private void jabber_OnRegistered(object sender, IQ iq)
        {
            _jabberClient.Login();
        }

        private void jabber_OnDisconnect(object sender)
        {
            this.Logger.Info("jabber_OnDisconnect - Disconnecting");

        }

        private void jabber_OnStreamInit(object o, ElementStream elementStream)
        {
            var client = (JabberClient)o;
            _discoManager.Stream = client;
        }

        private void jabber_OnConnect(object o, StanzaStream s)
        {
            Logger.Info("jabber_OnConnect - Connecting");
            var client = (JabberClient)o;
        }

        private void jabber_OnAuthenticate(object o)
        {
            Logger.Info("Authenticated");
            _discoManager.BeginFindServiceWithFeature(URI.MUC, hlp_DiscoHandler_FindServiceWithFeature, new object());
        }

        private bool jabber_OnInvalidCertificate(object o, X509Certificate cert, X509Chain chain, SslPolicyErrors errors)
        {
            // the current jabber server has an invalid certificate,
            // but override validation and accept it anyway.
            Logger.Info("Validating certificate");
            return true;
        }

        private void jabber_OnError(object o, Exception ex)
        {
            Logger.Error("ERROR!:", ex);
            throw ex;
        }

        private void jabber_OnReadText(object sender, string text)
        {
            // ignore keep-alive spaces
            if (text == " ")
            {
                Logger.Debug("RECV: Keep alive");
                return;
            }
            if(text.StartsWith("<iq"))
            {
                var xml = new XmlDocument();
                xml.LoadXml(text);
                var type = xml.SelectSingleNode("iq/@type");
                if ((type != null) && (type.InnerText == "error"))
                {
                    Logger.Error(string.Format("RECV: {0}", text));
                    return;
                }
            }
            Logger.Debug(string.Format("RECV: {0}", text));
        }

        private void jabber_OnWriteText(object sender, string text)
        {
            // ignore keep-alive spaces
            if (text == " ")
            {
                Logger.Debug("RECV: Keep alive");
                return;
            }
            Logger.Debug(string.Format("SEND: {0}", text));
        }

        #endregion

        #region DiscoHandler Event Handlers

        private void hlp_DiscoHandler_FindServiceWithFeature(DiscoManager sender, DiscoNode node, object state)
        {
            if (node == null)
                return;
            if (node.Name == "Rooms")
                _discoManager.BeginGetItems(node, hlp_DiscoHandler_SubscribeToRooms, new object());
        }

        private void hlp_DiscoHandler_SubscribeToRooms(DiscoManager sender, DiscoNode node, object state)
        {
            if (node == null)
                return;

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
            if (bare.Bare.Equals(_jabberClient.JID.Bare, StringComparison.InvariantCultureIgnoreCase))
                return;
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
            _presenceManager.OnPrimarySessionChange += presenceManager_OnPrimarySessionChange;
            _jabberClient.OnConnect += jabber_OnConnect;
            _jabberClient.OnAuthenticate += jabber_OnAuthenticate;
            _jabberClient.OnInvalidCertificate += jabber_OnInvalidCertificate;
            _jabberClient.OnError += jabber_OnError;
            _jabberClient.OnReadText += jabber_OnReadText;
            _jabberClient.OnWriteText += jabber_OnWriteText;
            _jabberClient.OnStreamInit += jabber_OnStreamInit;
            _jabberClient.OnDisconnect += jabber_OnDisconnect;
            _jabberClient.OnRegistered += jabber_OnRegistered;
            _jabberClient.OnRegisterInfo += jabber_OnRegisterInfo;
            _jabberClient.OnMessage += jabber_OnMessage;
            // connect to the HipChat server
            Logger.Info(string.Format("Connecting to '{0}'", _jabberClient.Server));
            _jabberClient.Connect();
            var retryCountLimit = 10;
            while (!_jabberClient.IsAuthenticated && retryCountLimit > 0)
            {
                Logger.Info(string.Format("Waiting..."));
                retryCountLimit--;
                Thread.Sleep(1000);
            }
            if (_jabberClient.IsAuthenticated)
            {
                Logger.Info(string.Format("Authenticated as '{0}'", _jabberClient.User));
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
            // extract the chat text
            if (message.Body == null && message.X == null)
            {
                return;
            }
            var commandText = (message.Body == null) ? message.X.InnerText.Trim() : message.Body.Trim();
            if (message.Type == MessageType.groupchat && commandText.Trim().StartsWith(this.MentionName))
            {
                commandText = this.RemoveMentionFromMessage(commandText);
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

        private string RemoveMentionFromMessage(string chatText)
        {
            // TODO: Remove all @'s
            return chatText.Replace(this.MentionName, string.Empty).Trim();
        }

        #endregion

    }

}
