﻿using System.Net;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;

namespace Genbox.SimpleS3.Extensions.HttpClientFactory.Extensions
{
    public static class HttpClientFactoryBuilderExtensions
    {
        public static IHttpClientBuilder WithProxy(this IHttpClientBuilder builder, IWebProxy proxy)
        {
            if (proxy == null)
                return builder;

            builder.Services.Configure<HttpClientFactoryOptions>(builder.Name, options =>
            {
                options.HttpMessageHandlerBuilderActions.Add(b =>
                {
                    HttpClientHandler handler = b.Services.GetRequiredService<HttpClientHandler>();
                    handler.Proxy = proxy;
                    handler.UseProxy = true;
                    b.PrimaryHandler = handler;
                });
            });

            return builder;
        }

        public static IHttpClientBuilder WithProxy(this IHttpClientBuilder builder, string proxyUrl)
        {
            if (string.IsNullOrEmpty(proxyUrl))
                return builder;

            builder.Services.Configure<HttpClientFactoryOptions>(builder.Name, options =>
            {
                options.HttpMessageHandlerBuilderActions.Add(b =>
                {
                    HttpClientHandler handler = b.Services.GetRequiredService<HttpClientHandler>();
                    handler.Proxy = new WebProxy(proxyUrl);
                    handler.UseProxy = true;
                    b.PrimaryHandler = handler;
                });
            });

            return builder;
        }
    }
}