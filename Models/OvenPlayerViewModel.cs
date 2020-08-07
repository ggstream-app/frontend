using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

// ReSharper disable InconsistentNaming

namespace GGStream.Models
{
    public class OvenPlayerViewModel
    {
        private readonly IConfiguration _configuration;

        public OvenPlayerViewModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string StreamKey { get; set; }

        public List<SvcInstance> Instances => _configuration.GetSection("SvcInstances").Get<List<SvcInstance>>();

        public string EndpointJson
        {
            get
            {
                var instances = Instances;

                var endpointArr = instances.Select(i =>
                {
                    var allEndpoints = new List<EndpointJson>();

                    if (i.Protocols.WebRTC) allEndpoints.Add(GetWebRtcEndpoint(i));
                    if (i.Protocols.DASH) allEndpoints.Add(GetDashEndpoint(i));
                    if (i.Protocols.HLS) allEndpoints.Add(GetHlsEndpoint(i));

                    return allEndpoints;
                }).Aggregate(new List<EndpointJson>(), (acc, x) => acc.Concat(x).ToList());

                return JsonConvert.SerializeObject(endpointArr);
            }
        }

        private EndpointJson GetWebRtcEndpoint(SvcInstance instance)
        {
            var protocol = instance.Secure ? "wss" : "ws";
            var port = instance.Secure ? "3334" : "3333";

            return new EndpointJson
            {
                type = "webrtc",
                file = $"{protocol}://{instance.Endpoint}:{port}/app/{StreamKey}",
                label = $"{instance.Name} - WebRTC"
            };
        }

        private EndpointJson GetDashEndpoint(SvcInstance instance)
        {
            var protocol = instance.Secure ? "https" : "http";
            var port = instance.Secure ? "8443" : "8080";

            return new EndpointJson
            {
                type = "dash",
                file = $"{protocol}://{instance.Endpoint}:{port}/app/{StreamKey}/manifest.mpd",
                label = $"{instance.Name} - DASH"
            };
        }

        private EndpointJson GetHlsEndpoint(SvcInstance instance)
        {
            var protocol = instance.Secure ? "https" : "http";
            var port = instance.Secure ? "8443" : "8080";

            return new EndpointJson
            {
                type = "hls",
                file = $"{protocol}://{instance.Endpoint}:{port}/app/{StreamKey}/playlist.m3u8",
                label = $"{instance.Name} - HLS"
            };
        }
    }

    public class SvcInstance
    {
        public string Name { get; set; }
        public string Endpoint { get; set; }
        public bool Secure { get; set; }
        public InstanceProtocols Protocols { get; set; }
    }

    public class InstanceProtocols
    {
        public bool WebRTC { get; set; }
        public bool DASH { get; set; }
        public bool HLS { get; set; }
    }

    internal class EndpointJson
    {
#pragma warning disable IDE1006 // Naming Styles
        public string type { get; set; }
        public string file { get; set; }
        public string label { get; set; }
#pragma warning restore IDE1006 // Naming Styles
    }
}