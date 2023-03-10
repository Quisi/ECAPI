using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
namespace EnergyScanApi.DTOs
{
    /// <summary>
    /// Data-Transfer-Object: User
    /// </summary>
    [JsonObject(MemberSerialization.OptOut)]
    public class UserDTO
    {
        /// <summary>
        /// Unique identifier
        /// </summary>
        [Required]
        public string Id { get; set; }
        /// <summary>
        /// Human readable login identifier
        /// </summary>
        [Required]
        public string Username { get; set; }
        /// <summary>
        /// Email address for verification and password recovery
        /// </summary>
        [Required]
        public string Email { get; set; }
        /// <summary>
        /// Group identifier
        /// </summary>
        public GroupDTO Group { get; set; }
        /// <summary>
        /// Verification switch for an active useraccount
        /// </summary>
        public bool Verified { get; set; }
        /// <summary>
        /// Constructor
        /// </summary>
        public UserDTO()
        {

        }
    }
}
