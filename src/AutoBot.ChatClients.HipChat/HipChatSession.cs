using System;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using AutoBot.Core.Chat;
using jabber;
using jabber.client;
using jabber.connection;
using jabber.protocol;
using jabber.protocol.client;
using jabber.protocol.iq;
using System.Configuration;
using Castle.Core.Logging;

namespace AutoBot.ChatClients.HipChat
{

    public sealed class HipChatSession : IChatSession
    {

        #region Events

        public event EventHandler<MessageReceivedEventArgs> MessageReceived;

        #endregion

        #region Fields

        private JabberClient _mJabberClient;
        private DiscoManager _mDiscoManager;
        private PresenceManager _mPresenceManager;

        #endregion

        #region Constructors

        public HipChatSession(ILogger logger)
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
            _mJabberClient.Login();
        }

        private void jabber_OnDisconnect(object sender)
        {
            this.Logger.Info("jabber_OnDisconnect - Disconnecting");

        }

        private void jabber_OnStreamInit(object o, ElementStream elementStream)
        {
            var client = (JabberClient)o;
            _mDiscoManager.Stream = client;
        }

        private void jabber_OnConnect(object o, StanzaStream s)
        {
            Logger.Info("jabber_OnConnect - Connecting");
            var client = (JabberClient)o;
        }

        private void jabber_OnAuthenticate(object o)
        {
            Logger.Info("Authenticated");
            _mDiscoManager.BeginFindServiceWithFeature(URI.MUC, hlp_DiscoHandler_FindServiceWithFeature, new object());
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
                _mDiscoManager.BeginGetItems(node, hlp_DiscoHandler_SubscribeToRooms, new object());
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
                    // we have to build a new JID here, with the nickname included http://xmpp.org/extensions/xep-0045.html#enter-muc
                    JID subscriptionJid = new JID(dn.JID.User, dn.JID.Server, "AutoBot .");
                    _mJabberClient.Subscribe(subscriptionJid, this.NickName, null);
                }
            }
        }

        #endregion

        #region PresenceManager Event Handlers

        private void presenceManager_OnPrimarySessionChange(object sender, JID bare)
        {
            if (bare.Bare.Equals(_mJabberClient.JID.Bare, StringComparison.InvariantCultureIgnoreCase))
                return;
        }

        #endregion

        #region IChatSession Interface

        public void Connect()
        {
            _mJabberClient = new JabberClient
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
            _mPresenceManager = new PresenceManager
            {
                Stream = _mJabberClient
            };
            _mDiscoManager = new DiscoManager();
            _mPresenceManager.OnPrimarySessionChange += presenceManager_OnPrimarySessionChange;
            _mJabberClient.OnConnect += jabber_OnConnect;
            _mJabberClient.OnAuthenticate += jabber_OnAuthenticate;
            _mJabberClient.OnInvalidCertificate += jabber_OnInvalidCertificate;
            _mJabberClient.OnError += jabber_OnError;
            _mJabberClient.OnReadText += jabber_OnReadText;
            _mJabberClient.OnWriteText += jabber_OnWriteText;
            _mJabberClient.OnStreamInit += jabber_OnStreamInit;
            _mJabberClient.OnDisconnect += jabber_OnDisconnect;
            _mJabberClient.OnRegistered += jabber_OnRegistered;
            _mJabberClient.OnRegisterInfo += jabber_OnRegisterInfo;
            _mJabberClient.OnMessage += jabber_OnMessage;
            // connect to the HipChat server
            Logger.Info(string.Format("Connecting to '{0}'", _mJabberClient.Server));
            _mJabberClient.Connect();
            int retryCountLimit = 10;
            while (!_mJabberClient.IsAuthenticated && retryCountLimit > 0)
            {
                Logger.Info(string.Format("Waiting..."));
                retryCountLimit--;
                Thread.Sleep(1000);
            }
            if (_mJabberClient.IsAuthenticated)
            {
                Logger.Info(string.Format("Authenticated as '{0}'", _mJabberClient.User));
            }
        }

        public void Disconnect()
        {
            this.Logger.Info(string.Format("Disconnecting from '{0}'", _mJabberClient.Server));
            _mJabberClient.Close();
        }

        #endregion

        #region Methods

        public void OnMessageReceived(Message message)
        {
            // take a local copy of the event so we don't get a race condition on the next line
            var handler = this.MessageReceived;
            if (handler != null)
            {
                // extract the chat text
                if (message.Body == null && message.X == null)
                {
                    return;
                }
                var commandText = message.Body == null ? message.X.InnerText.Trim() : message.Body.Trim();
                // intercept a handful of messages not directly for AutoBot
                if (message.Type == MessageType.groupchat && commandText.Trim().StartsWith(this.MentionName))
                {
                    commandText = this.RemoveMentionFromMessage(commandText);
                    // process random message
                }
                // build the chat message and response to pass to the event handler
                var chatMessage = new HipChatMessage(message.Type, message.Body, commandText);
                var responseJid = new JID(message.From.User, message.From.Server, message.From.Resource);
                var chatResponse = new HipChatResponse(this, responseJid, message.Type);
                var args = new MessageReceivedEventArgs(chatMessage, chatResponse);
                // call the event handler
                handler(this, args);
            }
        }

        public void SendResponse(MessageType messageType, string replyTo, string message)
        {
            _mJabberClient.Message(messageType, replyTo, message);
        }

        private string RemoveMentionFromMessage(string chatText)
        {
            //TODO: Remove all @'s
            return chatText.Replace(this.MentionName, string.Empty).Trim();
        }

        #endregion

    }

}
