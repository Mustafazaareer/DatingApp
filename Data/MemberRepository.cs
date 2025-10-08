using DatingApp.Entities;
using DatingApp.Helpers;
using DatingApp.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Data;

public class MemberRepository : IMemberRepository
{
    private readonly AppDbContext _context;
    
    public MemberRepository(AppDbContext context)
    {
        _context = context; // assign the injected context to the field
    }
    public void Update(Member member)
    {
        _context.Entry(member).State = EntityState.Modified;
    }

    public async Task<Member?> GetMemberForUpdate(string id)
    {
        return await _context.Members
            .Include(x => x.AppUser)
            .SingleOrDefaultAsync(x => x.Id == id);
    }

    
    

public async Task<Member?> GetMemberForMemberUpdate(string id)
{
    return await _context.Members
        .Include(m => m.AppUser)
        .SingleOrDefaultAsync(x => x.Id == id);
}

    public async Task<bool> SaveAllAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
    
    public async Task<PaginatedResult<Member>>  GetMembersAsync(MemberParams memberParams)
    {
        var query = _context.Members.AsQueryable();
        query = query.Where(m => m.Id != memberParams.CurrentMemberId);
        if (memberParams.Gender != null)
        {
            query = query.Where(m => m.Gender == memberParams.Gender);
        
        }
        
        var minDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-memberParams.MaxAge - 1));
        var maxDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-memberParams.MinAge ));
        query = query.Where(x => x.DateOfBirth >= minDob && x.DateOfBirth <= maxDob);

        
        query = memberParams.OrderBy switch
        {
            "created" => query.OrderByDescending(x => x.CreatedAt),
            _ => query.OrderByDescending(x => x.LastActive)
        };
        return await PaginationHelper.CreateAsync(query,memberParams.PageNumber,memberParams.PageSize);
    }

    public async Task<Member?> GetMemberByIdAsync(string id)
    {
        return await _context.Members.FindAsync(id);
    }

    public async Task<IReadOnlyList<Photo>> GetPhotosForMemberAsync(string memberId)
    {
        return await _context.Members
            .Where(x => x.Id == memberId)
            .SelectMany(x => x.Photos)
            .ToListAsync();
    }
}