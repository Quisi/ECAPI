using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace EnergyScanApi.DTOs
{
    /// <summary>
    /// Data-Transfer-Object: Barcode
    /// </summary>
    [JsonObject(MemberSerialization.OptOut)]
    public class BarcodeDTO
    {
        /// <summary>
        /// Unique identifier
        /// </summary>
        [Required]
        public string Id { get; set; }
        /// <summary>
        /// Barcode-Value
        /// </summary>
        [Required]
        public string Name { get; set; }
        /// <summary>
        /// Constructor
        /// </summary>
        public BarcodeDTO()
        {

        }

    }
}
