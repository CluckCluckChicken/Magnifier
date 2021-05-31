using Magnifier.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Magnifier.Services
{
    public class CommentService
    {
        private readonly IMongoCollection<Comment> comments;

        public CommentService(IMongoDBSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            comments = database.GetCollection<Comment>("comments");
        }

        public List<Comment> Get() =>
            comments.Find(comment => true).ToList();

        public Comment Get(int commentId) =>
            comments.Find<Comment>(comment => comment.commentId == commentId).FirstOrDefault();

        public Comment Create(Comment comment)
        {
            if (Get(comment.commentId) == null)
            {
                comments.InsertOne(comment);
            }
            return comment;
        }

        public void Update(int commentId, Comment commentIn) =>
            comments.FindOneAndReplace(comment => comment.commentId == commentId, commentIn);

        public void Remove(Comment commentIn) =>
            comments.DeleteOne(comment => comment._id == commentIn._id);

        public void Remove(string id) =>
            comments.DeleteOne(comment => comment._id == id);
    }
}