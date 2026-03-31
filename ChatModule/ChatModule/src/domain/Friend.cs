using System;
using ChatModule.src.domain.Enums;

namespace ChatModule.Models
{
    public class Friend
    {
        public Guid Id { get; set; }
        public Guid UserId1 { get; set; }
        public Guid UserId2 { get; set; }
        public FriendStatus Status { get; set; }
        public bool IsMatch { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
