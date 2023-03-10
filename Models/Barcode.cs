using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnergyScanApi.Models
{
    public class Barcode
    {
        /// <summary>
        /// Id of Barcode
        /// </summary>
        [Required]
        [Key]
        public string Id { get; set; }
        /// <summary>
        /// Actual Barcode
        /// </summary>
        [Required]
        public string Name { get; set; }
        /// <summary>
        /// Date when this Barcode was created
        /// </summary>
        public DateTime CreationDate { get; set; }
        /// <summary>
        /// Date when this entry was last changed
        /// </summary>
        public DateTime ChangedLast { get; set; }
        /// <summary>
        /// UserId of User made the last change
        /// </summary>
        [ForeignKey("User")]
        public string ChangedById { get; set; }

        public Barcode()
        {

        }

    }
}
