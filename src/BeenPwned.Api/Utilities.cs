﻿using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace BeenPwned.Api
{
    internal static class Utilities
    {
        internal static string BuildQueryString(Dictionary<string, string> keyValueDictionary)
        {
            var array = keyValueDictionary.Select(x => x.Key + "=" + WebUtility.UrlEncode(x.Value.ToString()))
                .ToArray();

            return "?" + string.Join("&", array);
        }

        internal static bool IsValidEmailaddress(string emailaddress)
        {
            // TODO needs implementation
            return true;
        }
    }
}