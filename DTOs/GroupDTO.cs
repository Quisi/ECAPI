using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace EnergyScanApi.DTOs
{
    /// <summary>
    /// Data-Transfer-Object: Group
    /// </summary>
    [JsonObject(MemberSerialization.OptOut)]
    public class GroupDTO
    {
        /// <summary>
        /// Unique identifier
        /// </summary>
        [Required]
        public string Id { get; set; }
        /// <summary>
        /// Group-Value
        /// </summary>
        [Required]
        public string Name { get; set; }
        /// <summary>
        /// Constructor
        /// </summary>
        public GroupDTO()
        {

        }
    }
}