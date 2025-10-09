// --------------------------------------------------------------------------------------------------------------------
// <project>EV Charging Station Management</project>
// <file>AuthController.cs</file>
// <author>GOMIS R J S (IT22349606)</author>
// <module>SE4040 - Enterprise Application Development</module>
// <date>2025-10-10</date>
// <summary>
//   Data model for saving refresh token in the database. For when the user requests for new token with refresh token
//   that refresh token is check with the available tokens in db and will remove token if expired or new token generated
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

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
