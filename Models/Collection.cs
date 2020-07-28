using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GGStream.Models
{
    public class Collection
    {
        /**
         * URL path to use when querying for this collection.
         */
        [Required]
        [Key]
        public string URL { get; set; }

        /**
         * Public name, used in branding.
         */
        [Required]
        public string Name { get; set; }

        /**
         * Base color to use for UI. Currently unused.
         */
        public string BaseColor { get; set; }

        /**
         * Whether this collection is private (need stream key to watch) or public (pull latest stream).
         */
        public Boolean Private { get; set; }

        /**
         * Whether to show How To instructions.
         */
        public Boolean ShowHowTo { get; set; }

        /**
         * Teams link to show in UI. If not set, won't show button.
         */
        public string TeamsLink { get; set; }

        /**
         * All streams for this collection
         */
        public List<Stream> Streams { get; set; }
    }
}