﻿using System;
using System.Net;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Authentication;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Extensions;
using Genbox.SimpleS3.Extensions.HttpClient.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Genbox.SimpleS3.Examples.Clients.DependencyInjection
{
    /// <summary>This is an example that shows the full capabilities of SimpleS3.</summary>
    public static class AmazonDiFullClient
    {
        public static S3Client Create(string keyId, string accessKey, AwsRegion region, IWebProxy proxy = null)
        {
            //In this example we are using Dependency Injection (DI) using Microsoft's DI framework
            ServiceCollection services = new ServiceCollection();

            //We use Microsoft.Extensions.Configuration framework here to load our config file
            ConfigurationBuilder configBuilder = new ConfigurationBuilder();
            configBuilder.AddJsonFile("Config.json", false);
            IConfigurationRoot root = configBuilder.Build();

            //We use Microsoft.Extensions.Logging here to add support for logging
            services.AddLogging(x =>
            {
                x.AddConsole();
                x.AddConfiguration(root.GetSection("Logging"));
            });

            //Here we create a core client without a network driver
            ICoreBuilder coreBuilder = services.AddSimpleS3Core(s3Config =>
            {
                root.Bind(s3Config);

                s3Config.Credentials = new StringAccessKey(keyId, accessKey);
                s3Config.Region = region;
            });

            //The default client is HttpClientFactory, but to show how we can change this, we use HttpClient here.
            coreBuilder.UseHttpClient()
                       .WithProxy(proxy);

            //This adds the S3Client service
            coreBuilder.UseS3Client();

            //Finally we build the service provider and return the S3Client
            IServiceProvider serviceProvider = services.BuildServiceProvider();

            return serviceProvider.GetRequiredService<S3Client>();
        }
    }
}