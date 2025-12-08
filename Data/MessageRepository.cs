using System.Text.RegularExpressions;
using DatingApp.Dtos.Message;
using DatingApp.Entities;
using DatingApp.Extensions;
using DatingApp.Helpers;
using DatingApp.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Data;

public class MessageRepository(AppDbContext context) : IMessageRepository
{
    public void AddMessage(Message message)
    {
        context.Messages.Add(message);
    }

    public void DeleteMessage(Message message)
    {
        context.Messages.Remove(message);
    }

    public async Task<Message?> GetMessage(string messageId)
    {
        return await context.Messages.FindAsync(messageId);
    }

    public async Task<PaginatedResult<MessageDto>> GetMessagesForMember(MessageParams messageParams)
    {
        var query = context.Messages.
                    OrderByDescending(x => x.Id)
                    .AsQueryable();
        query = messageParams.Container switch
        {
            "Outbox" => query.Where(m => m.SenderId == messageParams.MemberId && m.SenderDeleted == false),
            _ => query.Where(m => m.RecipientId == messageParams.MemberId && m.RecipientDeleted == false)
        };

        var messageQuery = query.Select(MessageExtensions.ToDtoProjection());
        return await PaginationHelper.CreateAsync(messageQuery, messageParams.PageNumber, messageParams.PageSize);
    }

    public async Task<IReadOnlyList<MessageDto>> GetMessageThread(string currentMemberId, string recipientId)
    {
        await context.Messages.Where(x =>
                x.RecipientId == currentMemberId && x.SenderId == recipientId && x.DateRead == null )
            .ExecuteUpdateAsync(setter => setter.SetProperty(x => x.DateRead, DateTime.UtcNow));
        return await context.Messages
            .Where(x => (x.RecipientId == currentMemberId && x.SenderId == recipientId && x.SenderDeleted == false) ||
                        (x.SenderId == currentMemberId && x.RecipientId == recipientId && x.RecipientDeleted == false)).OrderBy(x => x.MessageSent)
            .Select(MessageExtensions.ToDtoProjection()).ToListAsync();
    }

    public async Task<bool> SaveAllAsync()
    {
        return await context.SaveChangesAsync() > 0;
    }

 }