using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnergyScanApi.Models
{
    public class BarcodeCan
    {
        [Required]
        [Key]
        public string Id { get; set; }
        [Required]
        [ForeignKey("Barcode")]
        public string BarcodeId { get; set; }
        [Required]
        [ForeignKey("Can")]
        public string CanId { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime ChangedLast { get; set; }
        [ForeignKey("User")]
        public string ChangedById { get; set; }

        public BarcodeCan()
        {

        }
    }
}
