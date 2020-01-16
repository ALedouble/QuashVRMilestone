using System.Collections.Generic;

namespace Dissonance.Networking
{
    public struct PlayerInfo
    {
        /// <summary>
        /// Name of this player
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// CodecSettings for this player
        /// </summary>
        public readonly CodecSettings CodecSettings;

        public PlayerInfo(string name, CodecSettings codecSettings)
        {
            Name = name;
            CodecSettings = codecSettings;
        }
    }

    public interface IServerAdministration
    {
        /// <summary>
        /// Get a list of all players in the session
        /// </summary>
        /// <param name="output"></param>
        void GetPlayers([NotNull] List<PlayerInfo> output);

        /// <summary>
        /// Get the codec settings for a given player
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        CodecSettings GetCodecSettings(string playerId);

        /// <summary>
        /// Get the set of channels the given player is listening to
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        void GetChannels(string playerId, [NotNull] List<string> output);


        /// <summary>
        /// Add a filter which will control who can join the session/channels in the session
        /// </summary>
        /// <param name="filter"></param>
        void AddEverntFilter([NotNull] IServerEventFilter filter);


        /// <summary>
        /// Set if a player is allowed to send voice messages
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="muted"></param>
        void Mute(string playerId, bool muted);

        /// <summary>
        /// Set if a player is allowed to listen to voice messages
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="deafened"></param>
        void Deafen(string playerId, bool deafened);

        /// <summary>
        /// Remove a player from a specific channel
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="channelId"></param>
        void Kick(string playerId, string channelId);

        /// <summary>
        /// Remove a player from the session
        /// </summary>
        /// <param name="playerId"></param>
        void Kick(string playerId);
    }

    public enum FilterResult
    {
        /// <summary>
        /// No filtering. Apply the default action if there are no other filters.
        /// </summary>
        None,

        /// <summary>
        /// Deny the event, unless another filter allows it. SoftAllow overrides SoftDeny.
        /// </summary>
        SoftDeny,

        /// <summary>
        /// Allow the event, unless another filter denies it
        /// </summary>
        SoftAllow,

        /// <summary>
        /// Immediately allow the event (do not evaluate subsequent filters).
        /// </summary>
        Allow,

        /// <summary>
        /// Immediately deny the event (do not evaluate subsequent filters).
        /// </summary>
        Deny
    }

    public interface IServerEventFilter
    {
        /// <summary>
        /// Filter players trying to join the session
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        FilterResult AllowJoinSession(string playerId, CodecSettings settings);

        /// <summary>
        /// Filter player trying to listen to a channel
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="type"></param>
        /// <param name="channelId"></param>
        /// <returns></returns>
        FilterResult AllowChannelListen(string playerId, ChannelType type, string channelId);

        /// <summary>
        /// Filter player trying to send voice to a channel
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="type"></param>
        /// <param name="channelId"></param>
        /// <returns></returns>
        FilterResult AllowChannelSendVoice(string playerId, ChannelType type, string channelId);

        /// <summary>
        /// Filter player trying to send a text message to a channel
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="type"></param>
        /// <param name="channelId"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        FilterResult AllowChannelSendText(string playerId, ChannelType type, string channelId, string message);
    }
}
