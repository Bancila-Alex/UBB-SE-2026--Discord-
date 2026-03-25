

namespace Enums
{
    public enum UserStatus
    {
        Online, // 0
        Offline, // 1
        Busy, // 2
    }

    public enum FriendStatus
    {
        Pending, // 0
        Accepted, // 1
        Blocked, // 2
    }

    public enum ConversationType
    {
        Dm, // 0
        Group, // 1
    }

    public enum ParticipantRole
    {
        Admin, // 0
        Member, // 1
        Banned, // 2
    }

    public enum MessageType
    {
        Text, // 0
        Reaction, // 1
        System, // 2
    }
}