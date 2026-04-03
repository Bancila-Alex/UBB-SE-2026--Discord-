using System;
using System.Threading.Tasks;
using ChatModule.Models;
using ChatModule.Repositories;
using ChatModule.src.domain.Enums;
using ChatModule.src.domain;

namespace ChatModule.Services
{
    public class ModerationService
    {
        private readonly ParticipantRepository _participantRepository;
        private readonly MessageRepository _messageRepository;
        private readonly UserRepository _userRepository;

        public ModerationService(
            ParticipantRepository participantRepository,
            MessageRepository messageRepository,
            UserRepository userRepository)
        {
            _participantRepository = participantRepository;
            _messageRepository = messageRepository;
            _userRepository = userRepository;
        }

        public async Task BanMemberAsync(Guid conversationId, Guid adminId, Guid targetId)
        {
            await RequireAdminAsync(conversationId, adminId);
            await _participantRepository.UpdateRoleAsync(conversationId, targetId, ParticipantRole.Banned);
            var username = await ResolveUsernameAsync(targetId);
            await WriteSystemMessageAsync(conversationId, $"{username} was banned.");
        }

        public async Task UnbanMemberAsync(Guid conversationId, Guid adminId, Guid targetId)
        {
            await RequireAdminAsync(conversationId, adminId);
            await _participantRepository.UpdateRoleAsync(conversationId, targetId, ParticipantRole.Member);
            var username = await ResolveUsernameAsync(targetId);
            await WriteSystemMessageAsync(conversationId, $"{username} was unbanned.");
        }

        public async Task TimeoutMemberAsync(Guid conversationId, Guid adminId, Guid targetId, TimeSpan duration)
        {
            await RequireAdminAsync(conversationId, adminId);
            await _participantRepository.UpdateTimeoutAsync(conversationId, targetId, DateTime.UtcNow + duration);
            var username = await ResolveUsernameAsync(targetId);
            await WriteSystemMessageAsync(conversationId, $"{username} was timed out for {FormatDuration(duration)}.");
        }

        public async Task RemoveTimeoutAsync(Guid conversationId, Guid adminId, Guid targetId)
        {
            await RequireAdminAsync(conversationId, adminId);
            await _participantRepository.UpdateTimeoutAsync(conversationId, targetId, null);
            var username = await ResolveUsernameAsync(targetId);
            await WriteSystemMessageAsync(conversationId, $"Timeout removed for {username}.");
        }

        public async Task PromoteMemberAsync(Guid conversationId, Guid adminId, Guid targetId)
        {
            await RequireAdminAsync(conversationId, adminId);
            await _participantRepository.UpdateRoleAsync(conversationId, targetId, ParticipantRole.Admin);
            var username = await ResolveUsernameAsync(targetId);
            await WriteSystemMessageAsync(conversationId, $"{username} was promoted to admin.");
        }

        public async Task DemoteMemberAsync(Guid conversationId, Guid adminId, Guid targetId)
        {
            await RequireAdminAsync(conversationId, adminId);

            if (adminId == targetId)
            {
                var participants = await _participantRepository.GetAllForConversationAsync(conversationId);
                var adminCount = 0;
                foreach (var participant in participants)
                {
                    if (participant.Role == ParticipantRole.Admin)
                    {
                        adminCount++;
                    }
                }

                if (adminCount <= 1)
                {
                    throw new InvalidOperationException("You cannot demote the only admin in the group.");
                }
            }

            await _participantRepository.UpdateRoleAsync(conversationId, targetId, ParticipantRole.Member);
            var username = await ResolveUsernameAsync(targetId);
            await WriteSystemMessageAsync(conversationId, $"{username} was demoted to member.");
        }

        public async Task AddMemberAsync(Guid conversationId, Guid adminId, Guid newUserId)
        {
            await RequireAdminAsync(conversationId, adminId);

            var existing = await _participantRepository.GetAsync(conversationId, newUserId);
            if (existing != null)
            {
                throw new InvalidOperationException("User is already a participant of this conversation.");
            }

            await _participantRepository.CreateAsync(new Participant
            {
                Id = Guid.NewGuid(),
                ConversationId = conversationId,
                UserId = newUserId,
                JoinedAt = DateTime.UtcNow,
                Role = ParticipantRole.Member,
                LastReadMessageId = null,
                TimeoutUntil = null,
                IsFavourite = false
            });

            var username = await ResolveUsernameAsync(newUserId);
            await WriteSystemMessageAsync(conversationId, $"{username} was added to the group.");
        }

        private async Task<string> ResolveUsernameAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            return user?.Username ?? userId.ToString();
        }

        private async Task RequireAdminAsync(Guid conversationId, Guid userId)
        {
            var participant = await _participantRepository.GetAsync(conversationId, userId);
            if (participant == null || participant.Role != ParticipantRole.Admin)
            {
                throw new InvalidOperationException("Only admins can perform this action.");
            }
        }

        private async Task WriteSystemMessageAsync(Guid conversationId, string text)
        {
            await _messageRepository.CreateAsync(new Message
            {
                Id = Guid.NewGuid(),
                ConversationId = conversationId,
                UserId = null,
                Content = text,
                CreatedAt = DateTime.UtcNow,
                ReplyToId = null,
                IsEdited = false,
                IsDeleted = false,
                MessageType = MessageType.System,
                ParentMessageId = null
            });
        }

        private static string FormatDuration(TimeSpan duration)
        {
            var totalSeconds = Math.Max(0, (int)Math.Round(duration.TotalSeconds));
            var days = totalSeconds / 86400;
            totalSeconds %= 86400;
            var hours = totalSeconds / 3600;
            totalSeconds %= 3600;
            var minutes = totalSeconds / 60;

            if (days > 0)
            {
                return days == 1 ? "1 day" : $"{days} days";
            }

            if (hours > 0)
            {
                return minutes > 0
                    ? (hours == 1 ? $"1 hour {minutes} minutes" : $"{hours} hours {minutes} minutes")
                    : (hours == 1 ? "1 hour" : $"{hours} hours");
            }

            return minutes == 1 ? "1 minute" : $"{minutes} minutes";
        }
    }
}
