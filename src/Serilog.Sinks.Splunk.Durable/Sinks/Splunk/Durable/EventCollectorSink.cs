// Serilog.Sinks.Seq Copyright 2016 Serilog Contributors
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
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;

namespace Serilog.Sinks.Splunk.Durable
{
    class EventCollectorSink
    : ILogEventSink, IDisposable
    {
        readonly HttpLogShipper _shipper;
        readonly Logger _sink;

        public EventCollectorSink(
            string serverUrl,
            string eventCollectorToken,
            string bufferBaseFilename,
            int batchPostingLimit,
            TimeSpan period,
            long? bufferSizeLimitBytes,
            long? eventBodyLimitBytes,
            HttpMessageHandler messageHandler,
            long? retainedInvalidPayloadsLimitBytes,
            ITextFormatter jsonFormatter = null)
        {
            if (serverUrl == null) throw new ArgumentNullException(nameof(serverUrl));
            if (bufferBaseFilename == null) throw new ArgumentNullException(nameof(bufferBaseFilename));
            if (jsonFormatter == null) jsonFormatter = new CompactSplunkJsonFormatter(renderTemplate: true);

            var fileSet = new FileSet(bufferBaseFilename);

            _shipper = new HttpLogShipper(
                fileSet,
                serverUrl,
                eventCollectorToken,
                batchPostingLimit,
                period,
                eventBodyLimitBytes,
                messageHandler,
                retainedInvalidPayloadsLimitBytes,
                bufferSizeLimitBytes);

            const long individualFileSizeLimitBytes = 100L * 1024 * 1024;
            _sink = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.File(jsonFormatter,
                        fileSet.RollingFilePathFormat,
                        rollingInterval: RollingInterval.Day,
                        fileSizeLimitBytes: individualFileSizeLimitBytes,
                        rollOnFileSizeLimit: true,
                        retainedFileCountLimit: null,
                        encoding: Encoding.UTF8)
                .CreateLogger();
        }

        public void Dispose()
        {
            _sink.Dispose();
            _shipper.Dispose();
        }

        public void Emit(LogEvent logEvent)
        {
            _sink.Write(logEvent);
        }
    }
}

#endif
