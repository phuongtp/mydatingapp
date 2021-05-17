using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using API.Entities;
using API.Interfaces;
using API.DTOs;
using System.Linq;
using AutoMapper.QueryableExtensions;
using AutoMapper;

namespace API.Data
{
  public class UserRepository : IUserRepository
  {
    private readonly DataContext _context;
    private readonly IMapper _mapper;

    public UserRepository(DataContext context, IMapper mapper)
    {
      _mapper = mapper;
      _context = context;
    }

    public async Task<MemberDto> GetMemberByUsernameAsync(string username)
    {
      return await _context.Users
                  .Where(x => x.UserName == username)
                  .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
                  .SingleOrDefaultAsync();
    }

    public async Task<IEnumerable<MemberDto>> GetMembersAsync()
    {
      return await _context.Users 
              .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
              .ToListAsync();
    }

    public async Task<AppUser> GetUserByIdAsync(int id)
    {
      return await _context.Users.FindAsync(id);
    }

    public async Task<AppUser> GetUserByUsernameAsync(string username)
    {
      // If you use ProjectTo you don't have to use Include!!!
      return await _context.Users
        .Include(p => p.Photos)
        .Include(d => d.Deposits)
        .SingleOrDefaultAsync(x => x.UserName == username);
    }

    public async Task<IEnumerable<AppUser>> GetUsersAsync()
    {
    // If you use ProjectTo you don't have to use Include!!!      
      return await _context.Users
        .Include(p => p.Photos)
        .Include(d => d.Deposits)
        .ToListAsync();
    }

    public async Task<bool> SaveAllAsync()
    {
      return await _context.SaveChangesAsync() > 0;
    }

    public void Update(AppUser user)
    {
      _context.Entry(user).State = EntityState.Modified;
    }
  }
}