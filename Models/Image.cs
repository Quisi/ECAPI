using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnergyScanApi.Models
{

    public class Image
    {
        [Required]
        [Key]
        public string Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Url { get; set; }
        public string Description { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime ChangedLast { get; set; }
        [ForeignKey("User")]
        public string ChangedById { get; set; }

        public Image()
        {

        }
    }
}
