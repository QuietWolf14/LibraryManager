namespace LibraryManager.Models;

public class Book
{
    public int ID { get; set; }
    public string Title { get; set; }
    public Author? BookAuthor { get; set; }
}