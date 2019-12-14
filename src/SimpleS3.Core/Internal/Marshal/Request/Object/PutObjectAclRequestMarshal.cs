using System.IO;
using Genbox.SimpleS3.Abstracts;
using Genbox.SimpleS3.Abstracts.Constants;
using Genbox.SimpleS3.Abstracts.Marshal;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internal.Marshal.Request.Object
{
    [UsedImplicitly]
    internal class PutObjectAclRequestMarshal : IRequestMarshal<PutObjectAclRequest>
    {
        public Stream MarshalRequest(PutObjectAclRequest request, IS3Config config)
        {
            request.AddQueryParameter(AmzParameters.Acl, string.Empty);
            return null;
        }
    }
}