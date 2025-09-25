using Microsoft.EntityFrameworkCore;
using Task.Entities;
using Task.Interfaces;

namespace Task.Data;

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
 
        var member = await _context.Members
            .Include(m=>m.AppUser)
            .Include(m => m.Photos)
            .SingleOrDefaultAsync(m => m.Id == id);
        return member;
    }

    public async Task<bool> SaveAllAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<IReadOnlyList<Member>> GetAllMembersAsync()
    {
        var members = await _context.Members
            .Include(m => m.Photos)
            .ToListAsync();
        return members;
    }

    public async Task<Member?> GetMemberByIdAsync(string id)
    {
        return await _context.Members.FindAsync(id);
    }

    public async Task<IReadOnlyList<Photo>> GetAllMemberPhotosAsync(string memberId)
    {
        return await _context.Members.Where(m => m.Id == memberId).SelectMany(x => x.Photos).ToListAsync();
    }
}