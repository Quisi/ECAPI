using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace EnergyScanApi.DTOs
{
    /// <summary>
    /// Data-Transfer-Object: Taste
    /// </summary>
    [JsonObject(MemberSerialization.OptOut)]
    public class TasteDTO
    {
        /// <summary>
        /// Unique identifier
        /// </summary>
        [Required]
        public string Id { get; set; }
        /// <summary>
        /// Taste-Value
        /// </summary>
        [Required]
        public string Name { get; set; }
        /// <summary>
        /// Constructor
        /// </summary>
        public TasteDTO()
        {

        }

    }
}
