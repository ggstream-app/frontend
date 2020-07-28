using GGStream.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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

        public string EndpointJson
        {
            get
            {
                var instances = Instances;

                var endpointArr = instances.Select(i =>
                {
                    List<EndpointJson> allEndpoints = new List<EndpointJson>();

                    if (i.Protocols.WebRTC)
                    {
                        allEndpoints.Add(GetWebRTCEndpoint(i));
                    }
                    if (i.Protocols.DASH)
                    {
                        allEndpoints.Add(GetDASHEndpoint(i));
                    }
                    if (i.Protocols.HLS)
                    {
                        allEndpoints.Add(GetHLSEndpoint(i));
                    }

                    return allEndpoints;
                }).Aggregate(new List<EndpointJson>(), (acc, x) => acc.Concat(x).ToList());

                return JsonConvert.SerializeObject(endpointArr);
            }
        }

        private readonly IConfiguration Configuration;

        public OvenPlayerConfiguration(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private EndpointJson GetWebRTCEndpoint(SvcInstance instance)
        {
            var protocol = instance.Secure ? "wss" : "ws";
            var port = instance.Secure ? "3334" : "3333";

            return new EndpointJson()
            {
                type = "webrtc",
                file = $"{protocol}://{instance.Endpoint}:{port}/app/{StreamKey}",
                label = $"{instance.Name} - WebRTC",
            };
        }

        private EndpointJson GetDASHEndpoint(SvcInstance instance)
        {
            var protocol = instance.Secure ? "https" : "http";
            var port = instance.Secure ? "8443" : "8080";

            return new EndpointJson()
            {
                type = "dash",
                file = $"{protocol}://{instance.Endpoint}:{port}/app/{StreamKey}/manifest.mpd",
                label = $"{instance.Name} - DASH",
            };
        }

        private EndpointJson GetHLSEndpoint(SvcInstance instance)
        {
            var protocol = instance.Secure ? "https" : "http";
            var port = instance.Secure ? "8443" : "8080";

            return new EndpointJson()
            {
                type = "hls",
                file = $"{protocol}://{instance.Endpoint}:{port}/app/{StreamKey}/playlist.m3u8",
                label = $"{instance.Name} - HLS",
            };
        }
    }

    public class SvcInstance
    {
        public string Name { get; set; }
        public string Endpoint { get; set; }
        public Boolean Default { get; set; }
        public Boolean Secure { get; set; }
        public InstanceProtocols Protocols { get; set; }
    }

    public class InstanceProtocols
    {
        public Boolean WebRTC { get; set; }
        public Boolean DASH { get; set; }
        public Boolean HLS { get; set; }
    }

    class EndpointJson
    {
#pragma warning disable IDE1006 // Naming Styles
        public string type { get; set; }
        public string file { get; set; }
        public string label { get; set; }
#pragma warning restore IDE1006 // Naming Styles
    }
}
