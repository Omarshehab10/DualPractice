using Common.Enums;
using Common.Types;
using DTO.DTOs;
using DTO.DTOs.Parameters;
using DTO.DTOs.Responses;
using DTO.Parameters.PractitionerInfo;
using Lean.Integration.Seha.AspNetCore.Attribute;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.PractitionerService;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DualPractitionerBE.Controllers
{

    /// <summary>
    /// Practitioner controller
    /// </summary>
    [Route("api/practitioner/")]
    [ApiController]
    public class PractitionerController : AuthinticationBaseController<IPractitionerService>
    {
        private readonly IPractitionerService _practitionerService;
        private readonly IPdfService _pdfService;

        /// <summary>
        /// Constructor for Practitioner controller
        /// </summary>
        /// <param name="serviceProvider">Practitioner service</param>
        /// <param name="practitionerService">Practitioner service</param>
        /// <param name="pdfService">Practitioner service</param>
        public PractitionerController(IServiceProvider serviceProvider, IPractitionerService practitionerService, IPdfService pdfService) : base(serviceProvider)
        {
            _practitionerService = practitionerService;
            _pdfService = pdfService;
        }

        /// <summary>
        /// Get Practitioner Info and Check
        /// </summary>
        /// <param name="practitionerInfoRequest"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(ResultOfAction<initialDualPracticeResponse>), 200)]
        [ProducesResponseType(typeof(ResultOfAction<string>), 400)]
        [ProducesResponseType(typeof(ResultOfAction<string>), 404)]
        [ProducesResponseType(typeof(ResultOfAction<string>), 500)]
        [HttpGet("info")]
        [Authorize((int)Permissions.AddDualPracticeRequest)]
        public async Task<IActionResult> GetPractitionerInfoCheck([FromQuery] PractitionerInfoRequest practitionerInfoRequest)
        {
            var PractitionerInfo = await _practitionerService.GetPractitionerInfoCheck(practitionerInfoRequest);
            return Ok(PractitionerInfo);
        }
        /// <summary>
        /// Add practitioner Request
        /// </summary>
        /// <param name="practitionerInfoRequest"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(ResultOfAction<AddNewDualPractitionerResponse>), 200)]
        [ProducesResponseType(typeof(ResultOfAction<string>), 400)]
        [ProducesResponseType(typeof(ResultOfAction<string>), 404)]
        [ProducesResponseType(typeof(ResultOfAction<string>), 500)]
        [HttpPost("add-request")]
        [Authorize((int)Permissions.AddDualPracticeRequest)]
        public async Task<IActionResult> SendInsertRequest([FromBody] AddNewDualPractitionerRequest practitionerInfoRequest)
        {
            var PractitionerInfo = await _practitionerService.AddNewDualPractitionerRequest(practitionerInfoRequest);
            return Ok(PractitionerInfo);
        }

        /// <summary>
        /// Practitioner Review and Accept the request
        /// </summary>
        /// <param name="getRequestInfo"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(ResultOfAction<DefaultModel>), 200)]
        [ProducesResponseType(typeof(ResultOfAction<string>), 400)]
        [ProducesResponseType(typeof(ResultOfAction<string>), 404)]
        [ProducesResponseType(typeof(ResultOfAction<string>), 500)]
        [HttpPut("practitioner-review")]
        public async Task<IActionResult> GetPractitionerReview([FromForm] GetRequestInfo getRequestInfo)
        {
            var PractitionerInfo = await _practitionerService.GetPractitionerReviewFromAnat(getRequestInfo);
            return Ok(PractitionerInfo);
        }
        
        /// <summary>
        /// Practitioner Review and Accept the request
        /// </summary>
        /// <param name="getRequestInfoV2"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(ResultOfAction<DefaultModel>), 200)]
        [ProducesResponseType(typeof(ResultOfAction<string>), 400)]
        [ProducesResponseType(typeof(ResultOfAction<string>), 404)]
        [ProducesResponseType(typeof(ResultOfAction<string>), 500)]
        [HttpPut("practitioner-review-v2")]
        public async Task<IActionResult> GetPractitionerReviewV2([FromForm] GetRequestInfoV2 getRequestInfoV2)
        {
            var PractitionerInfo = await _practitionerService.GetPractitionerReviewFromAnatV2(getRequestInfoV2);
            return Ok(PractitionerInfo);
        }


        /// <summary>
        /// Get Dual Practice Report PDF Url
        /// </summary>
        /// <param name="serviceCode"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(ResultOfAction<GetPdfUrl>), 200)]
        [ProducesResponseType(typeof(ResultOfAction<string>), 400)]
        [ProducesResponseType(typeof(ResultOfAction<string>), 404)]
        [ProducesResponseType(typeof(ResultOfAction<string>), 500)]
        [HttpGet("get-dual-practice-report-url")]
        public async Task<IActionResult> GetDualPracticeReportUrl([FromQuery] RequestServiceCode serviceCode)
        {
            var url = await _pdfService.GetDualPracticeReportUrl(serviceCode.ReqServiceCode);
            return Ok(url);
        }


        /// <summary>
        /// Get Practitioner all requests
        /// </summary>
        /// <param name="practitionerNid"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(ResultOfAction<PractitionerAllRequestsResponse>), 200)]
        [ProducesResponseType(typeof(ResultOfAction<string>), 400)]
        [ProducesResponseType(typeof(ResultOfAction<string>), 404)]
        [ProducesResponseType(typeof(ResultOfAction<string>), 500)]
        [HttpGet("requests-list")]
        public async Task<IActionResult> GetPractitionerAllRequests()
        {
            var PractitionerRequestsList = await _practitionerService.GetPractitionerAllRequests();
            return Ok(PractitionerRequestsList);
        }

        /// <summary>
        /// Get Details of Request
        /// </summary>
        /// <param name="requestServiceCode"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(ResultOfAction<DetailsOfRequestsResponse>), 200)]
        [ProducesResponseType(typeof(ResultOfAction<string>), 400)]
        [ProducesResponseType(typeof(ResultOfAction<string>), 404)]
        [ProducesResponseType(typeof(ResultOfAction<string>), 500)]
        [HttpGet("details-of-request")]
        public  async Task<IActionResult> GetDetailsOfRequest([FromQuery] RequestServiceCode requestServiceCode)
        {
            var detailsOfRequestsResponse = await _practitionerService.GetDetailsOfRequest(requestServiceCode);
            return Ok(detailsOfRequestsResponse);

        }
        /// <summary>
        /// Get Rejection Reasons
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType(typeof(ResultOfAction<List<RejectionReasonsResponse>>), 200)]
        [ProducesResponseType(typeof(ResultOfAction<string>), 400)]
        [ProducesResponseType(typeof(ResultOfAction<string>), 404)]
        [ProducesResponseType(typeof(ResultOfAction<string>), 500)]
        [HttpGet("get-rejection-reasons")]
        public  async Task<IActionResult> GetRejectionReasons()
        {
            var rejectionReasonsList = await _practitionerService.GetRejectionReasonsList();
            return Ok(rejectionReasonsList);

        }

        /// <summary>
        /// Send Anat SMS And Notification After Five Days
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType(typeof(ResultOfAction<DefaultModel>), 200)]
        [ProducesResponseType(typeof(ResultOfAction<string>), 400)]
        [ProducesResponseType(typeof(ResultOfAction<string>), 404)]
        [ProducesResponseType(typeof(ResultOfAction<string>), 500)]
        [HttpGet("send-anat-notification")]
        public async Task<IActionResult> SendAnatSmsAndNotification([FromForm] SendAnatNotificationParameters sendAnatNotificationParameters)
        {
            var PractitionerInfo = await _practitionerService.SendAnatSmsAndNotificationAfterFiveDays(sendAnatNotificationParameters);
            return Ok(PractitionerInfo);

        }
    }
}
