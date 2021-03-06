﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using Xunit;
using Xunit.Abstractions;

namespace Genbox.SimpleS3.Core.Tests.OnlineTests.Buckets
{
    public class BucketLifecycleConfigurationTests : OnlineTestBase
    {
        public BucketLifecycleConfigurationTests(ITestOutputHelper helper) : base(helper)
        {
        }

        [Fact]
        public async Task PutLifecycleConfigurationTest()
        {
            await CreateTempBucketAsync(async x =>
            {
                IList<S3Rule> rules = new List<S3Rule>
                {
                    new S3Rule(true, "Transition logs after 30 days to StandardIa and after 60 days to OneZoneIa")
                    {
                        Transitions = new List<S3Transition>
                        {
                            new S3Transition(30, StorageClass.StandardIa),
                            new S3Transition(60, StorageClass.OneZoneIa)
                        },
                        Filter = new S3Filter { Prefix = "logs/" }
                    },
                    new S3Rule(true, "Expire temp folder after 5 days")
                    {
                        Expiration = new S3Expiration(5),
                        Filter = new S3Filter { Prefix = "temp/" }
                    }
                };

                PutBucketLifecycleConfigurationResponse putResp = await BucketClient.PutBucketLifecycleConfigurationAsync(x, rules).ConfigureAwait(false);
                Assert.True(putResp.IsSuccess);

            }).ConfigureAwait(false);
        }
    }
}