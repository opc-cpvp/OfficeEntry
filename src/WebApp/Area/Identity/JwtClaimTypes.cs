namespace OfficeEntry.WebApp.Area.Identity;

/// <summary>
/// Commonly used claim types
/// </summary>
public static class JwtClaimTypes
{
    /// <summary>Unique Identifier for the End-User at the Issuer.</summary>
    public const string Subject = "sub";

    /// <summary>End-User's full name in displayable form including all name parts, possibly including titles and suffixes, ordered according to the End-User's locale and preferences.</summary>
    public const string Name = "name";
}