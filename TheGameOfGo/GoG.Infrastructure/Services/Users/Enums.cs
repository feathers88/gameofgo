namespace GoG.Infrastructure.Services.Users
{
    public enum UserErrorCode
    {
        // Internal server errors will be handled by HTTP so no need to list it here.

        // UserId already exists.
        DuplicateUser,
        // Email address already exists.
        DuplicateEmail,
        // Email in invalid format or empty.
        EmailAddressIsInvalid,
        // Too short, too long, cannot contain spaces, other invalid characters, etc.
        UserIdDoesNotMeetRequrements,
        // Password to short or long, not complext enough...
        PasswordDoesNoteMeetRequirements,
        // New password after password change cannot match old password.
        NewPasswordCannotMatchOld,
        // Authorization failed, either userid or password was wrong (don't tell user which).
        AuthenticationFailed,
        // Authenticated OK, but not authorized to perform operation.
        NotAuthorized,

        // Add more as necessary, but remember these error codes will be passed to the client
        // so don't expose any sensitive info.  Error message strings will be logged, not
        // passed to client.
    }
}
