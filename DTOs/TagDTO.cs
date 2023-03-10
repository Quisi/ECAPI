using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace EnergyScanApi.DTOs
{   
    /// <summary>
    /// Data-Transfer-Object: Tag
    /// </summary>
    [JsonObject(MemberSerialization.OptOut)]
    public class TagDTO
    {
        /// <summary>
        /// Unique identifier
        /// </summary>
        [Required]
        public string Id { get; set; }
        /// <summary>
        /// Tag-Value
        /// </summary>
        [Required]
        public string Name { get; set; }
        /// <summary>
        /// Constructor
        /// </summary>
        public TagDTO()
        {

        }

    }
}
