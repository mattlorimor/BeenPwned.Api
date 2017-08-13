﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using BeenPwned.Api.Internals;
using BeenPwned.Api.Models;

namespace BeenPwned.Api
{
    public class BeenPwnedClient : IBeenPwnedClient
    {
        private readonly IRequestExcecuter _requestExcecuter;

        public BeenPwnedClient(string useragent, string baseApiUrl = "https://haveibeenpwned.com/api/v2/")
        {
            if (string.IsNullOrWhiteSpace(useragent))
                throw new ArgumentException("For communication to the HIBP API a user-agent needs to be set.", nameof(useragent));

            if (!Uri.IsWellFormedUriString(baseApiUrl, UriKind.Absolute))
                throw new ArgumentException("The given HIBP base URL does not seem to be valid. Make sure you provide a full, valid URL.", nameof(baseApiUrl));

            _requestExcecuter = new RequestExcecuter(useragent, baseApiUrl);
        }
        
        public async Task<IEnumerable<Breach>> GetAllBreaches(bool truncateResponse = true, string domain = "", bool includeUnverified = false)
        {
            var queryValues = new Dictionary<string, string>
            {
                {"truncateResponse", truncateResponse.ToString()},
                {"includeUnverified", includeUnverified.ToString()}
            };

            if (!string.IsNullOrWhiteSpace(domain))
                queryValues.Add("domain", domain);

            var endpointUrl = Utilities.BuildQueryString("breaches", queryValues);

            return await _requestExcecuter.GetResultAsync<IEnumerable<Breach>>(endpointUrl);
        }

        public async Task<IEnumerable<Breach>> GetBreachesForAccount(string account, bool truncateResponse = true, bool includeUnverified = false)
        {
            if (string.IsNullOrWhiteSpace(account))
                throw new ArgumentException("An account name needs to be specified", nameof(account));

            var queryValues = new Dictionary<string, string>
            {
                { "truncateResponse", truncateResponse.ToString() },
                { "includeUnverified", includeUnverified.ToString() }
            };

            var endpointUrl = Utilities.BuildQueryString($"breachesbreachedaccount/{account}", queryValues);

            return await _requestExcecuter.GetResultAsync<IEnumerable<Breach>>(endpointUrl);
        }

        public async Task<IEnumerable<Paste>> GetPastesForAccount(string account)
        {
            if (string.IsNullOrWhiteSpace(account))
                throw new ArgumentException("An account name needs to be specified", nameof(account));

            if (!Utilities.IsValidEmailaddress(account))
                throw new ArgumentException("Account it not a (valid) emailaddress", nameof(account));

            return await _requestExcecuter.GetResultAsync<IEnumerable<Paste>>($"pasteaccount/{account}");
        }

        public async Task<IEnumerable<string>> GetAllDataClasses()
        {
            return await _requestExcecuter.GetResultAsync<IEnumerable<string>>("dataclasses");
        }
        
        public async Task<bool> GetPwnedPassword(string password, bool originalPasswordIsAHash = false,
            bool sendAsPostRequest = false)
        {
            var queryValues = new Dictionary<string, string>
            {
                { "originalPasswordIsAHash", originalPasswordIsAHash.ToString() }
            };

            HttpResponseMessage result;

            if (sendAsPostRequest)
            {
                var formValues =
                    new List<KeyValuePair<string, string>> {new KeyValuePair<string, string>("Password", password)};

                var endpointUrl = Utilities.BuildQueryString("pwnedpassword", queryValues);

                result = await _requestExcecuter.PostAsync(endpointUrl, new FormUrlEncodedContent(formValues));
            }
            else
            {
                var endpointUrl = Utilities.BuildQueryString($"pwnedpassword/{password}", queryValues);

                result = await _requestExcecuter.GetAsync(endpointUrl);
            }

            switch ((int) result.StatusCode)
            {
                case 200:
                    return true;
                case 404:
                    return false;
                default:
                    throw new Exception($"Unexpected result from API. Statuscode {result.StatusCode}, message: {result.ReasonPhrase}");
            }
        }

        public void Dispose()
        {
            _requestExcecuter.Dispose();
        }
    }
}