using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnergyScanApi.Models
{
    /// <summary>
    /// Database representatiokn: Can
    /// </summary>
    public class Can
    {
        /// <summary>
        /// Unique identifier
        /// </summary>
        [Required]
        [Key]
        public string Id { get; set; }
        [Required]
        [ForeignKey("Can")]
        public string ManufacturerId { get; set; }
        [Required]
        [ForeignKey("Can")]
        public string TasteId { get; set; }
        [ForeignKey("Can")]
        public string CountryId { get; set; } = "";
        [ForeignKey("Status")]
        public string StatusId { get; set; } = "";
        public string Contentamount { get; set; } = "";
        public string Mhd { get; set; } = "";
        public bool Deposit { get; set; } = false;
        public string Closure { get; set; } = "";
        public string Description { get; set; } = "";
        public bool Damaged { get; set; } = false;
        public string Coffeincontent { get; set; } = "";
        public DateTime CreationDate { get; set; }
        public DateTime ChangedLast { get; set; }
        [ForeignKey("User")]
        public string ChangedById { get; set; } = "";
        /// <summary>
        /// Constructor
        /// </summary>
        public Can()
        {

        }
    }
}
