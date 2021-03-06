using System.IO;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Constants;
using Genbox.SimpleS3.Core.Abstracts.Marshallers;
using Genbox.SimpleS3.Core.Network.Requests.Buckets;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Requests.Buckets
{
    [UsedImplicitly]
    internal class GetBucketLockConfigurationRequestMarshal : IRequestMarshal<GetBucketLockConfigurationRequest>
    {
        public Stream MarshalRequest(GetBucketLockConfigurationRequest request, IConfig config)
        {
            request.SetQueryParameter(AmzParameters.ObjectLock, string.Empty);
            return null;
        }
    }
}