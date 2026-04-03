using System;
using ChatModule.src.domain.Enums;
using Microsoft.UI.Xaml.Media;
using Windows.UI;

namespace ChatModule.src.view_models
{
    public class MemberDisplayItem
    {
        public Guid UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
        public UserStatus Status { get; set; }
        public ParticipantRole Role { get; set; }
        public bool HasTimeout { get; set; }
        public DateTime? TimeoutUntil { get; set; }

        public string? TimeoutRemainingLabel
        {
            get
            {
                if (!TimeoutUntil.HasValue || TimeoutUntil.Value <= DateTime.UtcNow)
                    return null;
                var remaining = TimeoutUntil.Value - DateTime.UtcNow;
                if (remaining.TotalDays >= 1)
                    return $"⏱ {(int)remaining.TotalDays}d {remaining.Hours}h left";
                if (remaining.TotalHours >= 1)
                    return $"⏱ {(int)remaining.TotalHours}h {remaining.Minutes}m left";
                return $"⏱ {remaining.Minutes}m left";
            }
        }

        public string RoleLabel => Role switch
        {
            ParticipantRole.Admin => "Admin",
            ParticipantRole.Banned => "Banned",
            _ => "Member"
        };

        public bool IsMemberRole => Role == ParticipantRole.Member;
        public bool IsAdminRole => Role == ParticipantRole.Admin;

        public SolidColorBrush GetStatusBrush() => Status switch
        {
            UserStatus.Online => new SolidColorBrush(Color.FromArgb(255, 67, 181, 129)),
            UserStatus.Busy => new SolidColorBrush(Color.FromArgb(255, 250, 166, 26)),
            _ => new SolidColorBrush(Color.FromArgb(255, 116, 127, 141))
        };
    }
}
