using System;
using GameStore.Api.Data;
using GameStore.Api.DTOs;
using GameStore.Api.Entities;
using GameStore.Api.Mapping;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Endpoints;

public static class GamesEndpoints
{
    const string GetGameEndpoint = "GetGameById";

    public static RouteGroupBuilder MapGamesEndpoints(this WebApplication app)
    {
        // Create a route group for the games endpoints with a base path of "games" with parameter validation
        // This allows us to group related endpoints together and apply common middleware or settings
        var group = app.MapGroup("games").WithParameterValidation(); ;

        // GET endpoint to retrieve the list of games
        group.MapGet("/", async (GameStoreContext dbContext) => await dbContext.Games
            .Include(g => g.Genre)
            .Select(g => g.ToGameSummaryDto())
            .AsNoTracking()
            .ToListAsync()
        );

        // GET endpoint to retrieve a specific game by ID
        group.MapGet("/{id}", async (int id, GameStoreContext dbContext) =>
        {
            Game? game = await dbContext.Games.FindAsync(id);

            return game is not null ? Results.Ok(game.ToGameDetailsDto()) : Results.NotFound(new { Message = $"Game with ID {id} not found." });
        }).WithName(GetGameEndpoint);

        // POST endpoint to create a new game
        group.MapPost("/", async (CreateGameDTO newGame, GameStoreContext dbContext) =>
        {
            Game game = newGame.ToEntity();

            dbContext.Games.Add(game);
            await dbContext.SaveChangesAsync();

            return Results.CreatedAtRoute(
                GetGameEndpoint,
                new { id = game.Id },
                game.ToGameDetailsDto());
        });

        //PUT endpoint to update an existing game
        group.MapPut("/{id}", async (int id, UpdateGameDTO updatedGame, GameStoreContext dbContext) =>
        {
            Game? existingGame = await dbContext.Games.FindAsync(id);

            if (existingGame is null) return Results.NotFound(new { Message = $"Game with ID {id} not found." });

            // Update the existing game with the new values
            // This is a more efficient way to update the entity in the database without having to set each property individually
            dbContext.Entry(existingGame).CurrentValues.SetValues(updatedGame.ToEntity(id));

            // Save the changes to the database asynchronously
            await dbContext.SaveChangesAsync();

            return Results.NoContent();
        });

        //DELETE endpoint to delete a game
        group.MapDelete("/{id}", async (int id, GameStoreContext dbContext) =>
        {
            //Delete the game with the specified ID
            await dbContext.Games.Where(g => g.Id == id).ExecuteDeleteAsync();

            return Results.NoContent();
        });

        return group;
    }
}


RESOURCE_GROUP=myResourceGroup
EVENT_HUB_NAMESPACE = psynceventhubdemo
EVENT_HUB_NAME=psynceventhubazure
EVENT_HUB_AUTHORIZATION_RULE = authrule
COSMOS_DB_ACCOUNT=psyncdb
STORAGE_ACCOUNT = psync
FUNCTION_APP=eventhubappfunction
LOCATION = canadacentral