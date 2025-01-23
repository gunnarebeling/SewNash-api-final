

namespace SewNash.Models.DTOs;

public class SewClassDTO : ClassWithPhotos
{
    public int Id { get; set; }
    
    public string Name { get; set; }
    
    public string Description { get; set; }
    
    public int MaxPeople { get; set; }
    
    public decimal PricePerPerson { get; set; }
    public int Duration { get; set; }
    public List<PhotoDTO> Photos { get; set; }

}
public class SewClassForSessionDTO : ClassWithPhotos
{
    public int Id { get; set; }
    
    public string Name { get; set; }
    
    public string Description { get; set; }
    
    public int MaxPeople { get; set; }
    
    public decimal PricePerPerson { get; set; }
    public int Duration { get; set; }
    public List<PhotoDTO> Photos { get; set; }
    public PhotoDTO mainPhoto 
    {
        get
        {
            return Photos.SingleOrDefault(p => p.MainPhoto);
        }
    }
}
public class PostClassDTO
{
    public string Name { get; set; }
    
    public string Description { get; set; }
    
    public int MaxPeople { get; set; }
    
    public decimal PricePerPerson { get; set; }
    public int Duration { get; set; }
}