using DatingApp.Dtos.Message;
using DatingApp.Entities;
using DatingApp.Extensions;
using DatingApp.Helpers;
using DatingApp.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.Controllers;

public class MessagesController(IMessageRepository messageRepository, IMemberRepository memberRepository) : BaseController
{
    [HttpPost]
    public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
    {
        var sender = await memberRepository.GetMemberByIdAsync(User.GetMemberId());
        var recipient = await memberRepository.GetMemberByIdAsync(createMessageDto.RecipientId);

        if (recipient == null || sender == null || sender.Id == recipient.Id)
        {
            return BadRequest("Cant Send This Message !");
        }

        var message = new Message
        {
            RecipientId = recipient.Id,
            SenderId = sender.Id,
            Content = createMessageDto.Content
        };
        messageRepository.AddMessage(message);

        if (await messageRepository.SaveAllAsync()) return message.ToDto();
        return BadRequest("Failed To Send Message");
        
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedResult<MessageDto>>> GetMessagesByContainer([FromQuery] MessageParams messageParams)
    {
        messageParams.MemberId = User.GetMemberId();
        return await messageRepository.GetMessagesForMember(messageParams);
    }
    [HttpGet("thread/recipientId")]
    
    public async Task<ActionResult<IReadOnlyList<MessageDto>>> GetMessageThread(string recipientId)
    {
        return Ok(await messageRepository.GetMessageThread(User.GetMemberId(), recipientId));
    }
    [HttpDelete("{id}")]

    public async Task<ActionResult> DeleteMessage(string id)
    {
        var memberId = User.GetMemberId();
        var message = await messageRepository.GetMessage(id);
        if (message == null) return BadRequest("Cannot Delete This Message!");
    
        if(message.SenderId != memberId && message.RecipientId != memberId)
            return BadRequest("Cannot Delete This Message!");
        
        if (message.SenderId == memberId) message.SenderDeleted = true;
        if (message.RecipientId == memberId) message.RecipientDeleted = true;
        
        if(message is {SenderDeleted:true,RecipientDeleted:true})messageRepository.DeleteMessage(message);

        if (await memberRepository.SaveAllAsync()) return Ok();
        return BadRequest("Problem When Delete Message");

    }
    
}