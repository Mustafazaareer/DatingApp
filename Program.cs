using System.Text;
using API.Data;
using AutoMapper;
using DatingApp.Data;
using DatingApp.Entities;
using DatingApp.Helpers;
using DatingApp.Interfaces;
using DatingApp.Middlewares;
using DatingApp.Profiles;
using DatingApp.Services;
using DatingApp.SignalR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(opt =>
    {
        opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
    });
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
        options.JsonSerializerOptions.WriteIndented = true;
    });
builder.Services.AddScoped<ITokenService,TokenService>();
builder.Services.AddScoped<IPhotoService,PhotoService>();
builder.Services.AddScoped<IMemberRepository,MemberRepository>();
builder.Services.AddScoped<IMessageRepository,MessageRepository>();
builder.Services.AddScoped<ILikesRepository,LikesRepository>();
builder.Services.AddAutoMapper(typeof(MappingProfiles));
builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));
builder.Services.AddScoped<LogUserActivity>();
builder.Services.AddSingleton<ChatTracker>();
builder.Services.AddIdentityCore<AppUser>(opt =>
{
    opt.Password.RequireNonAlphanumeric = false;
    opt.User.RequireUniqueEmail = true;
}).AddRoles<IdentityRole>()
.AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddSignalR();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var tokenKey = builder.Configuration["TokenKey"] ?? throw new Exception("Token Key Not Found!");
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;

                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    });
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"))
    .AddPolicy("ModeratePhotoRole", policy => policy.RequireRole("Admin","Moderator"));

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

// Configure the HTTP request pipeline.
app.UseCors(x => x.AllowCredentials().AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:5434","https://localhost:5434"));

app.UseAuthentication();
app.UseAuthorization();

app.MapHub<ChatHub>("hubs/presence");
using var scope = app.Services.CreateScope();

var services = scope.ServiceProvider;

// try
// {
//     var userManager = services.GetRequiredService<
//         UserManager<AppUser>>();
//
//     await Seed.SeedUsers(userManager);
// }
// catch (Exception e)
// {
//     Console.WriteLine($"{e},Error occurred when Migration");
//     throw;
// }
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.MapControllers();

app.Run();
 