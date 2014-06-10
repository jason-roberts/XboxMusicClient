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
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.System.Profile;
using Microsoft.Xbox.Music.Platform.Client;
using Microsoft.Xbox.Music.Platform.Contract.DataModel;

namespace Microsoft.Xbox.Music.Platform.ClientRT
{
    /// <summary>
    /// Useful Windows Runtime specific extensions for the IXboxMusicClient
    /// </summary>
    public static class XboxMusicClientRtExtensions
    {
        private static readonly Lazy<string> clientInstanceId = new Lazy<string>(ComputeClientInstanceId);

        /// <summary>
        /// A valid clientInstanceId string. This string is specific to the current machine, user and application.
        /// </summary>
        public static string ClientInstanceId
        {
            get { return clientInstanceId.Value; }
        }

        /// <summary>
        /// Stream a media
        /// Access to this API is restricted under the terms of the Xbox Music API Pilot program (http://music.xbox.com/developer/pilot).
        /// </summary>
        /// <param name="client">An IXboxMusicClient instance to extend</param>
        /// <param name="id">Id of the media to be streamed</param>
        /// <returns>Stream response containing the url, expiration date and content type</returns>
        static public Task<StreamResponse> StreamAsync(this IXboxMusicClient client, string id)
        {
            return client.StreamAsync(id, ClientInstanceId);
        }

        /// <summary>
        /// Get a 30s preview of a media
        /// </summary>
        /// <param name="client">An IXboxMusicClient instance to extend</param>
        /// <param name="id">Id of the media to be streamed</param>
        /// <param name="country">ISO 2 letter code.</param>
        /// <returns>Stream response containing the url, expiration date and content type</returns>
        static public Task<StreamResponse> PreviewAsync(this IXboxMusicClient client, string id, string country = null)
        {
            return client.PreviewAsync(id, ClientInstanceId, country);
        }

        /// <summary>
        /// Compute a stable application specific client instance id string for use as "clientInstanceId" parameters in IXboxMusicClient
        /// </summary>
        /// <returns>A valid clientInstanceId string. This string is specific to the current machine, user and application.</returns>
        private static string ComputeClientInstanceId()
        {
            // Generate a somewhat stable application instance id
            HardwareToken ashwid = HardwareIdentification.GetPackageSpecificToken(null);
            byte[] id = ashwid.Id.ToArray();
            string idstring = Package.Current.Id.Name + ":";
            for (int i = 0; i < id.Length; i += 4)
            {
                short what = BitConverter.ToInt16(id, i);
                short value = BitConverter.ToInt16(id, i + 2);
                // Only include stable components in the id
                // http://msdn.microsoft.com/en-us/library/windows/apps/jj553431.aspx
                const int cpuId = 1;
                const int memorySize = 2;
                const int diskSerial = 3;
                const int bios = 9;
                if (what == cpuId || what == memorySize || what == diskSerial || what == bios)
                {
                    idstring += value.ToString("X4");
                }
            }
            return idstring.PadRight(32, 'X');
        }
    }
}
