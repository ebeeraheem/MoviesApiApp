using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Data.SqlClient;
using MoviesLibrary.Models;
using MoviesLibrary.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton(serviceProvider =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var connection = configuration.GetConnectionString("DefaultConnection");

    return new SqlConnectionFactory(connection);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var movies = app.MapGroup("api/movies");

movies.MapGet("/", GetMovies).Produces(StatusCodes.Status200OK);
movies.MapGet("/{id}", GetMovie)
    .Produces(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound);

movies.MapPost("/", CreateMovie)
    .Produces(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status400BadRequest);

movies.MapPut("/{id}", UpdateMovie)
    .Produces(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status400BadRequest)
    .Produces(StatusCodes.Status404NotFound);

movies.MapDelete("/{id}", DeleteMovie)
    .Produces(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status404NotFound);

static async Task<IResult> GetMovies(SqlConnectionFactory sqlConnectionFactory)
{
    var movies = await MovieServices.GetAllMovies(sqlConnectionFactory);
    return Results.Ok(movies);
}
static async Task<IResult> GetMovie(int id, SqlConnectionFactory sqlConnectionFactory)
{
    var movie = await MovieServices.GetMovieById(id, sqlConnectionFactory);
    return movie is not null ? Results.Ok(movie) : Results.NotFound();
}
static async Task<IResult> CreateMovie(MovieDto movieDto, SqlConnectionFactory sqlConnectionFactory)
{
    bool isCreated = await MovieServices.Add(movieDto, sqlConnectionFactory);
    return isCreated is false ? 
        Results.BadRequest("Title, ReleaseDate, and Director are required.") : 
        Results.Ok();
}
static async Task<IResult> UpdateMovie(int id, Movie movie, SqlConnectionFactory sqlConnectionFactory)
{
    bool? isUpdated = await MovieServices.Update(id, movie, sqlConnectionFactory);
    return isUpdated is null ? Results.BadRequest() : isUpdated is false ? Results.NotFound() : Results.NoContent();
}
static async Task<IResult> DeleteMovie(int id, SqlConnectionFactory sqlConnectionFactory)
{
    bool isDeleted = await MovieServices.Delete(id, sqlConnectionFactory);
    return isDeleted is false ? Results.NotFound() : Results.NoContent();
}

app.Run();

