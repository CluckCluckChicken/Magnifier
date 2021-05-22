using Magnifier.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Magnifier.Services
{
    public class UserService
    {
        private readonly IMongoCollection<User> users;

        public UserService(IMongoDBSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            users = database.GetCollection<User>("users");
        }

        public List<User> Get() =>
            users.Find(user => true).ToList();

        public User Get(string username) =>
            users.Find<User>(user => user.username == username).FirstOrDefault();

        public User Create(User user)
        {
            users.InsertOne(user);
            return user;
        }

        public void Update(string username, User userIn) =>
            users.ReplaceOne(user => user.username == username, userIn);

        public void Remove(User userIn) =>
            users.DeleteOne(user => user.id == userIn.id);

        public void Remove(string id) =>
            users.DeleteOne(user => user.id == id);
    }
}