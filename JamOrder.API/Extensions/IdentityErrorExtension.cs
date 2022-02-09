using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;

namespace JamOrder.API.Extensions
{
    public static class IdentityErrorExtension
    {
        public static string GetErrors(this IEnumerable<IdentityError> identityErrors)
        {
            return string.Join(", ", identityErrors.Select(x => x.Description));
        }
    }
}
