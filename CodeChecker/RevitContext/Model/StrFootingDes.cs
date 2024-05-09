using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeChecker.RevitContext.Model
{
    /// <summary>
    /// Class representing the description of a structural footing.
    /// </summary>
    public class StrFootingDes
    {
        /// <summary>
        /// Gets or sets the ID of the footing.
        /// </summary>
        public static string Id { get; set; }

        /// <summary>
        /// Gets or sets the length of the footing.
        /// </summary>
        public static double StrFootingLength { get; set; }

        /// <summary>
        /// Gets or sets the width of the footing.
        /// </summary>
        public static double StrFootingWidth { get; set; }

        /// <summary>
        /// Gets or sets the thickness of the footing.
        /// </summary>
        public static double StrFootingThickness { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="StrFootingDes"/> class.
        /// </summary>
        /// <param name="id">The ID of the footing.</param>
        /// <param name="strFootingLength">The length of the footing.</param>
        /// <param name="strFootingWidth">The width of the footing.</param>
        /// <param name="strFootingThickness">The thickness of the footing.</param>
        public StrFootingDes(string id, double strFootingLength, double strFootingWidth, double strFootingThickness)
        {
            Id = id;
            StrFootingWidth = strFootingWidth;
            StrFootingLength = strFootingLength;
            StrFootingThickness = strFootingThickness;
        }
    }
}