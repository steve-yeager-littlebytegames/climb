﻿using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Climb.Attributes;
using Climb.Data;
using Climb.Requests.Tournaments;
using Climb.Responses.Models;
using Climb.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Climb.API
{
    [Route("api/v1/tournaments")]
    public class TournamentApi : BaseApi<TournamentApi>
    {
        private readonly ITournamentService tournamentService;
        private readonly ApplicationDbContext dbContext;

        public TournamentApi(ILogger<TournamentApi> logger, ITournamentService tournamentService, ApplicationDbContext dbContext)
            : base(logger)
        {
            this.tournamentService = tournamentService;
            this.dbContext = dbContext;
        }

        [HttpGet("{tournamentID:int}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(TournamentDto))]
        [SwaggerResponse(HttpStatusCode.NotFound, null)]
        public async Task<IActionResult> Get(int tournamentID)
        {
            var tournament = await dbContext.Tournaments
                .Include(t => t.TournamentUsers).ThenInclude(tu => tu.LeagueUser).AsNoTracking()
                .FirstOrDefaultAsync(t => t.ID == tournamentID);
            if(tournament == null)
            {
                return NotFound();
            }

            var dto = new TournamentDto(tournament);
            return Ok(dto);
        }

        [HttpGet("")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(TournamentDto[]))]
        public async Task<IActionResult> GetAll()
        {
            var tournaments = await dbContext.Tournaments
                .Select(t => new TournamentDto(t))
                .ToArrayAsync();

            return Ok(tournaments);
        }

        [HttpPost("")]
        [SwaggerResponse(HttpStatusCode.Created, typeof(TournamentDto))]
        public async Task<IActionResult> Post([FromBody] CreateRequest request)
        {
            try
            {
                var tournament = await tournamentService.Create(request.LeagueID, request.SeasonID, request.Name);

                var dto = new TournamentDto(tournament);
                return CreatedAtAction("Home", "Tournament", null, dto);
            }
            catch(Exception exception)
            {
                return GetExceptionResult(exception, request);
            }
        }

        [HttpPost("leave")]
        [SwaggerResponse(HttpStatusCode.OK, null)]
        public async Task<IActionResult> Leave(int competitorID)
        {
            try
            {
                await tournamentService.Leave(competitorID);
                return Ok();
            }
            catch(Exception exception)
            {
                return GetExceptionResult(exception, new {competitorID});
            }
        }

        [HttpPost("start/{id:int}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(TournamentDto))]
        public async Task<IActionResult> Start(int id)
        {
            try
            {
                await tournamentService.Start(id);
                return Ok();
            }
            catch(Exception exception)
            {
                return GetExceptionResult(exception, new {id});
            }
        }
    }
}