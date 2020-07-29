using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GGStream.Models
{
    public class HomeModel
    {
        public List<Collection> PublicCollections { get; set; }
        public List<Stream> CurrentPublicStreams { get; set; }
    }
}
