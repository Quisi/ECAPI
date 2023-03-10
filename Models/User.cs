using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnergyScanApi.Models
{

    public class User
    {
        [Required]
        [Key]
        public string Id { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [ForeignKey("Group")]
        public string GroupId { get; set; }
        [Required]
        public bool Verified { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime ChangedLast { get; set; }

        public User()
        {

        }
    }
}
