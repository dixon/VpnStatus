using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VpnStatus
{
    internal static class ExtensionMethods
    {
        /// <summary>
        /// Returns true if this String is neither null or empty.
        /// </summary>
        /// <remarks>I'm also tired of typing !String.IsNullOrEmpty(s)</remarks>
        public static bool HasValue(this string s) => !string.IsNullOrWhiteSpace(s);

        /// <summary>
        /// Returns true if this String is either null or complete whitespace.
        /// </summary>
        public static bool IsNullOrWhiteSpace(this string s) => string.IsNullOrWhiteSpace(s);
    }
}
