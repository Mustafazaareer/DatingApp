using System.Linq.Expressions;
using CloudinaryDotNet;
using DatingApp.Dtos.Message;
using DatingApp.Entities;

namespace DatingApp.Extensions;

public static class MessageExtensions
{
    public static MessageDto ToDto(this Message message)
    {
        return new MessageDto
        {
            Id = message.Id,
            Content = message.Content,
            DateRead = message.DateRead,
            SenderId = message.SenderId,
            RecipientId = message.RecipientId,
            SenderImageUrl = message.Sender.ImageUrl!,
            RecipientImageUrl = message.Recipient.ImageUrl!,
            SenderDisplayName = message.Sender.DisplayName!,
            RecipientDisplayName = message.Recipient.DisplayName!,
            MessageSent = message.MessageSent
        };
    }

    public static Expression<Func<Message,MessageDto>> ToDtoProjection()
    {
        return message=> new MessageDto
        {
            Id = message.Id,
            Content = message.Content,
            DateRead = message.DateRead,
            SenderId = message.SenderId,
            RecipientId = message.RecipientId,
            SenderImageUrl = message.Sender.ImageUrl!,
            RecipientImageUrl = message.Recipient.ImageUrl!,
            SenderDisplayName = message.Sender.DisplayName!,
            RecipientDisplayName = message.Recipient.DisplayName!,
            MessageSent = message.MessageSent
        };
    }
}