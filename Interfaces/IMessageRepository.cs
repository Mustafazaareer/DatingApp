using System.Text.RegularExpressions;
using DatingApp.Dtos.Message;
using DatingApp.Entities;
using DatingApp.Helpers;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Interfaces;

public interface IMessageRepository
{
    void AddMessage(Message message);
    void DeleteMessage(Message message);
    Task<Message?> GetMessage(string messageId);
    Task<PaginatedResult<MessageDto>> GetMessagesForMember(MessageParams messageParams);
    Task<IReadOnlyList<MessageDto>> GetMessageThread(string currentMemberId, string recipientId);
    Task<bool> SaveAllAsync();
}