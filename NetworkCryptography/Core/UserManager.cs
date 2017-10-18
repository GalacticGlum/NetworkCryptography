/*
 * Author: Shon Verch
 * File Name: UserManager.cs
 * Project Name: NetworkCryptography
 * Creation Date: 10/17/2017
 * Modified Date: 10/17/2017
 * Description: Manages all users connected to the server.
 */

using System.Collections;
using System.Collections.Generic;

namespace NetworkCryptography.Core
{
    /// <summary>
    /// Manages all users connected to the server.
    /// </summary>
    public abstract class UserManager : IEnumerable<User>
    {
        /// <summary>
        /// Retrieves a user with the specified id.
        /// </summary>
        /// <param name="id">The id of the user.</param>
        /// <returns>The user with the specified id or null if it doesn't exist.</returns>
        public User this[int id] => Get(id);

        /// <summary>
        /// The amount of users in this manager.
        /// </summary>
        public int Count => users.Count;

        /// <summary>
        /// User lookup table.
        /// </summary>
        protected readonly Dictionary<int, User> users;

        public UserManager()
        {
            users = new Dictionary<int, User>();
        }

        /// <summary>
        /// Adds a new user to the manager database.
        /// </summary>
        /// <param name="user">The user to add.</param>
        public void Add(User user)
        {
            users.Add(user.Id, user);
        }

        /// <summary>
        /// Adds a new user with a specified id and name to the manager database.
        /// </summary>
        /// <param name="id">The id of the user.</param>
        /// <param name="name">The name of the user.</param>
        public void Add(int id, string name)
        {
            if (users.ContainsKey(id)) return;

            User user = new User(id, name);
            users.Add(id, user);
        }

        /// <summary>
        /// Removes a user with a specified id.
        /// </summary>
        /// <param name="id">The id of the user.</param>
        /// <returns>The user removed from the database.</returns>
        public User Remove(int id)
        {
            if (!users.ContainsKey(id)) return null;
            User user = Get(id);
            users.Remove(id);

            return user;
        }

        /// <summary>
        /// Retrieves a user with the specified id.
        /// </summary>
        /// <param name="id">The id of the user.</param>
        /// <returns>The user with the specified id or null if it doesn't exist.</returns>
        public User Get(int id) => !users.ContainsKey(id) ? null : users[id];
  
        /// <summary>
        /// Returns an enumerator that iterates through all the users in this manager.
        /// </summary>
        public IEnumerator<User> GetEnumerator() => users.Values.GetEnumerator();

        /// <summary>
        /// Returns an enumerator that iterates through all the users in this manager.
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
