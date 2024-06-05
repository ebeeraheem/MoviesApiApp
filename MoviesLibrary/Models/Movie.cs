using System.ComponentModel.DataAnnotations;

namespace MoviesLibrary.Models;

public class Movie
{
    public int Id { get; set; }
    public string Title { get; set; }

    [DataType(DataType.Date)]
    public DateTime ReleaseDate { get; set; }
    public string Director { get; set; }
}
