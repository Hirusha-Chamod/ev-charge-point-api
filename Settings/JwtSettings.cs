// --------------------------------------------------------------------------------------------------------------------
// <project>EV Charging Station Management</project>
// <file>AuthController.cs</file>
// <author>GOMIS R J S (IT22349606)</author>
// <module>SE4040 - Enterprise Application Development</module>
// <date>2025-10-10</date>
// <summary>
//   JWT settings class for JWT related settings needed for the application
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ev_charge_point_api.Settings
{
    public class JwtSettings
    {
        public string SecretKey { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public int ExpirationMinutes { get; set; } = 120; // 2 hours default
    }
}
