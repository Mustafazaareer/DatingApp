using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Task.Dtos;
using Task.Entities;

namespace Task.Data;

public class Seed
{
    public static async System.Threading.Tasks.Task SeedUsers(AppDbContext context)
    {
        if (await context.Users.AnyAsync()) return;
        var membersData =await File.ReadAllTextAsync("Data/UserSeedData.json");
        var members = JsonSerializer.Deserialize<List<SeedUserDto>>(membersData);
        if (members == null)
        {
            Console.WriteLine("There Is No Members To Seed !");
            return;
        }

        foreach (var member in members)
        {
            using var hmac = new HMACSHA512();

            var user = new AppUser
            {
                Id = member.Id,
                Email = member.Email,
                ImageUrl = member.ImageUrl,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("Pa$$w0rd")),
                PasswordSalt = hmac.Key,
                Name = member.DisplayName,
                Member = new Member
                {
                    Id = member.Id,
                    DisplayName = member.DisplayName,
                    City = member.City,
                    Country = member.Country,
                    CreatedAt = member.CreatedAt,
                    DateOfBirth = member.DateOfBirth,
                    Description = member.Description,
                    Gender = member.Gender,
                    ImageUrl = member.ImageUrl,
                    LastActive = member.LastActive,
                }
            };
            user.Member.Photos.Add(new Photo
            {
                URL = member.ImageUrl,
                MemberId = member.Id
            });
            // var user = mapper.Map<AppUser>(member);
            //
            // user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("Pa$$w0rd"));
            // user.PasswordSalt = hmac.Key;
            //
            context.Users.Add(user);
        }
        
        await context.SaveChangesAsync();

    }
}