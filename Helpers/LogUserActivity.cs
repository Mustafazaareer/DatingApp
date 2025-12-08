using DatingApp.Data;
using DatingApp.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Helpers;

public class LogUserActivity :IAsyncActionFilter
{
    public async  System.Threading.Tasks.Task OnActionExecutionAsync(ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        var resultContext = await next();
        if (context.HttpContext.User.Identity != null && context.HttpContext.User.Identity.IsAuthenticated != true) return;
        var memberId = resultContext.HttpContext.User.GetMemberId();
        var dbContext = resultContext.HttpContext.RequestServices.GetRequiredService<AppDbContext>();
        await dbContext.Members
            .Where(m => m.Id == memberId)
            .ExecuteUpdateAsync(s => s.SetProperty(p => p.LastActive, DateTime.UtcNow));

    }
}