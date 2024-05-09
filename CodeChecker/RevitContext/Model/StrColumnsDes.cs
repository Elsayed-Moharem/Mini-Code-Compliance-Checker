using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeChecker.RevitContext.Methods.STR
{
    /// <summary>
    /// Class representing the description of a structural column.
    /// </summary>
    public class StrColumnsDes
    {
        /// <summary>
        /// Gets or sets the ID of the column.
        /// </summary>
        public static string Id { get; set; }

        /// <summary>
        /// Gets or sets the length of the column.
        /// </summary>
        public static double StrColumnsLength { get; set; }

        /// <summary>
        /// Gets or sets the width of the column.
        /// </summary>
        public static double StrColumnsWidth { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="StrColumnsDes"/> class.
        /// </summary>
        /// <param name="id">The ID of the column.</param>
        /// <param name="strColumnsLength">The length of the column.</param>
        /// <param name="strColumnsWidth">The width of the column.</param>
        public StrColumnsDes(string id, double strColumnsLength, double strColumnsWidth)
        {
            Id = id;
            StrColumnsLength = strColumnsLength;
            StrColumnsWidth = strColumnsWidth;
        }
    }
}