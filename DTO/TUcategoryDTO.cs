using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations.Schema;

namespace diveWebAPI.DTO
{
    
    public class TUcategoryDTO : Controller
    {
        
        public int CategoryId { get; set; }

        public string? CategoryName { get; set; }
    }
}
