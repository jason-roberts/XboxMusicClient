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
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xbox.Music.Platform.Client;
using Microsoft.Xbox.Music.Platform.Contract.DataModel;

namespace Tests
{
    [TestClass]
    public class GettingStarted
    {
        [TestMethod, TestCategory("Anonymous"), TestCategory("GettingStarted")]
        public async Task TestGettingStarted()
        {
            // Start by registering an Azure Data Market client ID and secret ( see http://music.xbox.com/developer )

            // Create a client
            //IXboxMusicClient client = XboxMusicClientFactory.CreateXboxMusicClient("MyClientId", "YourClientSecretYourClientSecretYourSecret=");
            string clientid = ConfigurationManager.AppSettings["clientid"];
            string clientsecret = ConfigurationManager.AppSettings["clientsecret"];
            IXboxMusicClient client = XboxMusicClientFactory.CreateXboxMusicClient(clientid, clientsecret);

            // Use null to get your current geography.
            // Specify a 2 letter country code (such as "US" or "DE") to force a specific country.
            string country = null;

            // Search for albums in your current geography
            ContentResponse searchResponse = await client.SearchAsync(Namespace.music, "Foo Fighters", filter: SearchFilter.Albums, maxItems: 5, country: country);
            Console.WriteLine("Found {0} albums", searchResponse.Albums.TotalItemCount);
            foreach (Album albumResult in searchResponse.Albums.Items)
            {
                Console.WriteLine("{0}", albumResult.Name);
            }

            // List tracks in the first album
            Album album = searchResponse.Albums.Items[0];
            ContentResponse lookupResponse = await client.LookupAsync(album.Id, extras: ExtraDetails.Tracks, country: country);

            // Display information about the album
            album = lookupResponse.Albums.Items[0];
            Console.WriteLine("Album: {0} (link: {1}, image: {2})", album.Name, album.GetLink(ContentExtensions.LinkAction.Play), album.GetImageUrl(800, 800));
            foreach (Contributor contributor in album.Artists)
            {
                Artist artist = contributor.Artist;
                Console.WriteLine("Artist: {0} (link: {1}, image: {2})", artist.Name, artist.GetLink(), artist.GetImageUrl(1920, 1080));
            }
            foreach (Track track in album.Tracks.Items)
            {
                Console.WriteLine("Track: {0} - {1}", track.TrackNumber, track.Name);
            }
        }
    }
}
