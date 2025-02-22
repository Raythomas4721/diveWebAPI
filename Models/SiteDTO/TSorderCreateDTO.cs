using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace diveWebAPI.Models.TSorderDTO
{
    public class TSorderCreateDTO
    {
        public int? MemberId { get; set; }
        public int? SiteId { get; set; }
        //public string VenueName { get; set; }
        public DateOnly? SiteDay { get; set; }
        public TimeOnly? SiteTime { get; set; }
        public decimal? SitePay { get; set; }

    }
}
