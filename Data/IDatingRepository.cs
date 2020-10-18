﻿using DatingApp.API.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAtingApp.API.Data
{
    public interface IDatingRepository
    {
        void Add<T>(T entity) where T : class;
        void Delete<T>(T entity) where T : class;
        void Update<T>(T entity) where T : class;
        Task<bool> SaveAll();
        Task<IEnumerable<User>> Users();
        Task<User> User(int id);
        bool UserExists(int id);
        /*<IEnumerable<T>> Get<T>() where T : class;
Task<T> GetAll<T>() where T : class;*/
    }
}
