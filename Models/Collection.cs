using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;

namespace GGStream.Models
{
    public class Collection
    {
        /**
         * URL path to use when querying for this collection.
         */
        [Required]
        [Key]
        // ReSharper disable once InconsistentNaming
        public string URL { get; set; }

        /**
         * Public name, used in branding.
         */
        [Required]
        public string Name { get; set; }

        /**
         * Icon to display in name.
         */
        public string Icon { get; set; }

        /**
         * Base color to use for UI.
         */
        [DisplayName("Base color (#hex)")]
        public string BaseColor { get; set; }

        [NotMapped]
        public string NearWhiteColor => ChangeColorBrightness((float)0.8);

        [NotMapped]
        public string LightestColor => ChangeColorBrightness((float)0.6);

        [NotMapped]
        public string LighterColor => ChangeColorBrightness((float)0.4);

        [NotMapped]
        public string LightColor => ChangeColorBrightness((float)0.2);

        [NotMapped]
        public string DarkColor => ChangeColorBrightness((float)-0.2);


        [NotMapped]
        public string DarkerColor => ChangeColorBrightness((float)-0.4);

        [NotMapped]
        public string DarkestColor => ChangeColorBrightness((float)-0.6);

        [NotMapped]
        public string NearBlackColor => ChangeColorBrightness((float)-0.8);

        /**
         * Whether this collection is private (need stream key to watch) or public (pull latest stream).
         */
        public bool Private { get; set; }

        /**
         * Which instructions to show. None means no button is shown.
         */
        [DisplayName("Display Instructions?")]
        public InstructionType InstructionType { get; set; }

        /**
         * Teams link to show in UI. If not set, won't show button.
         */
        [DisplayName("Call/Chat Link")]
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

            var red = (float)color.R;
            var green = (float)color.G;
            var blue = (float)color.B;

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
