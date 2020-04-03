﻿using System;
using FlippinTenWebApi.DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models.Entities;
using Newtonsoft.Json;

namespace FlippinTenWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GameController : Controller
    {
        private readonly IGameRepository _gameRepository;
        private readonly ILogger _log;

        public GameController(IGameRepository gameRepository, ILogger<GameController> log)
        {
            _gameRepository = gameRepository;
            _log = log;
        }

        [HttpGet]
        public IActionResult Get(string playerName)
        {
            try
            {
                return Ok(_gameRepository.GetFromPlayer(playerName));
            }
            catch (Exception e)
            {
                _log.LogError(e, $"Get games failed.", playerName);

                return StatusCode(500);
            }
        }

        [HttpGet]
        [Route("{identifier}")]
        public IActionResult GetById(string identifier)
        {
            if (identifier is null)
                return BadRequest();

            try
            {
                var game = _gameRepository.Get(identifier);

                if (game is null)
                {
                    return NotFound();
                }

                return Ok(game);
            }
            catch (Exception e)
            {
                _log.LogError(e, $"Get game by ID failed.", identifier);

                return StatusCode(500);
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody]CardGame game)
        {
            _log.LogInformation($"Create game called: {JsonConvert.SerializeObject(game)}");

            if (string.IsNullOrEmpty(game.Name) || game.Players?.Count == 0)
            {
                return BadRequest();
            }

            try
            {
                var result = _gameRepository.Store(game);
                if (!result)
                {
                    _log.LogError($"Create game failed: {JsonConvert.SerializeObject(game)}", game.Identifier);

                    return StatusCode(500);
                }

                return Created(string.Empty, game);
            }
            catch (Exception e)
            {
                _log.LogError(e, $"Create game failed: {JsonConvert.SerializeObject(game)}", game.Identifier);

                return StatusCode(500);
            }
        }
    }
}
