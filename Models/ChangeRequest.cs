using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnergyScanApi.Models
{
    public class ChangeRequest
    {
        [Required]
        [Key]
        public string Id { get; set; }
        [Required]
        public string Table { get; set; }
        [Required]
        public string Field { get; set; }
        [Required]
        public string PkField { get; set; }
        [Required]
        public string Pk { get; set; }
        [Required]
        [ForeignKey("User")]
        public string UserId { get; set; }
        public DateTime Timestamp { get; set; }
        [Required]
        [ForeignKey("Status")]
        public string StateId { get; set; }
        [Required]
        public string ChangeOldValue { get; set; }
        [Required]
        public string ChangeNewValue { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime ChangedLast { get; set; }
        [ForeignKey("User")]
        public string ChangedById { get; set; }
        public ChangeRequest()
        {

        }
    }
}
