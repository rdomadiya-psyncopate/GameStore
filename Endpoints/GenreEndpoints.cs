using System;
using GameStore.Api.Data;
using GameStore.Api.Mapping;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Endpoints;

public static class GenreEndpoints
{
    public static RouteGroupBuilder MapGenresEndpoints(this WebApplication app)
    {
        var genres = app.MapGroup("/genres").WithParameterValidation();

        genres.MapGet("/", async (GameStoreContext dbContext) => await dbContext.Genres
                           .Select(genre => genre.ToGenreDto())
                           .AsNoTracking()
                           .ToListAsync());

        return genres;
    }
}
