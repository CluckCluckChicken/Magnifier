using Magnifier.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Magnifier.Services
{
    public class AuthCodeService
    {
        private readonly IMongoCollection<AuthCode> authCodes;

        public AuthCodeService(IMongoDBSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            authCodes = database.GetCollection<AuthCode>("authCodes");
        }

        public List<AuthCode> Get() =>
            authCodes.Find(authCode => true).ToList();

        public AuthCode Get(string code) =>
            authCodes.Find<AuthCode>(authCode => authCode.code == code).FirstOrDefault();

        public AuthCode Create(AuthCode authCode)
        {
            authCodes.InsertOne(authCode);
            return authCode;
        }

        public void Update(string code, AuthCode authCodeIn) =>
            authCodes.ReplaceOne(authCode => authCode.code == code, authCodeIn);

        public void Remove(AuthCode authCodeIn) =>
            authCodes.DeleteOne(comment => comment.id == authCodeIn.id);

        public void Remove(string code) =>
            authCodes.DeleteOne(authCode => authCode.code == code);
    }
}