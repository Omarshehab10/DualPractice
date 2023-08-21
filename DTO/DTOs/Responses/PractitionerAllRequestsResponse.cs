using System;
using System.Collections.Generic;
using System.Text;

namespace DTO.DTOs.Responses
{
    /// <summary>
    ///  Practitioner All Requests Response
    /// </summary>
    public class PractitionerAllRequestsResponse
    {
        /// <summary>
        ///  Practitioner All Requests Response
        /// </summary>
        public PractitionerAllRequestsResponse()
        {
            request = new List<DoctorRequestsDto>();
        }
        /// <summary>
        ///  request
        /// </summary>
        public List<DoctorRequestsDto> request { get; set; }
    }
    /// <summary>
    /// Doctor Requests Dto
    /// </summary>
    public class DoctorRequestsDto
    {
        /// <summary>
        ///  Doctor Requests Dto
        /// </summary>
        public DoctorRequestsDto()
        {
            DoctorDaySchedule = new List<DoctorDailySchedule>();
        }
        /// <summary>
        ///  Request id
        /// </summary>
        public string Requestid { get; set; }
        /// <summary>
        ///  Gov Org Name
        /// </summary>
        public string GovOrgName { get; set; }
        /// <summary>
        /// Gov Establishment City Name
        /// </summary>
        public string GovCityName { get; set; }
        /// <summary>
        /// Private Establishment City
        /// </summary>
        public string PrivateCityName { get; set; }
        /// <summary>
        ///  Prv Org Name
        /// </summary>
        public string PrvOrgName { get; set; }
        /// <summary>
        ///  Prv Org License Number
        /// </summary>
        public string PrvOrgLicenseNumber { get; set; }
        /// <summary>
        ///  Gov Org Category Name 
        /// </summary>
        public string GovOrgCategoryName { get; set; }
        /// <summary>
        ///  Prv Org Category Name 
        /// </summary>
        public string PrvOrgCategoryName { get; set; }

        /// <summary>
        ///  Specialty Name
        /// </summary>
        public string SpecialtyName { get; set; }

        /// <summary>
        ///  Approved Report Url
        /// </summary>
        public string ApprovedReportUrl { get; set; }
        /// <summary>
        ///  Doctor Day Schedule
        /// </summary>
        public List<DoctorDailySchedule> DoctorDaySchedule { get; set; }
        /// <summary>
        ///  Total Weekly Hours
        /// </summary>
        public decimal? TotalWeeklyHours { get; set; }
        /// <summary>
        ///  Approval Duration
        /// </summary>
        public int ApprovalDuration { get; set; }
        /// <summary>
        ///  Req Status
        /// </summary>
        public int ReqStatus { get; set; }
    }

    /// <summary>
    ///  Doctor Daily Schedule
    /// </summary>
    public class DoctorDailySchedule
    {
        /// <summary>
        ///  From
        /// </summary>
        public DateTime? From { get; set; }
        /// <summary>
        /// To
        /// </summary>
        public DateTime? To { get; set; }
        /// <summary>
        ///  TotalHours
        /// </summary>
        public decimal? TotalHours { get; set; }
        /// <summary>
        ///  Day
        /// </summary>
        public int Day { get; set; }

    }
}
