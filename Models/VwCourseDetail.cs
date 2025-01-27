using System;
using System.Collections.Generic;

namespace diveWebAPI.Models;

public partial class VwCourseDetail
{
    public int 課程id { get; set; }

    public string? 課程 { get; set; }

    public string? 等級 { get; set; }

    public string? 教練 { get; set; }

    public decimal? 價格 { get; set; }

    public byte[]? 圖片 { get; set; }

    public DateTime? 建立時間 { get; set; }

    public DateTime? 更新時間 { get; set; }
}
