using System.Collections.Generic;

namespace SewNash.Models.DTOs
{
    public interface ClassWithPhotos
    {
        List<PhotoDTO> Photos { get; set; }
    }
}