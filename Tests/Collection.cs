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
using Microsoft.Xbox.Music.Platform.Client;

namespace Tests
{
    [TestClass]
    public class Collection : TestBase
    {
        [TestMethod, TestCategory("Anonymous")]
        public async Task TestLookupPublicPlaylist()
        {
            // Tip: You get your own playlistId by opening your playlist on http://music.xbox.com
            //      If the page is http://music.xbox.com/playlist/great-music/66cd8e9d-802a-00fe-364d-3ead4f82facf
            //      the id is music.playlist.66cd8e9d-802a-00fe-364d-3ead4f82facf .
            const string playlistId = "music.playlist.0016e20b-80c0-00fe-fac0-1a47365516d1";

            // Get playlist contents as viewed from the US
            ContentResponse playlistUsResponse =
                await Client.LookupAsync(playlistId, ContentSource.Collection, country: "US").Log();
            foreach (var track in playlistUsResponse.Playlists.Items.First().Tracks.Items)
            {
                Console.WriteLine("  Track {0} can be {1} in the US", track.Id, String.Join(" and ", track.Rights));
            }

            // Get playlist contents as viewed from Brasil
            // Note that rights (such as Stream, FreeStream and Purchase) and collection item ids can be country specific
            ContentResponse playlistBrResponse =
                await Client.LookupAsync(playlistId, ContentSource.Collection, country: "BR").Log();
            foreach (var track in playlistBrResponse.Playlists.Items.First().Tracks.Items)
            {
                Console.WriteLine("  Track {0} can be {1} in Brasil", track.Id, String.Join(" and ", track.Rights));
            }
        }

        [TestMethod, TestCategory("Authenticated")]
        public async Task TestBrowsePlaylists()
        {
            // Get all the user's playlists
            ContentResponse browseResults =
                await AuthenticatedClient.BrowseAsync(Namespace.music, ContentSource.Collection, ItemType.Playlists).Log();
            Assert.IsNotNull(browseResults, "The browse response should not be null");
            AssertPaginatedListIsValid(browseResults.Playlists, 1);
        }
    }
}
