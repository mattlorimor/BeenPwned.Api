﻿using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace BeenPwned.Api.Internals
{
    internal interface IRequestExcecuter : IDisposable
    {
        Task<T> GetResultAsync<T>(string endpoint) where T : class;
        Task<HttpResponseMessage> GetAsync(string endpointUrl);
        Task<HttpResponseMessage> PostAsync(string endpointUrl, FormUrlEncodedContent formUrlEncodedContent);
    }
}
