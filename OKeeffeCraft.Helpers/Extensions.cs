using System.Collections;

namespace OKeeffeCraft.Helpers
{
    public static class Extensions
    {
        /// <summary>
        /// Indicates if a list or array is null or contains no elements (empty) 
        /// </summary>
        public static bool IsNullOrEmpty(this IList list)
        {
            return (list == null || list.Count < 1);
        }


        public static bool IsNullOrEmpty(this string value)
        {
            return (value == null || value.Length < 1);
        }


        public static string Truncate(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) { return value; }

            return value.Substring(0, Math.Min(value.Length, maxLength));
        }

        public static bool IsValidEmail(this string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
