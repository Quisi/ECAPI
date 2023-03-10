using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnergyScanApi.Models
{
    public class TagCan
    {
        [Required]
        [Key]
        public string Id { get; set; }
        [Required]
        public string TagId { get; set; }
        [Required]
        public string CanId { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime ChangedLast { get; set; }
        [ForeignKey("User")]
        public string ChangedById { get; set; }

        public TagCan()
        {

        }
    }
}
