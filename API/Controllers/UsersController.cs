using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class UsersController : ControllerBase
  {
    private readonly DataContext _context;
    public UsersController(DataContext context)
    {
      _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
    {
        // We have to watch until this Get action have to finished
        return await _context.Users.ToListAsync();
    }

    [HttpGet("{id}")]
    public ActionResult<AppUser> GetUser(int id)
    {
        return _context.Users.Find(id);
    }    
  }
}