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
    public class Link : TestBase
    {
        [TestMethod, TestCategory("Anonymous")]
        public async Task TestLinks()
        {
            const string katyPerryId = "music.97e60200-0200-11db-89ca-0019b92a3933";

            // Lookup Katy Perry's information, including latest album releases
            ContentResponse lookupResponse = await Client.LookupAsync(katyPerryId, extras: ExtraDetails.Albums, country: "US").Log();
            Artist artist = lookupResponse.Artists.Items.First();

            // Create a link to Katy Perry's artist page in an Xbox Music client
            string artistPageDeepLink = artist.Link;
            Console.WriteLine("Artist page deep link: {0}", artistPageDeepLink);
            Assert.IsNotNull(artistPageDeepLink, "The artist page deep link should not be null");

            // Create a link which starts playback of Katy Perry's latest album in the US (exclude singles and EPs)
            Album album = artist.Albums.Items.First(a => a.AlbumType == "Album");
            string albumPlayDeepLink = album.GetLink(ContentExtensions.LinkAction.Play);
            Console.WriteLine("Album play deep link: {0}", albumPlayDeepLink);
            Assert.IsNotNull(albumPlayDeepLink, "The album play deep link should not be null");
        }
    }
}
