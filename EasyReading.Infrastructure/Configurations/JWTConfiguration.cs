using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyReading.Infrastructure.Configurations
{
    public class JWTConfiguration
    {
        public string? ValidAudience { get; set; }
        public string? ValidIssuer { get; set; }
    }
}
