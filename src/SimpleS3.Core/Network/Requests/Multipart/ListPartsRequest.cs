﻿using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Requests.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Requests.Multipart
{
    /// <summary>This operation lists the parts that have been uploaded for a specific multipart upload.</summary>
    public class ListPartsRequest : BaseRequest, IHasUploadId, IHasRequestPayer, IHasBucketName, IHasObjectKey
    {
        public ListPartsRequest(string bucketName, string objectKey, string uploadId) : base(HttpMethod.GET)
        {
            BucketName = bucketName;
            ObjectKey = objectKey;
            UploadId = uploadId;
        }

        /// <summary>Requests Amazon S3 to encode the response and specifies the encoding method to use.</summary>
        public EncodingType EncodingType { get; set; }

        /// <summary>Sets the maximum number of parts to return in the response body.</summary>
        public int? MaxParts { get; set; }

        /// <summary>Specifies the part after which listing should begin. Only parts with higher part numbers will be listed.</summary>
        public string PartNumberMarker { get; set; }

        public string BucketName { get; set; }
        public string ObjectKey { get; set; }
        public Payer RequestPayer { get; set; }
        public string UploadId { get; set; }
    }
}