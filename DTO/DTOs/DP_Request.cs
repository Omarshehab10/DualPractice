using System;
using System.Collections.Generic;

namespace DTO.DTOs
{
    /// <summary>
    /// DP Request
    /// </summary>
    public class DP_Request
    {
        /// <summary>
        ///  full Name Ar
        /// </summary>
        public string fullNameAr { get; set; }
        /// <summary>
        ///  full Name En
        /// </summary>
        public string fullNameEn { get; set; }
        /// <summary>
        ///  service Code
        /// </summary>
        public string serviceCode { get; set; }
        /// <summary>
        ///  speciality Ar
        /// </summary>
        public string specialityAr { get; set; }
        /// <summary>
        ///  speciality En
        /// </summary>
        public string specialityEn { get; set; }
        /// <summary>
        ///  scfhs Category Ar
        /// </summary>
        public string scfhsCategoryAr { get; set; }
        /// <summary>
        ///  scfhs Category En
        /// </summary>
        public string scfhsCategoryEn { get; set; }
        /// <summary>
        ///  total Weekly Hours
        /// </summary>
        public decimal totalWeeklyHours { get; set; }
        /// <summary>
        ///  create Date
        /// </summary>
        public DateTime createDate { get; set; }
        /// <summary>
        ///  duration Months
        /// </summary>
        public int durationMonths { get; set; }
        /// <summary>
        ///  from
        /// </summary>
        public DateTime? from { get; set; }
        /// <summary>
        ///  to
        /// </summary>
        public DateTime? to { get; set; }
        /// <summary>
        ///  gov Org Name Ar
        /// </summary>
        public string govOrgNameAr { get; set; }
        /// <summary>
        ///  gov Org Name En
        /// </summary>
        public string govOrgNameEn { get; set; }
        /// <summary>
        ///  gov Org Type Name Ar
        /// </summary>
        public string govOrgTypeNameAr { get; set; }
        /// <summary>
        ///  gov Org Type Name En
        /// </summary>
        public string govOrgTypeNameEn { get; set; }
        /// <summary>
        ///  private Org Name Ar
        /// </summary>
        public string priOrgNameAr { get; set; }
        /// <summary>
        ///  private Org Name En
        /// </summary>
        public string priOrgNameEn { get; set; }
        /// <summary>
        ///  private Org Type Name Ar
        /// </summary>
        public string priOrgTypeNameAr { get; set; }
        /// <summary>
        ///  private Org Type Name En
        /// </summary>
        public string priOrgTypeNameEn { get; set; }
        /// <summary>
        ///  license Number
        /// </summary>
        public string licenseNumber { get; set; }
        /// <summary>
        ///  work Days
        /// </summary>
        public List<WorkDay> workDays { get; set; }
        /// <summary>
        ///  reportUrl
        /// </summary>
        public string reportUrl { get; set; }
        /// <summary>
        ///  QrCode
        /// </summary>
        public string QrCode { get; set; }
        /// <summary>
        ///  status
        /// </summary>
        public int status { get; set; }
    }
    /// <summary>
    ///  Work Day
    /// </summary>
    public class WorkDay
    {
        /// <summary>
        ///  From
        /// </summary>
        public DateTime From { get; set; }
        /// <summary>
        ///  To
        /// </summary>
        public DateTime To { get; set; }
        /// <summary>
        ///  Day
        /// </summary>
        public byte Day { get; set; }
        /// <summary>
        ///  From 2
        /// </summary>
        public DateTime? From2 { get; set; }
        /// <summary>
        ///  To 2
        /// </summary>
        public DateTime? To2 { get; set; }
    }
}
