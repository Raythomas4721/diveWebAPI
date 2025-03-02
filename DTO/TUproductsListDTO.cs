using diveWebAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace diveWebAPI.DTO
{
    public class TUproductsListDTO 
    {
        public int ProductId { get; set; }
        
        public int? SellerId { get; set; }

        public int? CategoryId { get; set; }
       
        public string ProductName { get; set; }
        
        public string ProductDescription { get; set; }

        public decimal? ProductPrice { get; set; }
       

        public DateTime? UpdatedAt { get; set; }
     
        public DateTime? CreatedAt { get; set; }
      
        public int? ProductConditionId { get; set; }
  
        public bool? ProductStatus { get; set; }

        public string? TUproductImages { get; set; }
        //public virtual TUcategory Category { get; set; }

        //public virtual TUproductCondition ProductCondition { get; set; }

        //public virtual TMmemberList Seller { get; set; }

        //public virtual ICollection<TUorderDetail> TUorderDetails { get; set; } = new List<TUorderDetail>();

        //public virtual ICollection<TUproductImage> TUproductImages { get; set; } = new List<TUproductImage>();
    }
}
