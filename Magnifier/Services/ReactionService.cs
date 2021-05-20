using Magnifier.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Magnifier.Services
{
    public class ReactionService
    {
        private readonly IMongoCollection<Reaction> reactions;

        public ReactionService(IMongoDBSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            reactions = database.GetCollection<Reaction>("reactions");
        }

        public List<Reaction> Get() =>
            reactions.Find(reaction => true).ToList();

        public Reaction Get(string name) =>
            reactions.Find<Reaction>(reaction => reaction.name == name).FirstOrDefault();

        public Reaction Create(Reaction reaction)
        {
            reactions.InsertOne(reaction);
            return reaction;
        }

        public void Update(string id, Reaction reactionIn) =>
            reactions.ReplaceOne(reaction => reaction.id == id, reactionIn);

        public void Remove(Reaction reactionIn) =>
            reactions.DeleteOne(reaction => reaction.id == reactionIn.id);

        public void Remove(string id) =>
            reactions.DeleteOne(reaction => reaction.id == id);
    }
}