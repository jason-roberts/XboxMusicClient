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
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xbox.Music.Platform.Client;
using Microsoft.Xbox.Music.Platform.Contract.AuthenticationDataModel;
using Microsoft.Xbox.Music.Platform.Contract.DataModel;

namespace Tests
{
    public abstract class TestBase
    {
        protected static IXboxMusicClient Client { get; private set; }

        protected static IXboxMusicClient AuthenticatedClient
        {
            get
            {
                string xtoken = ConfigurationManager.AppSettings["xtoken"];
                if (String.IsNullOrEmpty(xtoken) || xtoken.Length < 20)
                {
                    Assert.Inconclusive("The XToken header value should be set in App.config for user authenticated tests");
                }
                return Client.CreateUserAuthenticatedClient(new TestXToken {AuthorizationHeaderValue = xtoken});
            }
        }

        // When implementing real calls, use a unique stable client instance id per client id.
        // See XboxMusicClientRtExtensions for an example when using Windows Runtime.
        protected static string ClientInstanceId { get { return "XboxMusicClientTests12345678901234567890"; } }

        static TestBase()
        {
            string clientid = ConfigurationManager.AppSettings["clientid"];
            string clientsecret = ConfigurationManager.AppSettings["clientsecret"];
            Assert.IsNotNull(clientid, "The client id should be set in App.config");
            Assert.IsNotNull(clientsecret, "The client secret should be set in App.config");
            Assert.IsFalse(clientsecret.Contains("%"), "The client secret should not be URL encoded");
            Client = XboxMusicClientFactory.CreateXboxMusicClient(clientid, clientsecret);
        }

        protected void AssertPaginatedListIsValid<TContent>(PaginatedList<TContent> list, int minItems,
            int? minTotalItems = null)
        {
            Assert.IsNotNull(list, "Results should contain " + typeof (TContent));
            Assert.IsNotNull(list.Items, "Results should contain " + typeof (TContent) + " items");
            Assert.IsTrue(minItems <= list.Items.Count,
                "Results should contain more than " + minItems + " " + typeof (TContent) + " items");
            if (minTotalItems != null)
                Assert.IsTrue(minTotalItems <= list.TotalItemCount,
                    "The total number of  " + typeof (TContent) + " should be greater than " + minTotalItems);
        }

        private class TestXToken : IXToken
        {
            public string AuthorizationHeaderValue { get; set; }
            public DateTime NotAfter { get; set; }
            public DateTime IssueInstant { get; set; }

            public Task<bool> RefreshAsync(CancellationToken cancellationToken)
            {
                return Task.FromResult(false);
            }
        }
    }
}
