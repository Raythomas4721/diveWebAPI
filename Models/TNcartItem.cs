using System;
using System.Collections.Generic;

namespace diveWebAPI.Models;

public partial class TNcartItem
{
    public int CartitemId { get; set; }

    public int? CartId { get; set; }

    public int? UproductId { get; set; }

    public int? ProductvariantsId { get; set; }

    public int? Quantity { get; set; }

    public decimal? UnitpriceatCart { get; set; }

    public bool? IsLocked { get; set; }

    public string? Condition { get; set; }

    public DateTime? CreationDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual TNcart? Cart { get; set; }

    public virtual TNproductvariant? Productvariants { get; set; }

    public virtual TUproduct? Uproduct { get; set; }
}
