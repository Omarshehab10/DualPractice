using Common.Enums;
using Common.Types;
using DTO.DTOs;
using DTO.DTOs.Parameters;
using DTO.DTOs.Responses;
using Microsoft.AspNetCore.Mvc;
using Services.PrivateOrganization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AuthorizeAttribute = Lean.Integration.Seha.AspNetCore.Attribute.AuthorizeAttribute;

namespace DualPractitionerBE.Controllers
{

    /// <summary>
    /// Private Organization Controller
    /// </summary>
    [Route("api/privateorganization/")]
    [ApiController]
    public class PrivateOrganizationController : AuthinticationBaseController<IPrivateOrganizationService>
    {
        private readonly IPrivateOrganizationService _priOrganizationServiceService;
        /// <summary>
        /// Constructor for Private Organization Controller 
        /// </summary>
        /// <param name="serviceProvider"></param>
        public PrivateOrganizationController(IServiceProvider serviceProvider, IPrivateOrganizationService priOrganizationServiceService) : base(serviceProvider)
        {
            _priOrganizationServiceService = priOrganizationServiceService;
        }

        /// <summary>
        /// Private Organization Accept the request and deduct Points
        /// </summary>
        /// <param name="getRequestInfo"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(ResultOfAction<DefaultModel>), 200)]
        [ProducesResponseType(typeof(ResultOfAction<string>), 400)]
        [ProducesResponseType(typeof(ResultOfAction<string>), 404)]
        [ProducesResponseType(typeof(ResultOfAction<string>), 500)]
        [HttpPost("review-request-and-deduct")]
        [Authorize((int)Permissions.AddDualPracticeRequest)]
        public async Task<IActionResult> GetGovOrganisationReview ([FromForm] GetRequestInfo getRequestInfo)
        {
            var result = await _priOrganizationServiceService.AcceptOrCancelRequest(getRequestInfo);
            return Ok(result);
        }

        /// <summary>
        ///   Get List Of Requests For Private Organization
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType(typeof(ResultOfAction<List<RequestsOfRequestingOrgResponse>>), 200)]
        [ProducesResponseType(typeof(ResultOfAction<string>), 400)]
        [ProducesResponseType(typeof(ResultOfAction<string>), 404)]
        [ProducesResponseType(typeof(ResultOfAction<string>), 500)]
        [HttpGet("get-list-requests-private-organization")]
        [Authorize((int)Permissions.AddDualPracticeRequest)]
        public async Task<IActionResult> GetListRequestsPrivateOrganization()
        {
            var List = await _priOrganizationServiceService.GetListRequestsOfRequestingOrg();
            return Ok(List);
        }

        /// <summary>
        ///   Get DP license to HLS API
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType(typeof(ResultOfAction<GetLicenseToHLSResponse>), 200)]
        [ProducesResponseType(typeof(ResultOfAction<string>), 400)]
        [ProducesResponseType(typeof(ResultOfAction<string>), 404)]
        [ProducesResponseType(typeof(ResultOfAction<string>), 500)]
        [HttpGet("get-license-to-hls")]
        public async Task<IActionResult> GetDPlicensetoHLSAPI([FromQuery] ParctitionerNId parctitionerNId)
        {
            var result = await _priOrganizationServiceService.GetDPlicensetoHLS(parctitionerNId);
            return Ok(result);
        }


        /// <summary>
        /// Private Organization Cancel Request
        /// </summary>
        /// <param name="requestServiceCode"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(ResultOfAction<DefaultModel>), 200)]
        [ProducesResponseType(typeof(ResultOfAction<string>), 400)]
        [ProducesResponseType(typeof(ResultOfAction<string>), 404)]
        [ProducesResponseType(typeof(ResultOfAction<string>), 500)]
        [HttpPost("cancel-request-by-private-org")]
        [Authorize((int)Permissions.AddDualPracticeRequest)]
        public async Task<IActionResult> CancelRequestPrivateOrg([FromForm] RequestServiceCode requestServiceCode)
        {
            var result = await _priOrganizationServiceService.CancelRequestByPrivateOrg(requestServiceCode);
            return Ok(result);
        }
    }
}
