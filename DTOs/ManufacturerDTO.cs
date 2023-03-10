using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace EnergyScanApi.DTOs
{
    /// <summary>
    /// Data-Transfer-Object: Manufacturer
    /// </summary>
    [JsonObject(MemberSerialization.OptOut)]
    public class ManufacturerDTO
    {
        /// <summary>
        /// Unique identifier
        /// </summary>
        [Required]
        public string Id { get; set; }
        /// <summary>
        /// Manufacturer-Value
        /// </summary>
        [Required]
        public string Name { get; set; }
        /// <summary>
        /// Constructor
        /// </summary>
        public ManufacturerDTO()
        {

        }
    }
}
