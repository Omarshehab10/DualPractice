using System;
using System.Collections.Generic;

namespace DAL.Models
{
    public partial class NonMohOrganizationLookup
    {
        public int Id { get; set; }
        public int Orgid { get; set; }
        public string Name { get; set; }
        public string Region { get; set; }
    }
}
