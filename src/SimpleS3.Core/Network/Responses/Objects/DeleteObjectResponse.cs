﻿using Genbox.SimpleS3.Core.Network.Responses.Properties;
using Genbox.SimpleS3.Core.Network.SharedProperties;

namespace Genbox.SimpleS3.Core.Network.Responses.Objects
{
    public class DeleteObjectResponse : BaseResponse, IHasRequestCharged, IHasVersionId, IHasDeleteMarker
    {
        public bool IsDeleteMarker { get; internal set; }
        public string VersionId { get; internal set; }
        public bool RequestCharged { get; internal set; }
    }
}