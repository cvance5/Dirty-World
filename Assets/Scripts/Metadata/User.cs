using Player;

namespace Metadata
{
    public class User
    {
        public string UserName { get; }

        public Character CurrentCharacter { get; private set; }

        public User(string userName)
        {
            UserName = userName;
        }

        public void AssignCharacter(Character character)
        {
            CurrentCharacter = character;
        }
    }
}