using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
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
        private readonly IUserService userService;

        private readonly IMessageService messageService;

        private readonly IMapper mapper;

        public MessagesController(IUserService userService, IMessageService messageService,
            IMapper mapper)
        {
            this.mapper = mapper;
            this.messageService = messageService;
            this.userService = userService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessagesForUser([FromQuery]
            MessageParams messageParams)
        {
            messageParams.Username = User.GetUserName();

            var messages = await this.messageService.GetMessagesForUser(messageParams);

            Response.AddPaginationHeader(messages.CurrentPage, messages.PageSize,
                messages.TotalCount, messages.TotalPages);

            return messages;
        }

        [HttpGet("thread/{username}")]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread(string username)
        {
            var currentUsername = User.GetUserName();

            return this.Ok(await this.messageService.GetMessageThread(currentUsername, username));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMessage(int id)
        {
            var username = User.GetUserName();

            var message = await this.messageService.GetMessage(id);

            if (message.Sender.UserName != username && message.Recipient.UserName != username)
            {
                return this.Unauthorized();
            }

            if (message.Sender.UserName == username)
            {
                message.SenderDeleted = true;
            }

            if (message.Recipient.UserName == username)
            {
                message.RecipientDeleted = true;
            }

            if (message.SenderDeleted && message.RecipientDeleted)
            { 
                this.messageService.DeleteMessage(message);
            }

            if (await this.messageService.SaveAllAsync())
            {
                return this.Ok();
            }

            return this.BadRequest("Problem deleting the message");
        }
    }
}