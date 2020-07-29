using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
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
         * Base color to use for UI.
         */
        public string BaseColor { get; set; }

        [NotMapped]
        public string LightColor { 
            get
            {
                return ChangeColorBrightness((float)0.3);
            } 
        }

        [NotMapped]
        public string DarkColor
        {
            get
            {
                return ChangeColorBrightness((float)-0.3);
            }
        }

        /**
         * Whether this collection is private (need stream key to watch) or public (pull latest stream).
         */
        public Boolean Private { get; set; }

        /**
         * Which instructions to show. None means no button is shown.
         */
        public InstructionType InstructionType { get; set; }

        /**
         * Teams link to show in UI. If not set, won't show button.
         */
        public string CallLink { get; set; }

        /**
         * All streams for this collection
         */
        public List<Stream> Streams { get; set; }

        private string ChangeColorBrightness(float correctionFactor)
        {
            if (BaseColor == null)
            {
                return null;
            }

            var color = ColorTranslator.FromHtml(BaseColor);

            float red = (float)color.R;
            float green = (float)color.G;
            float blue = (float)color.B;

            if (correctionFactor < 0)
            {
                correctionFactor = 1 + correctionFactor;
                red *= correctionFactor;
                green *= correctionFactor;
                blue *= correctionFactor;
            }
            else
            {
                red = (255 - red) * correctionFactor + red;
                green = (255 - green) * correctionFactor + green;
                blue = (255 - blue) * correctionFactor + blue;
            }

            var newColor = Color.FromArgb(color.A, (int)red, (int)green, (int)blue);
            return ColorTranslator.ToHtml(newColor);
        }
    }
}

public enum InstructionType
{
    None = 0,
    Jackbox
}
