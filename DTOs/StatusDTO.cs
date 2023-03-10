using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace EnergyScanApi.DTOs
{
    /// <summary>
    /// Data-Transfer-Object: Status
    /// </summary>
    [JsonObject(MemberSerialization.OptOut)]
    public class StatusDTO
    {
        /// <summary>
        /// Unique identifier
        /// </summary>
        [Required]
        public string Id { get; set; }
        /// <summary>
        /// Status-Value
        /// </summary>
        [Required]
        public string Name { get; set; }
        /// <summary>
        /// Constructor
        /// </summary>
        public StatusDTO()
        {

        }

    }
}
