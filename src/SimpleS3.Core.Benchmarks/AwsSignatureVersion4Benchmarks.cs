﻿using System;
using System.Net.Http;
using Amazon.Runtime;
using Amazon.Util;
using AwsSignatureVersion4.Private;
using BenchmarkDotNet.Attributes;
using Genbox.SimpleS3.Core.Abstracts.Constants;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Authentication;
using Genbox.SimpleS3.Core.Builders;
using Genbox.SimpleS3.Core.Network.Requests;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using HttpMethod = Genbox.SimpleS3.Core.Abstracts.Enums.HttpMethod;

namespace Genbox.SimpleS3.Core.Benchmarks
{
    //This benchmark tests against https://github.com/FantasticFiasco/aws-signature-version-4

    public class DummyRequest : BaseRequest
    {
        public DummyRequest() : base(HttpMethod.GET)
        {
        }
    }

    [MemoryDiagnoser]
    [InProcess]
    public class AwsSignatureVersion4Benchmarks
    {
        private AuthorizationHeaderBuilder _headerBuilder;
        private DummyRequest _request;
        private HttpRequestMessage _request2;
        private ImmutableCredentials _credentials;

        [GlobalSetup]
        public void Setup()
        {
            {
                S3Config config = new S3Config();
                config.Region = AwsRegion.EuWest1;
                config.Credentials = new StringAccessKey("keyidkeyidkeyidkeyid", "accesskeyacceskey123accesskeyacceskey123");

                IOptions<S3Config> options = Options.Create(config);

                SigningKeyBuilder signingKeyBuilder = new SigningKeyBuilder(options, NullLogger<SigningKeyBuilder>.Instance);
                ScopeBuilder scopeBuilder = new ScopeBuilder(options);
                SignatureBuilder signatureBuilder = new SignatureBuilder(signingKeyBuilder, scopeBuilder, NullLogger<SignatureBuilder>.Instance, options);

                _headerBuilder = new AuthorizationHeaderBuilder(options, scopeBuilder, signatureBuilder, NullLogger<AuthorizationHeaderBuilder>.Instance);

                _request = new DummyRequest();
                _request.SetHeader(AmzHeaders.XAmzContentSha256, "UNSIGNED-PAYLOAD");
            }

            {
                _request2 = new HttpRequestMessage(System.Net.Http.HttpMethod.Get, "https://dummyurl");
                _credentials = new ImmutableCredentials("keyidkeyidkeyidkeyid", "accesskeyacceskey123accesskeyacceskey123", null);

                // Add required headers
                _request2.AddHeader(HeaderKeys.XAmzDateHeader, DateTime.UtcNow.ToIso8601BasicDateTime());

                // Add conditional headers
                _request2.AddHeaderIf(_credentials.UseToken, HeaderKeys.XAmzSecurityTokenHeader, _credentials.Token);
                _request2.AddHeaderIf(!_request2.Headers.Contains(HeaderKeys.HostHeader), HeaderKeys.HostHeader, _request2.RequestUri.Host);
            }
        }

        [Benchmark]
        public string SimpleS3()
        {
            return _headerBuilder.BuildAuthorization(_request);
        }

        [Benchmark]
        public string ASV4()
        {
            // Build the canonical request
            (string canonicalRequest, string signedHeaders) = CanonicalRequest.BuildAsync(_request2, null).Result;

            // Build the string to sign
            (string stringToSign, string credentialScope) = StringToSign.Build(
                DateTime.UtcNow,
                "eu-west-1",
                "S3",
                canonicalRequest);

            // Build the authorization header
            string authorizationHeader = AuthorizationHeader.Build(
                DateTime.UtcNow,
                "eu-west-1",
                "S3",
                _credentials,
                signedHeaders,
                credentialScope,
                stringToSign);

            return authorizationHeader;
        }
    }
}