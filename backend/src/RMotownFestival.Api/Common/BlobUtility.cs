﻿using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.Extensions.Options;
using System;

namespace RMotownFestival.Api.Common
{
    public class BlobUtility
    {
        private StorageSharedKeyCredential Credential { get; }
        private BlobServiceClient Client { get; }
        private BlobSettingsOptions Options { get; }

        public BlobUtility(StorageSharedKeyCredential credential,
            BlobServiceClient client, IOptions<BlobSettingsOptions> options)
        {
            Credential = credential;
            Client = client;
            Options = options.Value;
        }

        internal BlobContainerClient GetThumbsContainer()
        {
            return Client.GetBlobContainerClient(Options.ThumbsContainer);
        }

        public BlobContainerClient GetPicturesContainer()
        {
            return Client.GetBlobContainerClient(Options.PicturesContainer);
        }

        public string GetSasUri(BlobContainerClient container, string blobName)
        {
            //Create a SAS token that is valid for 2 min
            var sasBuilder = new BlobSasBuilder()
            {
                BlobContainerName = container.Name,
                BlobName = blobName,
                Resource = "b",
                StartsOn = DateTimeOffset.UtcNow.AddMinutes(-1),
                ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(2)
            };

            sasBuilder.SetPermissions(BlobAccountSasPermissions.Read);

            //Use the key to get the SAS token
            var sasToken = sasBuilder.ToSasQueryParameters(Credential).ToString();
            return $"{container.Uri.AbsoluteUri}/{blobName}?{sasToken}";
        }
    }
}
