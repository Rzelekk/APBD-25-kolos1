using System.ComponentModel.DataAnnotations;

namespace Kolos1.Models.DTOs;

public class NewBookDTO
{
    [Required]
    public String title { get; set; }
    [Required]
    public List<NewAuthorDTO> authors { get; set; }
}

public class NewAuthorDTO
{
    public string firstName { get; set; }
    public string lastName { get; set; }
}