using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatModule.Models;
using ChatModule.Repositories;
using ChatModule.src.domain.Enums;

namespace ChatModule.Services
{
    public class MemberPanelService
    {
        private readonly ParticipantRepository _participantRepo;
        private readonly UserRepository _userRepo;
        private readonly FriendRepository _friendRepo;

        public MemberPanelService(
            ParticipantRepository participantRepo,
            UserRepository userRepo,
            FriendRepository friendRepo)
        {
            _participantRepo = participantRepo;
            _userRepo = userRepo;
            _friendRepo = friendRepo;
        }

        /// <summary>
        /// Be careful when implementing / extending later, since GetMembersAsync also returns banned members, so if you want to exclude them, you need to filter them out by Role == ParticipantRole.Banned
        /// </summary>
        /// <param name="conversationId"></param>
        /// <returns></returns>
        public async Task<List<Participant>> GetMembersAsync(Guid conversationId)
        {
            return await _participantRepo.GetAllForConversationAsync(conversationId);
        }

        public async Task<List<Participant>> GetBannedMembersAsync(Guid conversationId)
        {
            var participants = await _participantRepo.GetAllForConversationAsync(conversationId);
            return participants.Where(p => p.Role == ParticipantRole.Banned).ToList();
        }

        public async Task<List<User>> SearchUsersToAddAsync(Guid conversationId, string query)
        {
            var existingParticipants = await _participantRepo.GetAllForConversationAsync(conversationId);
            var existingUserIds = existingParticipants.Select(p => p.UserId).ToHashSet();
            var users = await _userRepo.SearchByUsernameAsync(query);
            return users.Where(u => !existingUserIds.Contains(u.Id)).ToList();
        }

    }
}
