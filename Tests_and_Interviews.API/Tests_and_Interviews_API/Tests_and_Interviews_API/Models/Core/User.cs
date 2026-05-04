namespace Tests_and_Interviews_API.Models.Core
{
    /// <summary>
    /// User class represents an individual user in the system, containing properties 
    /// for the user's unique identifier, name, and email address.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Gets or sets the unique identifier for the entity.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Gets or sets the name associated with this entity.
        /// </summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the email address associated with this entity.
        /// </summary>
        public string Email { get; set; } = string.Empty;
    }
}