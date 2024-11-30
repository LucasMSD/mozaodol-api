﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using ProjetoTelegram.Application.DTOs.ContactDTOs;
using ProjetoTelegram.Application.DTOs.UserDTOs;
using ProjetoTelegram.Application.Interfaces.UserInterfaces;
using System.Security.Claims;

namespace ProjetoTelegram.Api.Controllers.User
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> ListAllUsers()
        {
            var getUsersResult = await _userService.GetAll();

            if (getUsersResult.IsFailed) return BadRequest(getUsersResult.Errors);
            return Ok(getUsersResult.Value);
        }

        [HttpGet("current")]
        public async Task<IActionResult> GetCurrent()
        {
            var userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
            return Ok((await _userService.Get(new ObjectId(userId))).Value);
        }


        [HttpGet("contacts")]
        public async Task<IActionResult> ListContacts()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Ok((await _userService.GetContacts(new ObjectId(userId))).Value);
        }

        [HttpPost("contacts/add")]
        public async Task<IActionResult> AddContact([FromBody] AddContactModel addContact)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Ok((await _userService.AddContact(new ObjectId(userId), addContact)).Value);
        }

        [HttpDelete("contacts/remove/{contactId}")]
        public async Task<IActionResult> AddContact([FromRoute] ObjectId contactId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Ok((await _userService.RemoveContact(new ObjectId(userId), contactId)).Value);
        }

        [HttpPut("pushToken")]
        public async Task<IActionResult> UpdatePushToken([FromBody] UpdatePushTokenModel updatePushTokenModel)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            await _userService.UpdatePushToken(new ObjectId(userId), updatePushTokenModel);
            return Ok(new { Message = "Atualizado" });
        }
    }
}
