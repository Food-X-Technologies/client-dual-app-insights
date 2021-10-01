* using Microsoft.ApplicationInsights;
* 		using Microsoft.ApplicationInsights.Channel;
* 		using Microsoft.ApplicationInsights.Extensibility;
* 		using Microsoft.ApplicationInsights.Extensibility.Implementation;
* 		using System;
* 		
* 		
* 		namespace MultipleAIAccounts
* 		{
* 		    class Program
* 		    {
* 		        static void Main(string[] args)
* 		        {
* 		            var config = TelemetryConfiguration.Active;
* 		
* 		            var channel1 = new InMemoryChannel();
* 		            config.DefaultTelemetrySink.TelemetryChannel = channel1;
* 		
* 		            var chainBuilder1 = new TelemetryProcessorChainBuilder(config, config.DefaultTelemetrySink);
* 		            chainBuilder1.Use((next) =>
* 		            {
* 		                var p1 = new StubTelemetryProcessor(next, "1");
* 		                p1.OnProcess = (telemetry) => {
* 		                    telemetry.Context.InstrumentationKey = "Instrumentation-key-1";
* 		                };
* 		                return p1;
* 		            });
* 		            config.DefaultTelemetrySink.TelemetryProcessorChainBuilder = chainBuilder1;
* 		
* 		            var channel2 = new InMemoryChannel();
* 		            var sink2 = new TelemetrySink(config, channel2);
* 		            config.TelemetrySinks.Add(sink2);
* 		
* 		            sink2.TelemetryProcessorChainBuilder = new TelemetryProcessorChainBuilder(config, sink2);
* 		            sink2.TelemetryProcessorChainBuilder.Use((next) =>
* 		            {
* 		                var p2 = new StubTelemetryProcessor(next, "2");
* 		                p2.OnProcess = (telemetry) => {
* 		                    telemetry.Context.InstrumentationKey = "Instrumentation-key-2";
* 		                };
* 		                return p2;
* 		            });
* 		
* 		            config.TelemetryProcessorChainBuilder.Build();
* 		            var client = new TelemetryClient(config);
* 		            string time = DateTime.Now.ToString();
* 		            client.TrackTrace($"Hi, I am a new message at {time}");
* 		            channel1.Flush();
* 		            channel2.Flush();
* 		            client.Flush();
* 		        }
* 		    }
* 		    public sealed class StubTelemetryProcessor : ITelemetryProcessor, IDisposable
* 		    {
* 		        private ITelemetryProcessor next;
* 		
* 		        public StubTelemetryProcessor(ITelemetryProcessor next, string name)
* 		        {
* 		            this.name = name;
* 		            this.next = next;
* 		            this.OnDispose = () => { };
* 		            this.OnProcess = (unusedTelemetry) => { };
* 		        }
* 		
* 		        public Action<ITelemetry> OnProcess { get; set; }
* 		
* 		        public Action OnDispose { get; set; }
* 		
* 		        public string name { get; set; }
* 		
* 		        public void Process(ITelemetry telemetry)
* 		        {
* 		            this.OnProcess(telemetry);
* 		            if (this.next != null)
* 		            {
* 		                this.next.Process(telemetry);
* 		            }
* 		        }
* 		
* 		        public void Dispose()
* 		        {
* 		            this.OnDispose();
* 		        }
* 		    }
* 		
* 		}
