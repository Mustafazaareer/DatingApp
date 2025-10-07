using System.Net;
using System.Text.Json;
using DatingApp.Errors;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace DatingApp.Middlewares;

public class ExceptionMiddleware(RequestDelegate next,ILogger<ExceptionMiddleware> logger , IHostEnvironment env)
{
    public async System.Threading.Tasks.Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception e)
        {
            logger.LogError(e,"Message",e.Message);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var result = env.IsDevelopment()
                ? new ApiException(context.Response.StatusCode, e.Message, e.StackTrace)
                : new ApiException(context.Response.StatusCode, e.Message, "Internal Server Error !");

            var options = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            var json = JsonSerializer.Serialize(result, options);

            await context.Response.WriteAsync(json);
        }
        
    }
}