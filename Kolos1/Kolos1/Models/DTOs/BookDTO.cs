namespace Kolos1.Models.DTOs;

public class BookDTO
{
    public int id { get; set; }
    public String title { get; set; }
    public List<AuthorDTO> authors { get; set; }
}

public class AuthorDTO
{
    public string firstName { get; set; }
    public string lastName { get; set; }
}