/*
 * Author: Shon Verch
 * File Name: User.cs
 * Project Name: NetworkCryptography
 * Creation Date: 10/17/2017
 * Modified Date: 10/17/2017
 * Description: A information about a user conected to the server.
 */

namespace NetworkCryptography.Core
{
    /// <summary>
    /// Information about a user conected to the server.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Integer-id of the user.
        /// Used for easy lookup of users.
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// The name of the user.
        /// Used for display purposes only.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Indicates whether the user is offline.
        /// </summary>
        public bool IsOffline { get; set; }

        /// <summary>
        /// Creates a new user with a specified id and name.
        /// </summary>
        /// <param name="id">The id of the user.</param>
        /// <param name="name">The name of the user.</param>
        public User(int id, string name)
        {
            Id = id;
            Name = name;
            IsOffline = false;
        }
    }
}
