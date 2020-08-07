using System.Collections.Generic;

namespace GGStream.Models
{
    public class HomeViewModel
    {
        public List<Collection> PublicCollections { get; set; }
        public List<Stream> CurrentPublicStreams { get; set; }
    }
}