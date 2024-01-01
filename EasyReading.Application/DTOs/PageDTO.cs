using Postgrest.Attributes;
using Postgrest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EasyReading.Application.DTOs
{
    public class PageDTO
    {
        [JsonPropertyName("similarity")]
        public float Similarity { get; set; }
        [JsonPropertyName("body")]
        public string? Body { get; set; }
    }
}
