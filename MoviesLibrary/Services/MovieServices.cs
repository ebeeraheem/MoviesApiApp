using Dapper;
using Microsoft.Data.SqlClient;
using MoviesLibrary.Models;

namespace MoviesLibrary.Services;

public static class MovieServices
{
    //This should be replaced by a database
    private static List<Movie> _movies = new List<Movie>();
    private static int _nextMovieId = 1;

    public static async Task<IEnumerable<Movie>> GetAllMovies(SqlConnectionFactory sqlConnectionFactory)
    {
        using (var connection = sqlConnectionFactory.GetConnection())
        {
            const string slqStatement = "SELECT Id, Title, ReleaseDate, Director FROM Movies;";
            var movies = await connection.QueryAsync<Movie>(slqStatement);
            return movies;
        }
    }
    public static async Task<Movie?> GetMovieById(int id, SqlConnectionFactory sqlConnectionFactory)
    {
        using (var connection = sqlConnectionFactory.GetConnection())
        {
            const string slqStatement = "SELECT Id, Title, ReleaseDate, Director FROM Movies WHERE Id = @MovieId;";

            var movie = await connection.QuerySingleOrDefaultAsync<Movie>(slqStatement, new { MovieId = id });
            return movie is not null ? movie : null;
        }
    }
    public static async Task<bool> Add(MovieDto movieDto, SqlConnectionFactory sqlConnectionFactory)
    {
        if (!IsValidMovieDto(movieDto))
        {
            return false;
        }

        using (var connection = sqlConnectionFactory.GetConnection())
        {
            const string sqlStatement = "INSERT INTO Movies(Title, ReleaseDate, Director) VALUES(@Title, @ReleaseDate, @Director);";
            await connection.ExecuteAsync(sqlStatement, movieDto);

            return true;
        }
    }
    public static async Task<bool?> Update(int id, Movie movie, SqlConnectionFactory sqlConnectionFactory)
    {
        if (movie.Id != id)
        {
            return null;
        }

        using (var connection = sqlConnectionFactory.GetConnection())
        {
            //Check if the movie exists in the db and return NotFound if it doesn't
            const string slqStatement = "SELECT Id, Title, ReleaseDate, Director FROM Movies WHERE Id = @MovieId;";

            var movieInDb = await connection.QuerySingleOrDefaultAsync<Movie>(slqStatement, new { MovieId = id });

            if (movieInDb is null)
            {
                return false;
            }

            //Update the movie with the newly provided movie object
            const string sqlStatement = "UPDATE Movies SET Title = @Title, ReleaseDate = @ReleaseDate, Director = @Director WHERE Id = @Id;";

            await connection.ExecuteAsync(sqlStatement, movie);

            return true;
        }
    }
    public static async Task<bool> Delete(int id, SqlConnectionFactory sqlConnectionFactory)
    {
        using (var connection = sqlConnectionFactory.GetConnection())
        {
            //Check if the movie exists in the db and return NotFound if it doesn't
            const string slqStatement = "SELECT Id, Title, ReleaseDate, Director FROM Movies WHERE Id = @MovieId;";

            var movieInDb = await connection.QuerySingleOrDefaultAsync<Movie>(slqStatement, new { MovieId = id });

            if (movieInDb is null)
            {
                return false;
            }

            //Delete the movie if it exists
            const string sqlStatement = "DELETE FROM Movies WHERE Id = @MovieId;";
            await connection.ExecuteAsync(sqlStatement, new { MovieId = id });

            return true;
        }
    }
    public static bool IsValidMovieDto(MovieDto movieDto)
    {
        return !string.IsNullOrWhiteSpace(movieDto.Title) &&
               movieDto.ReleaseDate != null &&
               !string.IsNullOrWhiteSpace(movieDto.Director);
    }
}
