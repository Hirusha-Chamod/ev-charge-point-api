using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace ev_charge_point_api.Models
{
    public class RefreshToken
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Token { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public DateTime ExpiryDate { get; set; }
    }
}
