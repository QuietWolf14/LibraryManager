using System.ComponentModel.DataAnnotations;

namespace LibraryManager.Models;

public class Book
{
    public int ID { get; set; }
    
    [Required]
    public string Title { get; set; }
    public int AuthorID { get; set; }
    public Author? BookAuthor { get; set; }
}