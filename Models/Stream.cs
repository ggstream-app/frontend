using System;
using System.Collections.Generic;
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
        public Guid StreamKey { get; set; }

        /**
         * Collection this stream is a part of.
         */
        [Required]
        public Collection Collection { get; set; }

        /**
         * Full Stream Key used in OME, in the form of <Collection URL>_<StreamKey>.
         */
        public string FullStreamKey { 
            get
            {
                return $"{Collection.URL}_{StreamKey}";
            }
        }

        /**
         * Date the stream starts, currently used only for pulling latest.
         */
        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        /**
         * Date the stream ends, currently unused.
         */
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        /**
         * Whether to show How To instructions, wil override Collection-level setting.
         */
        public Boolean ShowHowTo { get; set; }

        /**
         * Teams link to show in UI, will override Collection-level setting.
         */
        public string TeamsLink { get; set; }   
    }
}
