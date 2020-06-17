using System;

namespace OfficeEntry.Domain.Exceptions
{
    public class AdAccountInvalidException : Exception
    {
        public AdAccountInvalidException(string adAccount, Exception ex = null)
            : base($"AD Account \"{adAccount}\" is invalid.", ex)
        {
        }
    }
}
