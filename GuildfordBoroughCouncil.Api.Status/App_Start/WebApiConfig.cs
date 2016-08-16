using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace GuildfordBoroughCouncil.Api.Status
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();
            config.Formatters.XmlFormatter.UseXmlSerializer = true;
        }
    }
}
