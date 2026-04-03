using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using ChatModule.Models;
using ChatModule.src.domain.Enums;
using Microsoft.Data.SqlClient;

namespace ChatModule.Repositories
{
    public class ParticipantRepository
    {
        private readonly DatabaseManager _db;

        public ParticipantRepository(DatabaseManager db)
        {
            _db = db;
        }

        public async Task<Participant?> GetAsync(Guid conversationId, Guid userId)
        {
            await using var connection = new SqlConnection(_db.ConnectionString);
            await connection.OpenAsync();

            const string sql = @"
SELECT TOP 1 Id, ConversationId, UserId, JoinedAt, Role, LastReadMessageId, TimeoutUntil, IsFavourite, Nickname
FROM Participants
WHERE ConversationId = @ConversationId AND UserId = @UserId;";

            await using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@ConversationId", conversationId);
            command.Parameters.AddWithValue("@UserId", userId);

            await using var reader = await command.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
            {
                return null;
            }

            return MapParticipant(reader);
        }

        public async Task<List<Participant>> GetAllForConversationAsync(Guid conversationId)
        {
            var participants = new List<Participant>();

            await using var connection = new SqlConnection(_db.ConnectionString);
            await connection.OpenAsync();

            const string sql = @"
SELECT Id, ConversationId, UserId, JoinedAt, Role, LastReadMessageId, TimeoutUntil, IsFavourite, Nickname
FROM Participants
WHERE ConversationId = @ConversationId;";

            await using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@ConversationId", conversationId);

            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                participants.Add(MapParticipant(reader));
            }

            return participants;
        }

        public async Task<List<Participant>> GetAllForUserAsync(Guid userId)
        {
            var participants = new List<Participant>();

            await using var connection = new SqlConnection(_db.ConnectionString);
            await connection.OpenAsync();

            const string sql = @"
SELECT Id, ConversationId, UserId, JoinedAt, Role, LastReadMessageId, TimeoutUntil, IsFavourite, Nickname
FROM Participants
WHERE UserId = @UserId;";

            await using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@UserId", userId);

            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                participants.Add(MapParticipant(reader));
            }

