using System;

namespace GuildfordBoroughCouncil.Api.Status.Models
{
    public class pingdom_http_custom_check
    {
        public string status { get; set; }
        public double response_time { get; set; }
    }
}