using System.Collections.Generic;

namespace Metadata
{
    public class User
    {
        public string UserName { get; }

        public List<string> Games { get; }

        public User(string userName)
        {
            UserName = userName;
            Games = new List<string>();
        }

        public void RegisterGame(string gameName)
        {
            if (Games.Contains(gameName))
            {
                throw new System.ArgumentException($"User has already registered that game.");
            }
            else Games.Add(gameName);
        }
    }
}