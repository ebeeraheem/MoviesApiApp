using System.ComponentModel.DataAnnotations;

namespace MoviesLibrary.Models;

public class MovieDto
{
    [Required]
    public string Title { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime ReleaseDate { get; set; }

    [Required]
    public string Director { get; set; }
}
