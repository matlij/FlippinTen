using System;
using System.Diagnostics;
using FlippinTenWeb.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FlippinTenWeb.Controllers
{
    [Route("api/[controller]")]
    public class GamePlayController : Controller
    {
        private readonly IGameLogicLayer _gameLayer;

        public GamePlayController(IGameLogicLayer gameLayer)
        {
            _gameLayer = gameLayer;
        }

        [HttpGet]
        public IActionResult Get(string playerName)
        {
            Console.WriteLine($"{playerName} called get games.");

            try
            {
                return Ok(_gameLayer.GetGames(playerName));
            }
            catch (Exception e)
            {
                Console.WriteLine($"Get games failed. PlayerName {playerName}: {e}");

                return StatusCode(500);
            }
        }

        [HttpGet]
        [Route("{identifier}")]
        public IActionResult GetById(string identifier)
        {
            Console.WriteLine($"Get games called. Game identifier: {identifier}");

            if (identifier is null)
                return BadRequest();

            try
            {
                var game = _gameLayer.GetGame(identifier);

                if (game is null)
                {
                    return NotFound();
                }

                return Ok(game);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Get game with identifier {identifier} failed: {e}");

                return StatusCode(500);
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody]GamePlay game)
        {
            if (string.IsNullOrEmpty(game.Name) || game.Players?.Count == 0)
            {
                return BadRequest();
            }

            try
            {
                return Ok(_gameLayer.CreateGame(game));
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return StatusCode(500);
            }
        }

        [HttpPatch("identifier")]
        public IActionResult Patch(string identifier, [FromBody]JsonPatchDocument<GamePlay> patchDocument)
        {
            if (string.IsNullOrEmpty(identifier) || patchDocument is null)
            {
                return BadRequest();
            }

            try
            {
                var game = _gameLayer.GetGame(identifier);

                patchDocument.ApplyTo(game);

                _gameLayer.UpdateGame(game);

                return NoContent();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return StatusCode(500);
            }
        }

        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
