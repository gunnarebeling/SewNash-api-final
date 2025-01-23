using System.ComponentModel.DataAnnotations;


namespace SewNash.Models;

public class Photo
{
    public int Id { get; set; }
    [Required]
    public string FileKey { get; set; }
    public bool MainPhoto { get; set; } 
    public int? SewClassId { get; set; }
    public SewClass SewClass { get; set; }

}