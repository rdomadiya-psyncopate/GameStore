
using GameStore.Api.Data;
using GameStore.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container. Making connection string available to the app using DBcontext
//Added connection string to appsettings.json
var connString = builder.Configuration.GetConnectionString("GameStore");

//Dependency Injection for DbContext
builder.Services.AddSqlite<GameStoreContext>(connString);

var app = builder.Build();

app.MapGamesEndpoints();
app.MapGenresEndpoints();

await app.MigrateDbAsync();

app.Run();
