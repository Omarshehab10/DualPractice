using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DTO.DTOs
{
    /// <summary>
    ///  Service Code
    /// </summary>
    public class ServiceCode
    {
        /// <summary>
        ///  Prefix
        /// </summary>
        public string Prefix { set; get; }
        /// <summary>
        ///  Date
        /// </summary>
        public DateTime? Date { set; get; }
        /// <summary>
        ///  Day
        /// </summary>
        public int Code { set; get; }
    }
}
