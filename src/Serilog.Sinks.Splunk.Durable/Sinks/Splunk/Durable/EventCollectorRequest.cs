﻿// Serilog.Sinks.Seq Copyright 2017 Serilog Contributors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#if DURABLE

using System;
using System.Net.Http;
using System.Text;

#if HRESULTS
using System.Runtime.InteropServices;
#endif

namespace Serilog.Sinks.Splunk.Durable
{
    internal class EventCollectorRequest : HttpRequestMessage
    {
        internal EventCollectorRequest(string splunkHost, string jsonPayLoad, string uri = "services/collector")
        {
            var hostUrl = $@"{splunkHost}/{uri}";

            if (splunkHost.Contains("services/collector"))
            {
                hostUrl = $@"{splunkHost}";
            }

            var stringContent = new StringContent(jsonPayLoad, Encoding.UTF8, "application/json");
            RequestUri = new Uri(hostUrl);
            Content = stringContent;
            Method = HttpMethod.Post;
        }
    }
}

#endif
