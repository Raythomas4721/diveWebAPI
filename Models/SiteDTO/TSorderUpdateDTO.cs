using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace diveWebAPI.Models.TSorderDTO
{
    // PUT 時通常不會改變OrderId，因此這裡不包含它。
    // 如果需要修改SiteId，則保留；否則視具體需求決定是否需要修改SiteId。

    public class TSorderUpdateDTO
    {

        public int OrderId { get; set; }

        public int? MemberId { get; set; }

        //public int? SiteId{get;set;} 根據實際需求決定是否允許修改

        //public string VenueName { get; set; }

        public DateOnly? SiteDay { get; set; }

        public TimeOnly? SiteTime { get; set; }

        public decimal? SitePay { get; set; }

    }

}


