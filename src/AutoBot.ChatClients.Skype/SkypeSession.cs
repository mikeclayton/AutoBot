using AutoBot.Core.Chat;
using Castle.Core.Logging;
using SKYPE4COMLib;
using System;

namespace AutoBot.ChatClients.Skype
{

    public class SkypeSession : IChatSession
    {

        #region Fields

        private SKYPE4COMLib.Skype _skype;

        #endregion

        #region Events

        public event EventHandler<MessageReceivedEventArgs> MessageReceived;

        #endregion

        #region Constructors

        public SkypeSession(ILogger logger)
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

        private SKYPE4COMLib.Skype Skype
        {
            get
            {
                if (_skype == null)
                {
                    _skype = new SKYPE4COMLib.Skype();
                }
                return _skype;
            }
        }

        #endregion

        #region Skype Event Handlers

        private void Skype_ApplicationConnecting(SKYPE4COMLib.Application application, UserCollection usercollection)
        {
            this.Logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name);
        }

        private void Skype_ApplicationDatagram(SKYPE4COMLib.Application application, ApplicationStream applicationstream, String itext)
        {
            this.Logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name);
        }

        private void Skype_ApplicationReceiving(SKYPE4COMLib.Application application, ApplicationStreamCollection applicationstreamcollection)
        {
            this.Logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name);
        }

        private void Skype_ApplicationSending(SKYPE4COMLib.Application application, ApplicationStreamCollection applicationstreamcollection)
        {
            this.Logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name);
        }

        private void Skype_ApplicationStreams(SKYPE4COMLib.Application application, ApplicationStreamCollection applicationstreamcollection)
        {
            this.Logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name);
        }

        private void Skype_AsyncSearchUsersFinished(int cookie, UserCollection usercollection)
        {
            this.Logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name);
        }

        private void Skype_AttachmentStatus(TAttachmentStatus status)
        {
            switch (status)
            {
                case TAttachmentStatus.apiAttachPendingAuthorization:
                    break;
                case TAttachmentStatus.apiAttachRefused:
                    break;
                case TAttachmentStatus.apiAttachSuccess:
                    break;
                default:
                    this.Logger.Debug(status.ToString());
                    break;
            }
        }

        private void Skype_AutoAway(bool automatic)
        {
            this.Logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name);
        }

        private void Skype_CallDtmfReceived(Call call, string code)
        {
            this.Logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name);
        }

        private void Skype_CallHistory()
        {
            this.Logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name);
        }

        private void Skype_CallInputStatusChanged(Call call, bool status)
        {
            this.Logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name);
        }

        private void Skype_CallSeenStatusChanged(Call call, bool status)
        {
            this.Logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name);
        }

        private void Skype_CallStatus(Call call, TCallStatus status)
        {
            this.Logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name);
        }

        private void Skype_CallTransferStatusChanged(Call call, TCallStatus status)
        {
            this.Logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name);
        }

        private void Skype_CallVideoReceiveStatusChanged(Call call, TCallVideoSendStatus status)
        {
            this.Logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name);
        }

        private void Skype_CallVideoSendStatusChanged(Call call, TCallVideoSendStatus status)
        {
            this.Logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name);
        }

        private void Skype_CallVideoStatusChanged(Call call, TCallVideoStatus status)
        {
            this.Logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name);
        }

        private void Skype_ChatMembersChanged(Chat chat, UserCollection usercollection)
        {
            this.Logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name);
        }

        private void Skype_ChatMemberRoleChanged(IChatMember ichatmember, TChatMemberRole role)
        {
            this.Logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name);
        }

        private void Skype_Command(Command command)
        {
            this.Logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name);
            this.Logger.Debug("    id       = " + command.Id);
            this.Logger.Debug("    command  = " + command.Command);
            this.Logger.Debug("    expected = " + command.Expected);
            this.Logger.Debug("    reply    = " + command.Reply);
        }

        private void Skype_ConnectionStatus(TConnectionStatus status)
        {
            this.Logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name);
            this.Logger.Debug("    status = " + status);
        }

        private void Skype_ContactsFocused(string contacts)
        {
            this.Logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name);
        }

        private void Skype_Error(Command command, int number, string description)
        {
            this.Logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name);
        }

        private void Skype_FileTransferStatusChanged(IFileTransfer ifiletransfer, TFileTransferStatus status)
        {
            this.Logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name);
        }

        private void Skype_GroupDeleted(int group)
        {
            this.Logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name);
        }

        private void Skype_GroupExpanded(Group group, bool expanded)
        {
            this.Logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name);
        }

        private void Skype_GroupUsers(Group group, UserCollection usercollection)
        {
            this.Logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name);
        }

        private void Skype_GroupVisible(Group group, bool visible)
        {
            this.Logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name);
        }

        private void Skype_MessageHistory(string user)
        {
            this.Logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name);
        }

        private void Skype_MessageStatus(ChatMessage chatmessage, TChatMessageStatus status)
        {
            this.Logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name);
            this.Logger.Debug("    status = " + status);
            switch (status)
            {
                case TChatMessageStatus.cmsRead:
                    break;
                case TChatMessageStatus.cmsReceived:
                    this.Logger.Debug("    body   = " + chatmessage.Body);
                    this.OnMessageReceived(chatmessage);
                    break;
                case TChatMessageStatus.cmsSending:
                    break;
                case TChatMessageStatus.cmsSent:
                    break;
                default:
                    throw new System.InvalidOperationException(status.ToString());
            }
        }

        private void Skype_Mute(bool mute)
        {
            this.Logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name);
        }

        private void Skype_OnlineStatus(User user, TOnlineStatus status)
        {
            this.Logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name);
        }

        private void Skype_PluginEventClicked(PluginEvent pluginevent)
        {
            this.Logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name);
        }

        private void Skype_PluginMenuItemClicked(PluginMenuItem pluginmenuitem, UserCollection usercollection, TPluginContext plugincontext, string contextid)
        {
            this.Logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name);
        }

        private void Skype_Reply(Command command)
        {
            this.Logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name);
            this.Logger.Debug("    id       = " + command.Id);
            this.Logger.Debug("    command  = " + command.Command);
            this.Logger.Debug("    expected = " + command.Expected);
            this.Logger.Debug("    reply    = " + command.Reply);
        }

        private void Skype_SilentModeStatusChanged(bool silent)
        {
            this.Logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name);
        }

        private void Skype_SmsMessageStatusChanged(SmsMessage smsmessage, TSmsMessageStatus status)
        {
            this.Logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name);
        }

        private void Skype_SmsTargetStatusChanged(SmsTarget smstarget, TSmsTargetStatus status)
        {
            this.Logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name);
        }

        private void Skype_UILanguageChanged(string code)
        {
            this.Logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name);
        }

        private void Skype_UserAuthorizationRequestReceived(User user)
        {
            this.Logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name);
        }

        private void Skype_UserMood(User user, string moodtext)
        {
            this.Logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name);
        }

        private void Skype_UserStatus(TUserStatus status)
        {
            this.Logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name);
            this.Logger.Debug("    status = " + status);
        }

        private void Skype_VoicemailStatus(Voicemail voicemail, TVoicemailStatus status)
        {
            this.Logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name);
        }

        private void Skype_WallpaperChanged(string path)
        {
            this.Logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name);
        }

        #endregion

        #region Methods

        private void AttachEventHandlers(SKYPE4COMLib.Skype skype)
        {
            var evt = (_ISkypeEvents_Event)skype;
            evt.ApplicationConnecting += this.Skype_ApplicationConnecting;
            evt.ApplicationDatagram += this.Skype_ApplicationDatagram;
            evt.ApplicationReceiving += this.Skype_ApplicationReceiving;
            evt.ApplicationSending += this.Skype_ApplicationSending;
            evt.ApplicationStreams += this.Skype_ApplicationStreams;
            evt.AsyncSearchUsersFinished += this.Skype_AsyncSearchUsersFinished;
            evt.AttachmentStatus += this.Skype_AttachmentStatus;
            evt.AutoAway += this.Skype_AutoAway;
            evt.CallDtmfReceived += this.Skype_CallDtmfReceived;
            evt.CallHistory += this.Skype_CallHistory;
            evt.CallInputStatusChanged += this.Skype_CallInputStatusChanged;
            evt.CallSeenStatusChanged += this.Skype_CallSeenStatusChanged;
            evt.CallStatus += this.Skype_CallStatus;
            evt.CallTransferStatusChanged += this.Skype_CallTransferStatusChanged;
            evt.CallVideoReceiveStatusChanged += this.Skype_CallVideoReceiveStatusChanged;
            evt.CallVideoSendStatusChanged += this.Skype_CallVideoSendStatusChanged;
            evt.CallVideoStatusChanged += this.Skype_CallVideoStatusChanged;
            evt.ChatMembersChanged += this.Skype_ChatMembersChanged;
            evt.ChatMemberRoleChanged += this.Skype_ChatMemberRoleChanged;
            evt.Command += this.Skype_Command;
            evt.ConnectionStatus += this.Skype_ConnectionStatus;
            evt.ContactsFocused += this.Skype_ContactsFocused;
            evt.Error += this.Skype_Error;
            evt.FileTransferStatusChanged += this.Skype_FileTransferStatusChanged;
            evt.GroupDeleted += this.Skype_GroupDeleted;
            evt.GroupExpanded += this.Skype_GroupExpanded;
            evt.GroupUsers += this.Skype_GroupUsers;
            evt.GroupVisible += this.Skype_GroupVisible;
            evt.MessageHistory += this.Skype_MessageHistory;
            evt.MessageStatus += this.Skype_MessageStatus;
            evt.Mute += this.Skype_Mute;
            evt.OnlineStatus += this.Skype_OnlineStatus;
            evt.PluginEventClicked += this.Skype_PluginEventClicked;
            evt.PluginMenuItemClicked += this.Skype_PluginMenuItemClicked;
            evt.Reply += this.Skype_Reply;
            evt.SilentModeStatusChanged += this.Skype_SilentModeStatusChanged;
            evt.SmsMessageStatusChanged += this.Skype_SmsMessageStatusChanged;
            evt.SmsTargetStatusChanged += this.Skype_SmsTargetStatusChanged;
            evt.UILanguageChanged += this.Skype_UILanguageChanged;
            evt.UserAuthorizationRequestReceived += this.Skype_UserAuthorizationRequestReceived;
            evt.UserMood += this.Skype_UserMood;
            evt.UserStatus += this.Skype_UserStatus;
            evt.VoicemailStatus += this.Skype_VoicemailStatus;
            evt.WallpaperChanged += this.Skype_WallpaperChanged;
        }

        public void Connect()
        {
            this.AttachEventHandlers(this.Skype);
            this.Skype.Attach(this.Skype.Protocol, false);
        }

        public void Disconnect()
        {
        }

        public void OnMessageReceived(ChatMessage chatmessage)
        {
            // take a local copy of the event so we don't get a race condition on the next line
            var handler = this.MessageReceived;
            if (handler != null)
            {
                // extract the chat text
                if (chatmessage.Body == null)
                {
                    return;
                }
                var commandText = chatmessage.Body.Trim();
                // build the chat message and response to pass to the event handler
                var message = new SkypeMessage(chatmessage.Body, commandText);
                var response = new SkypeResponse(chatmessage.Chat);
                var args = new MessageReceivedEventArgs(message, response);
                // call the event handler
                handler(this, args);
            }
        }

        public void SendResponse(Chat chat, string message)
        {
            chat.SendMessage(message);
        }

        #endregion

    }

}
