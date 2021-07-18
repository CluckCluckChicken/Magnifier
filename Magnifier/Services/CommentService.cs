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

        public List<Comment> GetMany(List<int> commentIds) =>
            comments.Find<Comment>(comment => commentIds.Contains(comment.commentId)).ToList();

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

        /*
        {
          "_id": {
            "$oid": "60d00083927d820f4585918c"
          },
          "commentId": 147414923,
          "comment": {
            "_id": null,
            "id": 147414923,
            "content": "@IHaveToBlink  It&#39;s an extension, I can&#39;t tell the name of it because of https://scratch.mit.edu/discuss/topic/284272/",
            "datetime_created": {
              "$date": "2021-06-21T02:33:55.000Z"
            },
            "author": {
              "_id": null,
              "username": "Chiroyce",
              "image": "//cdn2.scratch.mit.edu/get_image/user/58524660_60x60.png"
            }
          },
          "residence": 1,
          "residenceId": "Chiroyce",
          "reactions": [],
          "isPinned": false,
          "isReply": true,
          "replies": [],
          "stars": null
        }
		*/
    }
}