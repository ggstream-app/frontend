using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GGStream.Models
{
    public class Stream
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string ID { get; set; }

        /**
         * The stream key, generated at create time as {url}-{nanoid:21}
         */
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [DisplayName("Stream Key")]
        public string StreamKey { get; set; }

        /**
         * The name of the stream.
         */
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [DisplayName("Stream Name")]
        [Required]
        public string Name { get; set; }

        /**
         * Collection this stream is a part of.
         */
        public Collection Collection { get; set; }
        public string CollectionURL { get; set; }

        /**
         * Date the stream starts, currently used only for pulling latest.
         */
        [DataType(DataType.DateTime)]
        [DisplayName("Start Date")]
        public DateTime? StartDate { get; set; }

        /**
         * Date the stream ends, currently unused.
         */
        [DataType(DataType.DateTime)]
        [DisplayName("End Date")]
        public DateTime? EndDate { get; set; }
    }
}
