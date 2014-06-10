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
    public class Catalog : TestBase
    {
        [TestMethod, TestCategory("Anonymous")]
        public async Task TestSearchAndLookup()
        {
            // Search for "Daft Punk"
            ContentResponse searchResults = await Client.SearchAsync(Namespace.music, "Daft Punk", country: "US").Log();
            Assert.IsNotNull(searchResults, "The search response should not be null");
            AssertPaginatedListIsValid(searchResults.Artists, 1, 1);
            AssertPaginatedListIsValid(searchResults.Albums, 10, 20);
            AssertPaginatedListIsValid(searchResults.Tracks, 25, 100);
            Assert.IsNotNull(searchResults.Tracks.ContinuationToken, "Search results should contain continuation for tracks");

            // Get the 2nd page of track results
            ContentResponse continuedSearchResults =
                await Client.SearchContinuationAsync(Namespace.music, searchResults.Tracks.ContinuationToken).Log();
            Assert.IsNotNull(continuedSearchResults, "The continued search response should not be null");
            Assert.IsNull(continuedSearchResults.Artists, "The continued search response should not contain artists");
            Assert.IsNull(continuedSearchResults.Albums, "The continued search response should not contain albums");
            AssertPaginatedListIsValid(continuedSearchResults.Tracks, 25, 100);

            // List tracks in the first album
            Album firstAlbum = searchResults.Albums.Items.First();
            ContentResponse albumTrackResults = await Client.LookupAsync(firstAlbum.Id, extras: ExtraDetails.Tracks, country: "US").Log();
            AssertPaginatedListIsValid(albumTrackResults.Albums, 1);
            Album firstAlbumLookup = albumTrackResults.Albums.Items.First();
            Assert.AreEqual(firstAlbum.Id, firstAlbumLookup.Id, "Album ids should be the same");
            Assert.IsNotNull(firstAlbumLookup.Tracks, "Album should have tracks");
            Assert.IsNotNull(firstAlbumLookup.Tracks.Items, "Album should have tracks");
        }

        [TestMethod, TestCategory("Anonymous")]
        public async Task TestBrowseTopTracks()
        {
            // Get popular tracks in France
            ContentResponse browseResults = await Client.BrowseAsync(Namespace.music, ContentSource.Catalog, ItemType.Tracks, country: "FR").Log();
            Assert.IsNotNull(browseResults, "The browse response should not be null");
            AssertPaginatedListIsValid(browseResults.Tracks, 25, 100);

            // Get popular pop tracks in the US
            ContentResponse browsePopResults = await Client.BrowseAsync(Namespace.music, ContentSource.Catalog, ItemType.Tracks, genre: "Pop", country: "US").Log();
            Assert.IsNotNull(browsePopResults, "The browse response should not be null");
            AssertPaginatedListIsValid(browsePopResults.Tracks, 25, 100);
        }

        [TestMethod, TestCategory("Anonymous")]
        public async Task TestBrowseArtistTopTracks()
        {
            const string daftPunkId = "music.C61C0000-0200-11DB-89CA-0019B92A3933";

            // Get Daft Punk's top 5 tracks in Germany
            ContentResponse browseResults =
                await
                    Client.SubBrowseAsync(daftPunkId, ContentSource.Catalog, BrowseItemType.Artist,
                    ExtraDetails.TopTracks, maxItems: 5, country: "DE").Log();
            Assert.IsNotNull(browseResults, "The browse response should not be null");
            AssertPaginatedListIsValid(browseResults.Artists.Items.First().TopTracks, 5, 100);
        }

        [TestMethod, TestCategory("Anonymous")]
        public async Task TestBrowseGenres()
        {
            // This test will fail if this computer's current geography is not valid for Xbox Music catalog content or
            // if the geography could not be determined by the service. Provide the "country" parameter if that is the case.

            // Get available genres in this computer's current geography
            ContentResponse genreResults = await Client.BrowseGenresAsync(Namespace.music).Log();
            Assert.IsNotNull(genreResults, "The browse response should not be null");
            Console.WriteLine("Culture: {0}", genreResults.Culture);
            Assert.IsNotNull(genreResults.Genres, "The browse response should contain genres");
            Assert.IsTrue(0 < genreResults.Genres.Count, "The browse response should contain at least one genre");
            Assert.IsNotNull(genreResults.Culture, "The genre response should contain the applicable culture");
        }
    }
}
