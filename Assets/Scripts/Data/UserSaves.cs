﻿using Data.IO;
using Data.Serialization;
using Metadata;
using System.Collections.Generic;
using System.IO;

namespace Data
{
    public static class UserSaves
    {
        private static List<string> _savedUsers;
        public static List<string> SavedUsers => new List<string>(_savedUsers);
        public static bool HasSavedData => _savedUsers.Count > 0;
        public static bool SaveExists(string userName) => _savedUsers.Contains(userName);

        public static User CurrentUser { get; private set; }

        public static void Refresh() => _savedUsers = DataReader.FindAllFiles(DataTypes.Users);

        public static User LoadUser(string userToLoad)
        {
            var userJson = DataReader.Read(userToLoad, DataTypes.Users);
            CurrentUser = SerializableUser.Deserialize(userJson).ToObject();

            return CurrentUser;
        }

        public static User CreateUser(string userName)
        {
            CurrentUser = new User(userName);
            SaveUser();
            return CurrentUser;
        }

        public static void SaveUser()
        {
            if (!DataReader.Exists(DataTypes.Users))
            {
                Directory.CreateDirectory(Paths.ToPath(DataTypes.Users));
            }

            var writeLocation = Paths.ToPath(DataTypes.Users, CurrentUser.UserName);
            var data = new SerializableUser(CurrentUser);

            DataWriter.Write(writeLocation, data.Serialize());
        }
    }
}