namespace Tests_and_Interviews.Models.Core
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Users")]
    public class User
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("name", TypeName = "nvarchar(255)")]
        public string Name { get; set; }

        [Column("email", TypeName = "nvarchar(255)")]
        public string Email { get; set; }

        [Column("cv_xml", TypeName = "nvarchar(max)")]
        public string? CvXml { get; set; }

        public User(int id, string name, string email, string? cvXml = null)
        {
            this.Id = id;
            this.Name = name;
            this.Email = email;
            this.CvXml = cvXml;
        }

        public User() { }
    }
}
