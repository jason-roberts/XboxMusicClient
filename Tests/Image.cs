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
    public class Image : TestBase
    {
        [TestMethod, TestCategory("Anonymous")]
        public async Task TestArtistImage()
        {
            const string katyPerryId = "music.97e60200-0200-11db-89ca-0019b92a3933";

            // Lookup Katy Perry's information
            ContentResponse lookupResponse = await Client.LookupAsync(katyPerryId, country: "US").Log();
            Artist artist = lookupResponse.Artists.Items.First();

            // Get a 1920x1080 image URL
            string squareImageUrl = artist.GetImageUrl(1920, 1080);
            Console.WriteLine("1920x1080 image URL: {0}", squareImageUrl);

            // Get the default image URL
            string defaultImageUrl = artist.ImageUrl;
            Console.WriteLine("Default image URL: {0}", defaultImageUrl);
        }
    }
}
