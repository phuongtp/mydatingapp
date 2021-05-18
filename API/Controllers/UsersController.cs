using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
  [Authorize]
  public class UsersController : BaseApiController
  {
    private readonly IUserRepository _userRepository;
    public UsersController(IUserRepository userRepository)
    {
      _userRepository = userRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
    {
      // We have to watch until this Get action have to finished
      var users = await _userRepository.GetMembersAsync();
      // var usersToReturn = _mapper.Map<IEnumerable<MemberDto>>(users);
      return Ok(users);
    }

    [Authorize]
    [HttpGet("{username}")]
    public async Task<ActionResult<MemberDto>> GetUser(string username)
    {
      var user = await _userRepository.GetMemberByUsernameAsync(username);
      // var userToReturn = _mapper.Map<MemberDto>(user);
      return Ok(user);
    }
  }
}