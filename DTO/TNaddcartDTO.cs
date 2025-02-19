using diveWebAPI.Models;

namespace diveWebAPI.DTO
{
    public class TNaddcartDTO
    {
        public int ProductId { get; set; }
        public int ColorId { get; set; }
        public int SizeId { get; set; }
        public int ThicknessId { get; set; }
        public int GenderId { get; set; }

     
        public int Quantity { get; set; }
        // 其他，比如 MemberId(若前端沒帶，就自行從JWT或session拿?)
     
        public int MemberId { get; set; }
    }
}
