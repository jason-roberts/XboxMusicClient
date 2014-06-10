// Copyright (c) Microsoft Corporation
// All rights reserved. 
//
// Licensed under the Apache License, Version 2.0 (the ""License""); you may
// not use this file except in compliance with the License. You may obtain a
// copy of the License at http://www.apache.org/licenses/LICENSE-2.0 
//
// THIS CODE IS PROVIDED *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY
// IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE,
// MERCHANTABLITY OR NON-INFRINGEMENT. 
//
// See the Apache Version 2.0 License for specific language governing
// permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xbox.Music.Platform.Contract.DataModel;

namespace Tests
{
    [TestClass]
    public class Stream : TestBase
    {
        [TestMethod, TestCategory("Anonymous")]
        public async Task TestPreview()
        {
            // Get popular tracks in Great Britain
            ContentResponse browseResults = await Client.BrowseAsync(Namespace.music, ContentSource.Catalog, ItemType.Tracks, country: "GB").Log();
            Assert.IsNotNull(browseResults, "The browse response should not be null");
            AssertPaginatedListIsValid(browseResults.Tracks, 25, 100);

            // Request a preview stream URL for the first one
            Track track = browseResults.Tracks.Items.First();
            StreamResponse streamResponse = await Client.PreviewAsync(track.Id, ClientInstanceId, country: "GB").Log();
            Assert.IsNotNull(streamResponse, "The preview stream URL response should not be null");
            Assert.IsNotNull(streamResponse.Url, "The preview stream URL should not be null");
            Assert.IsNotNull(streamResponse.ContentType, "The preview stream content type should not be null");
        }

        [TestMethod, TestCategory("Authenticated")]
        public async Task TestStream()
        {
            // Check that the user has an Xbox Music Pass subscription
            UserProfileResponse userProfileResponse = await AuthenticatedClient.GetUserProfileAsync(Namespace.music).Log();
            // Beware: HasSubscription is bool?. You want != true instead of == false
            if (userProfileResponse.HasSubscription != true)
            {
                Assert.Inconclusive("The user doesn't have an Xbox Music Pass subscription. Cannot stream from catalog.");
            }

            // Get popular tracks in the user's country
            ContentResponse browseResults = await AuthenticatedClient.BrowseAsync(Namespace.music, ContentSource.Catalog, ItemType.Tracks).Log();
            Assert.IsNotNull(browseResults, "The browse response should not be null");
            AssertPaginatedListIsValid(browseResults.Tracks, 25, 100);

            // Stream the first streamable track
            Track track = browseResults.Tracks.Items.First(t => t.Rights.Contains("Stream"));
            StreamResponse streamResponse = await AuthenticatedClient.StreamAsync(track.Id, ClientInstanceId).Log();
            Assert.IsNotNull(streamResponse, "The stream URL response should not be null");
            Assert.IsNotNull(streamResponse.Url, "The stream URL should not be null");
            Assert.IsNotNull(streamResponse.ContentType, "The stream content type should not be null");
            Assert.IsNotNull(streamResponse.ExpiresOn, "The stream expiry date should not be null");
        }
    }
}
