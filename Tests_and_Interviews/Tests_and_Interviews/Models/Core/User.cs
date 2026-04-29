namespace Tests_and_Interviews.Models.Core
{
    using System;

    public class User
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string? CvXml { get; set; }

        public User(int id, string name, string email, string? cvXml = null)
        {
            this.Id = id;
            this.Name = name;
            this.Email = email;
            this.CvXml = cvXml;
        }
    }
}
