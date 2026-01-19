using System.Security.Claims;
namespace Notes_Backend.Utilities
{
    public static class UserHelper
    {
        /// <summary>
        /// Attempts to extract the current user's ID from JWT claims (from 'nameid' or 'sub').
        /// </summary>
        /// <param name="user">The ClaimsPrincipal representing the authenticated user.</param>
        /// <param name="userId">The parsed user ID if successful; otherwise, 0.</param>
        /// <returns>True if the user ID was found and parsed successfully; otherwise, false.</returns>
        public static bool TryGetUserId(this ClaimsPrincipal user, out int userId)
        {
            userId = 0;
            var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier)
                              ?? user.FindFirstValue("sub");
            return int.TryParse(userIdClaim, out userId);
        }
    }
}