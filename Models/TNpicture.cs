using System;
using System.Collections.Generic;

namespace diveWebAPI.Models;

public partial class TNpicture
{
    public int PictureId { get; set; }

    public int? ProductId { get; set; }

    public byte[]? Image { get; set; }

    public bool? IsMain { get; set; }

    public virtual TNproduct? Product { get; set; }
}
