using Metadata;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Data.Serialization
{
    public class SerializableUser : ISerializable<User>
    {
        [JsonProperty("userName")]
        public string UserName;

        [JsonProperty("games")]
        public List<string> Games = new List<string>();

        [JsonConstructor]
        public SerializableUser() { }

        public SerializableUser(User user)
        {
            UserName = user.UserName;
            Games = user.Games;
        }

        public string Serialize() => JsonConvert.SerializeObject(this);
        public static SerializableUser Deserialize(string userJson) => JsonConvert.DeserializeObject<SerializableUser>(userJson);

        public User ToObject()
        {
            var user = new User(UserName);

            foreach (var game in Games)
            {
                user.RegisterGame(game);
            }

            return user;
        }
    }
}