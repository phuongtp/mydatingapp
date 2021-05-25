using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using API.Entities;
using API.Interfaces;
using API.DTOs;
using System.Linq;
using AutoMapper.QueryableExtensions;
using AutoMapper;
using API.Helpers;
using System;

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

    public async Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams)
    {
      var query = _context.Users.AsQueryable();                    

                      // .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
                      // .AsNoTracking()
                      // .AsQueryable();
      // Filter: User owner and the Gender of obersit
      query = query.Where(u => u.UserName != userParams.CurrentUsername);
      query = query.Where(u => u.Gender == userParams.Gender);

      var minDob = DateTime.Today.AddYears(-userParams.MaxAge - 1);
      var maxDob = DateTime.Today.AddYears(-userParams.MinAge);

      // Filter: Age ; between 18 and 30.
      query = query.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);

      // Sort Order
      query = userParams.OrderBy switch
      {
        "created" => query.OrderByDescending(u=>u.Created),
        _ => query.OrderByDescending(u=>u.LastActive)
      };      
      // query = userParams.OrderBy switch
      // {
      //   "created" => query.OrderByDescending(u => u.Created),
      //     _ => query.OrderByDescending(u => u.LastActive)
      // };

      return await PagedList<MemberDto>.CreateAsync(query.ProjectTo<MemberDto>(_mapper.ConfigurationProvider).AsNoTracking(), 
          userParams.PageNumber, userParams.PageSize);
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

    public async Task<string> GetUserGender(string username)
    {
      return await _context.Users
          .Where(x => x.UserName == username)
          .Select(x => x.Gender).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<AppUser>> GetUsersAsync()
    {
    // If you use ProjectTo you don't have to use Include!!!      
      return await _context.Users
        .Include(p => p.Photos)
        .Include(d => d.Deposits)
        .ToListAsync();
    }

    // public async Task<bool> SaveAllAsync()
    // {
    //   return await _context.SaveChangesAsync() > 0;
    // }

    public void Update(AppUser user)
    {
      _context.Entry(user).State = EntityState.Modified;
    }
  }
}