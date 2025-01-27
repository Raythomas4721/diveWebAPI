using System;
using System.Collections.Generic;

namespace diveWebAPI.Models;

public partial class TUproductImage
{
    public int ProductImagesId { get; set; }

    public int? UproductId { get; set; }

    public byte[]? Uimage { get; set; }

    public virtual TUproduct? Uproduct { get; set; }
}
