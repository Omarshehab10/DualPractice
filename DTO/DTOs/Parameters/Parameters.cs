using System;
using System.Collections.Generic;
using System.Text;

namespace DTO.Parameters
{
    /// <summary>
    /// Base Filter Parameters
    /// </summary>
    public class BaseFilterParameters
    {
        /// <summary>
        /// Page Number 
        /// </summary>
        public int PageNumber { get; set; } = 1;
        /// <summary>
        /// Page Size 
        /// </summary>
        public int PageSize { get; set; } = 50;
        /// <summary>
        /// Keyword
        /// </summary>
        public string Keyword { get; set; }
    }

    /// <summary>
    /// Ordering Parameters
    /// </summary>
    public class OrderingParameters
    {
        /// <summary>
        /// Order Field 
        /// </summary>
        public string OrderField { get; set; }
        /// <summary>
        /// Is Desc
        /// </summary>
        public bool IsDesc { get; set; }
    }

}
