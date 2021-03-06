﻿using System.Net;
using Microsoft.Extensions.DependencyInjection;

namespace Genbox.SimpleS3.Extensions.HttpClient.Extensions
{
    public static class HttpClientBuilderExtensions
    {
        public static IHttpClientBuilder WithProxy(this IHttpClientBuilder builder, IWebProxy proxy)
        {
            if (proxy == null)
                return builder;

            builder.Services.Configure<HttpClientConfig>(config =>
            {
                config.UseProxy = true;
                config.Proxy = proxy;
            });

            return builder;
        }

        public static IHttpClientBuilder WithProxy(this IHttpClientBuilder builder, string proxyUrl)
        {
            if (string.IsNullOrEmpty(proxyUrl))
                return builder;

            builder.Services.Configure<HttpClientConfig>(config =>
            {
                config.UseProxy = true;
                config.Proxy = new WebProxy(proxyUrl);
            });

            return builder;
        }
    }
}