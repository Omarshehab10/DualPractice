namespace DTO.DTOs.SehaIntegration
{
    /// <summary>
    /// UserDto
    /// </summary>
    public class UserDto
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Role
        /// </summary>
        public string Role { get; set; }
        /// <summary>
        /// Ticket
        /// </summary>
        public string Ticket { get; set; }
        /// <summary>
        /// UserId
        /// </summary>
        public int UserId { get; set; }
    }

    /// <summary>
    /// AuthenticationDto
    /// </summary>
    public class AuthenticationDto
    {
        /// <summary>
        /// Code
        /// </summary>
        public string Code { get; set; }

    }
}
