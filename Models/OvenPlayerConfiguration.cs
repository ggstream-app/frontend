using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GGStream.Models
{
    public class OvenPlayerConfiguration
    {
        public string StreamKey { get; set; }

        public List<SvcInstance> Instances
        {
            get
            {
                return Configuration.GetSection("SvcInstances").Get<List<SvcInstance>>();
            }
        }

        public Boolean SupportsWebRTC
        {
            get
            {
                List<string> protocols = Configuration.GetSection("Protocols").Get<List<string>>();
                return protocols.Contains("WebRTC");
            }
        }

        /**
         * DASH is not currently supported
         */
        public Boolean SupportsDASH
        {
            get
            {
                List<string> protocols = Configuration.GetSection("Protocols").Get<List<string>>();
                return protocols.Contains("DASH");
            }
        }

        /**
         * HLS is not currently supported
         */
        public Boolean SupportsHLS
        {
            get
            {
                List<string> protocols = Configuration.GetSection("Protocols").Get<List<string>>();
                return protocols.Contains("HLS");
            }
        }

        public string EndpointJson
        {
            get
            {
                var instances = Instances;

                var endpointArr = instances.Select(i =>
                {
                    var protocol = i.Secure ? "wss" : "ws";
                    var port = i.Secure ? "3334" : "3333";

                    return new EndpointJson()
                    {
                        type = "webrtc",
                        file = $"{protocol}://{i.Endpoint}:{port}/app/{StreamKey}",
                        label = $"WebRTC - {i.Name}",
                    };
                }).ToArray();

                return JsonConvert.SerializeObject(endpointArr);
            }
        }

        private readonly IConfiguration Configuration;

        public OvenPlayerConfiguration(IConfiguration configuration)
        {
            Configuration = configuration;
        }
    }

    public class SvcInstance
    {
        public string Name { get; set; }
        public string Endpoint { get; set; }
        public Boolean Default { get; set; }
        public Boolean Secure { get; set; }
    }

    public class EndpointJson
    {
#pragma warning disable IDE1006 // Naming Styles
        public string type { get; set; }
        public string file { get; set; }
        public string label { get; set; }
#pragma warning restore IDE1006 // Naming Styles
    }
}