            return participants;
        }

        public async Task CreateAsync(Participant participant)
        {
            await using var connection = new SqlConnection(_db.ConnectionString);
            await connection.OpenAsync();

            const string sql = @"
INSERT INTO Participants
    (Id, ConversationId, UserId, JoinedAt, Role, LastReadMessageId, TimeoutUntil, IsFavourite, Nickname)
VALUES
    (@Id, @ConversationId, @UserId, @JoinedAt, @Role, @LastReadMessageId, @TimeoutUntil, @IsFavourite, @Nickname);";

            await using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Id", participant.Id);
            command.Parameters.AddWithValue("@ConversationId", participant.ConversationId);
            command.Parameters.AddWithValue("@UserId", participant.UserId);
            command.Parameters.AddWithValue("@JoinedAt", participant.JoinedAt);
            command.Parameters.AddWithValue("@Role", (int)participant.Role);
            command.Parameters.AddWithValue("@LastReadMessageId", (object?)participant.LastReadMessageId ?? DBNull.Value);
            command.Parameters.AddWithValue("@TimeoutUntil", (object?)participant.TimeoutUntil ?? DBNull.Value);
            command.Parameters.AddWithValue("@IsFavourite", participant.IsFavourite);
            command.Parameters.AddWithValue("@Nickname", (object?)participant.Nickname ?? DBNull.Value);

            await command.ExecuteNonQueryAsync();
        }

        public async Task UpdateRoleAsync(Guid conversationId, Guid userId, ParticipantRole role)
        {
            await using var connection = new SqlConnection(_db.ConnectionString);
            await connection.OpenAsync();

            const string sql = @"
UPDATE Participants
SET Role = @Role
WHERE ConversationId = @ConversationId AND UserId = @UserId;";

            await using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Role", (int)role);
            command.Parameters.AddWithValue("@ConversationId", conversationId);
            command.Parameters.AddWithValue("@UserId", userId);

            await command.ExecuteNonQueryAsync();
        }

        public async Task UpdateLastReadAsync(Guid conversationId, Guid userId, Guid messageId)
        {
            await using var connection = new SqlConnection(_db.ConnectionString);
            await connection.OpenAsync();

            const string sql = @"
UPDATE Participants
SET LastReadMessageId = @MessageId
WHERE ConversationId = @ConversationId AND UserId = @UserId;";

            await using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@MessageId", messageId);
            command.Parameters.AddWithValue("@ConversationId", conversationId);
            command.Parameters.AddWithValue("@UserId", userId);

            await command.ExecuteNonQueryAsync();
        }

        public async Task UpdateTimeoutAsync(Guid conversationId, Guid userId, DateTime? until)
        {
            await using var connection = new SqlConnection(_db.ConnectionString);
            await connection.OpenAsync();

            const string sql = @"
UPDATE Participants
SET TimeoutUntil = @Until
WHERE ConversationId = @ConversationId AND UserId = @UserId;";

            await using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Until", (object?)until ?? DBNull.Value);
            command.Parameters.AddWithValue("@ConversationId", conversationId);
            command.Parameters.AddWithValue("@UserId", userId);

            await command.ExecuteNonQueryAsync();
        }

        public async Task UpdateFavouriteAsync(Guid conversationId, Guid userId, bool isFav)
        {
            await using var connection = new SqlConnection(_db.ConnectionString);
            await connection.OpenAsync();

            const string sql = @"
UPDATE Participants
SET IsFavourite = @IsFav
WHERE ConversationId = @ConversationId AND UserId = @UserId;";

            await using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@IsFav", isFav);
            command.Parameters.AddWithValue("@ConversationId", conversationId);
            command.Parameters.AddWithValue("@UserId", userId);

            await command.ExecuteNonQueryAsync();
        }

        public async Task DeleteAsync(Guid conversationId, Guid userId)
        {
            await using var connection = new SqlConnection(_db.ConnectionString);
            await connection.OpenAsync();

            const string sql = @"
DELETE FROM Participants
WHERE ConversationId = @ConversationId AND UserId = @UserId;";

            await using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@ConversationId", conversationId);
            command.Parameters.AddWithValue("@UserId", userId);

            await command.ExecuteNonQueryAsync();
        }

        public async Task UpdateNicknameAsync(Guid conversationId, Guid userId, string? nickname)
        {
            await using var connection = new SqlConnection(_db.ConnectionString);
            await connection.OpenAsync();

            const string sql = @"
UPDATE Participants
SET Nickname = @Nickname
WHERE ConversationId = @ConversationId AND UserId = @UserId;";

            await using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Nickname", (object?)nickname ?? DBNull.Value);
            command.Parameters.AddWithValue("@ConversationId", conversationId);
            command.Parameters.AddWithValue("@UserId", userId);

            await command.ExecuteNonQueryAsync();
        }

        private static Participant MapParticipant(SqlDataReader reader)
        {
            var idOrdinal = reader.GetOrdinal("Id");
            var conversationIdOrdinal = reader.GetOrdinal("ConversationId");
            var userIdOrdinal = reader.GetOrdinal("UserId");
            var joinedAtOrdinal = reader.GetOrdinal("JoinedAt");
            var roleOrdinal = reader.GetOrdinal("Role");
            var lastReadMessageIdOrdinal = reader.GetOrdinal("LastReadMessageId");
            var timeoutUntilOrdinal = reader.GetOrdinal("TimeoutUntil");
            var isFavouriteOrdinal = reader.GetOrdinal("IsFavourite");
            var nicknameOrdinal = reader.GetOrdinal("Nickname");

            return new Participant
            {
                Id = reader.GetGuid(idOrdinal),
                ConversationId = reader.GetGuid(conversationIdOrdinal),
                UserId = reader.GetGuid(userIdOrdinal),
                JoinedAt = reader.GetDateTime(joinedAtOrdinal),
                Role = (ParticipantRole)reader.GetByte(roleOrdinal),
                LastReadMessageId = reader.IsDBNull(lastReadMessageIdOrdinal) ? null : reader.GetGuid(lastReadMessageIdOrdinal),
                TimeoutUntil = reader.IsDBNull(timeoutUntilOrdinal) ? null : reader.GetDateTime(timeoutUntilOrdinal),
                IsFavourite = reader.GetBoolean(isFavouriteOrdinal),
                Nickname = reader.IsDBNull(nicknameOrdinal) ? null : reader.GetString(nicknameOrdinal),
            };
        }
    }
}
