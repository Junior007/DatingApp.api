﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.API.Helpers
{
    public static class Extensions
    {
        public static void AddApplicationError(this HttpResponse response, string message)
        {

            response.Headers.Add("Application-Error",message);
            response.Headers.Add("Access-Control-Expose-Header", "Application-Error");
            response.Headers.Add("Access-Control-Allow-Origin", "*");
        }

        public static int CalculatedAge(this DateTime dateOfBirth) {
            int age = DateTime.Now.Year - dateOfBirth.Year;
            if (DateTime.Now.AddYears(age) > dateOfBirth)
                age--;
            return age;
        
        }
    }
}
