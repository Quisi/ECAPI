namespace EnergyScanApi.DTOs
{
    /// <summary>
    /// Access Token inherit from UserDTO
    /// </summary>
    public class TokenDTO : UserDTO
    {
        /// <summary>
        /// Access-Token
        /// </summary>
        public string access_token { get; set; }
        /// <summary>
        /// Constructor
        /// </summary>
        public TokenDTO()
        {

        }
    }
}
