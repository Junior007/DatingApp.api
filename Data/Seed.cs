using DatingApp.API.Model;
using DatingApp.API.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.API.Data
{
    public class Seed
    {
        public static void SeedUsers(DataContext context, IAuthService authService) {
            if (!context.Users.Any()) {
                string usersData = System.IO.File.ReadAllText("Data/UserSeedDAta.json");
                var users = JsonConvert.DeserializeObject<List<User>>(usersData );
                foreach (var user in users) {
                    user.Username = user.Username.ToLower();
                    authService.Register(user, "password");
                }
            }
        
        }
    }
}
