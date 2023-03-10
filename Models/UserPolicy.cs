using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnergyScanApi.Models
{

    public class UserPolicy
    {
        [Required]
        [Key]
        public string Id { get; set; }
        [Required]
        [ForeignKey("User")]
        public string UserId { get; set; }
        [Required]
        [ForeignKey("Policy")]
        public string PolicyId { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime ChangedLast { get; set; }

        public UserPolicy()
        {

        }
    }
}
