using Metadata;
using Newtonsoft.Json;

namespace Data.Serialization
{
    public class SerializableUser : ISerializable<User>
    {
        [JsonProperty("userName")]
        public string UserName;

        [JsonConstructor]
        public SerializableUser() { }

        public SerializableUser(User user)
        {
            UserName = user.UserName;
        }

        public string Serialize() => JsonConvert.SerializeObject(this);
        public static SerializableUser Deserialize(string userJson) => JsonConvert.DeserializeObject<SerializableUser>(userJson);

        public User ToObject()
        {
            return new User(UserName);
        }
    }
}