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
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xbox.Music.Platform.Contract.DataModel;

namespace Tests
{
    // Extension methods used to add output when executing tests
    public static class TestBaseExtensions
    {
        private static async Task<TResponse> LogResponse<TResponse>(this Task<TResponse> responseTask, Action<TResponse> log) where TResponse : BaseResponse
        {
            TResponse response = await responseTask;
            if (response == null)
            {
                Console.WriteLine("Response is null");
            }
            else
            {
                log(response);

                if (response.Error != null)
                {
                    Console.WriteLine("  Error: {0} {1}", response.Error.ErrorCode, response.Error.Description);
                    Console.WriteLine("         {0}", response.Error.Message);
                }
            }
            return response;
        }

        // Log top level content items and sub tracks for debug purposes
        public static Task<ContentResponse> Log(this Task<ContentResponse> responseTask)
        {
            return LogResponse(responseTask, response =>
            {
                Console.WriteLine("Response top level content:");
                foreach (Content content in response.GetAllTopLevelContent())
                {
                    Playlist playlist = content as Playlist;
                    Artist artist = content as Artist;
                    Album album = content as Album;
                    Track track = content as Track;
                    if (album != null)
                        Console.WriteLine("  {0} {1}: {2}, {3}", content.GetType().Name, content.Id, content.Name,
                            String.Join(" and ", album.Artists.Select(contributor => contributor.Artist.Name)));
                    else if (track != null)
                        Console.WriteLine("  {0} {1}: {2}, {3}, {4}", content.GetType().Name, content.Id, content.Name,
                            track.Album.Name,
                            String.Join(" and ", track.Artists.Select(contributor => contributor.Artist.Name)));
                    else
                        Console.WriteLine("  {0} {1}: {2}", content.GetType().Name, content.Id, content.Name);

                    IPaginatedList<Track> tracks = playlist != null
                        ? playlist.Tracks
                        : album != null
                            ? album.Tracks
                            : artist != null
                                ? artist.TopTracks
                                : null;
                    if (tracks != null && tracks.ReadOnlyItems != null)
                    {
                        Console.WriteLine("  Contained tracks:");
                        foreach (Track subTrack in tracks.ReadOnlyItems)
                        {
                            Console.WriteLine("    {0} {1}: {2}, {3}, {4}", subTrack.GetType().Name, subTrack.Id,
                                subTrack.Name, subTrack.Album != null ? subTrack.Album.Name : null,
                                String.Join(" and ", subTrack.Artists.Select(contributor => contributor.Artist.Name)));
                        }
                    }
                }

                if (response.Genres != null)
                {
                    foreach (var genre in response.Genres)
                    {
                        Console.WriteLine(" Genre: {0}", genre);
                    }
                }
            });
        }

        public static Task<StreamResponse> Log(this Task<StreamResponse> responseTask)
        {
            return LogResponse(responseTask, response =>
            {
                Console.WriteLine("Stream response:");
                Console.WriteLine("  URL: {0}", response.Url);
                Console.WriteLine("  Content type: {0}", response.ContentType);
                Console.WriteLine("  Expiration date: {0}", response.ExpiresOn);
            });
        }

        public static Task<UserProfileResponse> Log(this Task<UserProfileResponse> responseTask)
        {
            return LogResponse(responseTask, response =>
            {
                Console.WriteLine("User profile response:");
                Console.WriteLine("  Has subscription: {0}", response.HasSubscription);
                Console.WriteLine("  Culture: {0}", response.Culture);
                Console.WriteLine("  Is subscription available for purchase: {0}", response.IsSubscriptionAvailableForPurchase);
            });
        }
    }
}