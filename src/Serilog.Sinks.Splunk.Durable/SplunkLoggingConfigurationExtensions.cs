// Serilog.Sinks.Seq Copyright 2016 Serilog Contributors
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using System.Net.Http;
using Serilog.Sinks.PeriodicBatching;
using Serilog.Sinks.Splunk.Durable;
using Serilog.Formatting;

namespace Serilog
{
    /// <summary>
    /// Extends Serilog configuration to write events to Splunk.
    /// </summary>
    public static class SplunkLoggingConfigurationExtensions
    {
        /// <summary>
        /// Writes to splunk using HTTP Event Collector
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="splunkHost"></param>
        /// <param name="eventCollectorToken"></param>
        /// <param name="bufferFileFullName"></param>
        /// <param name="restrictedToMinimumLevel"></param>
        /// <param name="batchIntervalInSeconds"></param>
        /// <param name="batchSizeLimit"></param>
        /// <param name="bufferSizeLimitBytes"></param>
        /// <param name="eventBodyLimitBytes"></param>
        /// <param name="retainedInvalidPayloadsLimitBytes"></param>
        /// <param name="messageHandler"></param>
        /// <param name="levelSwitch"></param>
        /// <param name="jsonFormatter">The text formatter used to render log events into a JSON format for consumption by Splunk</param>
        /// <returns></returns>
        public static LoggerConfiguration SplunkEventCollector(
           this LoggerSinkConfiguration configuration,
           string splunkHost,
           string eventCollectorToken,
           string bufferFileFullName,
           LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
           int batchIntervalInSeconds = 2,
           int batchSizeLimit = 100,
           long? bufferSizeLimitBytes = null,
           long? eventBodyLimitBytes = 512 * 1024,
           long? retainedInvalidPayloadsLimitBytes = null,
           HttpMessageHandler messageHandler = null,
           LoggingLevelSwitch levelSwitch = null,
           ITextFormatter jsonFormatter = null)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            var eventCollectorSink = new EventCollectorSink(
                splunkHost,
                eventCollectorToken,
                bufferFileFullName,
                batchSizeLimit,
                TimeSpan.FromSeconds(batchIntervalInSeconds),
                bufferSizeLimitBytes,
                eventBodyLimitBytes,
                messageHandler,
                retainedInvalidPayloadsLimitBytes,
                jsonFormatter);

            return configuration.Sink(eventCollectorSink, restrictedToMinimumLevel, levelSwitch);
        }
    }
}
