using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.API.Dtos
{
    public class PhotoForCreation
    {
        public PhotoForCreation()
        {
            DateAdded = DateTime.Now;
        }
        public string Url { get; set; }
        public IFormFile file { get; set; }
        public string Description { get; set; }
        public DateTime DateAdded { get; set; }
        public string PublicId { get; set; }
    }
}
