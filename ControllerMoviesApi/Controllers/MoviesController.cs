using MoviesLibrary.Models;
using MoviesLibrary.Services;
using Microsoft.AspNetCore.Mvc;
using Dapper;


namespace MoviesLibrary.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MoviesController : ControllerBase
{
    // GET: api/Movies
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IResult> /*ActionResult<IEnumerable<Movie>>*/ Get(SqlConnectionFactory sqlConnectionFactory)
    {
        var movies = await MovieServices.GetAllMovies(sqlConnectionFactory);
        return Results.Ok(movies);
    }

    // GET api/Movies/5
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> Get(int id, SqlConnectionFactory sqlConnectionFactory)
    {
        var movie = await MovieServices.GetMovieById(id, sqlConnectionFactory);
        return movie is not null ? Results.Ok(movie) : Results.NotFound();
    }

    // POST api/Movies
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IResult> Post([FromBody] MovieDto movieDto, SqlConnectionFactory sqlConnectionFactory)
    {
        bool isCreated = await MovieServices.Add(movieDto, sqlConnectionFactory);
        return isCreated is false ?
            Results.BadRequest("Title, ReleaseDate, and Director are required.") :
            Results.Ok();
    }

    // PUT api/Movies/5
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> Put(int id, [FromBody] Movie movie, SqlConnectionFactory sqlConnectionFactory)
    {
        bool? isUpdated = await MovieServices.Update(id, movie, sqlConnectionFactory);
        return isUpdated is null ? Results.BadRequest() : isUpdated is false ? Results.NotFound() : Results.NoContent();
    }

    // DELETE api/Movies/5
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> Delete(int id, SqlConnectionFactory sqlConnectionFactory)
    {
        bool isDeleted = await MovieServices.Delete(id, sqlConnectionFactory);
        return isDeleted is false ? Results.NotFound() : Results.NoContent();
    }
}
