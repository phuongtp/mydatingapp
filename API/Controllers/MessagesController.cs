using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
  [Authorize]
  public class MessagesController : BaseApiController
  {
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    public MessagesController(IUnitOfWork unitOfWork, IMapper mapper)
    {
      _mapper = mapper;
      _unitOfWork = unitOfWork;
    }

    [HttpPost]
    public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
    {
      var username = User.GetUsername();

      if (username == createMessageDto.RecipientUsername.ToLower())
        return BadRequest("You cannot send messages to yourself");

      var sender = await _unitOfWork.UserRepository.GetUserByUsernameAsync(username);
      var recipient = await _unitOfWork.UserRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername);

      if (recipient == null) return NotFound();

      var message = new Message
      {
        Sender = sender,
        Recipient = recipient,
        SenderUsername = sender.UserName,
        RecipientUsername = recipient.UserName,
        Content = createMessageDto.Content
      };

      _unitOfWork.MessageRepository.AddMessage(message);

      if (await _unitOfWork.Complete()) return Ok(_mapper.Map<MessageDto>(message));

      return BadRequest("Failed to send message");
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageForUser([FromQuery]MessageParams messageParams)
    {
        messageParams.Username = User.GetUsername();

        var messages = await _unitOfWork.MessageRepository.GetMessagesForUser(messageParams);

        Response.AddPaginationHeader(messages.CurrentPage, messages.PageSize, 
                    messages.TotalCount, messages.TotalPages);

        return messages;
    }

    [HttpGet("thread/{username}")]
    public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread(string username)
    {
      var currentUsername = User.GetUsername();

      return Ok(await _unitOfWork.MessageRepository.GetMessageThread(currentUsername, username));

    }
  }
}