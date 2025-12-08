namespace DatingApp.Dtos.Message;

public class CreateMessageDto
{
    public required string Content { get; set; }
    public required string RecipientId { get; set; }

}