﻿using System.IO;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Authentication;
using Genbox.SimpleS3.Core.Abstracts.Constants;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Features;
using Genbox.SimpleS3.Core.Abstracts.Wrappers;
using Genbox.SimpleS3.Core.Authentication;
using Genbox.SimpleS3.Core.Common;
using Genbox.SimpleS3.Core.Internals.Extensions;
using JetBrains.Annotations;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Core.Network.RequestWrappers
{
    /// <summary>Add chunked streaming support to requests</summary>
    [PublicAPI]
    public sealed class ChunkedContentRequestStreamWrapper : IRequestStreamWrapper
    {
        private readonly IChunkedSignatureBuilder _chunkedSigBuilder;
        private readonly IOptions<S3Config> _config;
        private readonly ISignatureBuilder _signatureBuilder;

        public ChunkedContentRequestStreamWrapper(IOptions<S3Config> config, IChunkedSignatureBuilder chunkedSigBuilder, ISignatureBuilder signatureBuilder)
        {
            Validator.RequireNotNull(config, nameof(config));
            Validator.RequireNotNull(chunkedSigBuilder, nameof(chunkedSigBuilder));
            Validator.RequireNotNull(signatureBuilder, nameof(signatureBuilder));

            _chunkedSigBuilder = chunkedSigBuilder;
            _signatureBuilder = signatureBuilder;
            _config = config;
        }

        public bool IsSupported(IRequest request)
        {
            return _config.Value.PayloadSignatureMode == SignatureMode.StreamingSignature && request is ISupportStreaming;
        }

        public Stream Wrap(Stream input, IRequest request)
        {
            Validator.RequireNotNull(request, nameof(request));

            if (input == null)
                return null;

            //See https://docs.aws.amazon.com/AmazonS3/latest/API/sigv4-streaming.html
            request.SetHeader(HttpHeaders.ContentEncoding, "aws-chunked");
            request.SetHeader(AmzHeaders.XAmzDecodedContentLength, input.Length);
            request.SetHeader(AmzHeaders.XAmzContentSha256, "STREAMING-AWS4-HMAC-SHA256-PAYLOAD");

            byte[] seedSignature = _signatureBuilder.CreateSignature(request);
            return new ChunkedStream(_config, _chunkedSigBuilder, request, seedSignature, input);
        }
    }
}