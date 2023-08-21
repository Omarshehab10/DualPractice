using Common.Enums;
using Common.Types;
using DTO.DTOs;
using DTO.DTOs.OrganizationRegistry;
using DTO.DTOs.Parameters;
using DTO.DTOs.Responses;
using Microsoft.AspNetCore.Mvc;
using Services.ExternalServices.OrganizationRegistry;
using Services.GovOrganization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AuthorizeAttribute = Lean.Integration.Seha.AspNetCore.Attribute.AuthorizeAttribute;

namespace DualPractitionerBE.Controllers
{
    /// <summary>
    /// Gov Organization Controller
    /// </summary>
    [Route("api/govorganization/")]
    [ApiController]
    public class GovOrganizationController : AuthinticationBaseController<IGovOrganizationService>
    {
        private readonly IGovOrganizationService _govOrganizationServiceService;
        private readonly IOrganizationRegistryService _organizationRegistryService;
        /// <summary>
        /// Constructor for Practitioner controller
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="govOrganizationService"></param>
        /// <param name="organizationRegistryService"></param>
        public GovOrganizationController(IServiceProvider serviceProvider, IGovOrganizationService govOrganizationService, IOrganizationRegistryService organizationRegistryService) : base(serviceProvider)
        {
            _govOrganizationServiceService = govOrganizationService;
            _organizationRegistryService = organizationRegistryService;
        }

        /// <summary>
        /// Gov Organisation Review and Accept the request
        /// </summary>
        /// <param name="getRequestInfo"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(ResultOfAction<DefaultModel>), 200)]
        [ProducesResponseType(typeof(ResultOfAction<string>), 400)]
        [ProducesResponseType(typeof(ResultOfAction<string>), 404)]
        [ProducesResponseType(typeof(ResultOfAction<string>), 500)]
        [HttpPut("gov-review")]
        [Authorize((int)Permissions.ApproveOrRejectDPRequest)]
        public async Task<IActionResult> GetGovOrganisationReview ([FromForm] GetRequestInfo getRequestInfo)
        {
            var PractitionerInfo = await _govOrganizationServiceService.GovOrganisationReview(getRequestInfo);
            return Ok(PractitionerInfo);
        }

        /// <summary>
        /// Gov Level 2 Organisation Review and Accept the request
        /// </summary>
        /// <param name="getRequestInfo"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(ResultOfAction<DefaultModel>), 200)]
        [ProducesResponseType(typeof(ResultOfAction<string>), 400)]
        [ProducesResponseType(typeof(ResultOfAction<string>), 404)]
        [ProducesResponseType(typeof(ResultOfAction<string>), 500)]
        [HttpPut("gov-review-lvltwo")]
        [Authorize((int)Permissions.FinalApproveOrRejectDPRequest)]
        public async Task<IActionResult> GovOrganisationReviewLevelTwo([FromForm] GetRequestInfo getRequestInfo)
        {
            var PractitionerInfo = await _govOrganizationServiceService.GovOrganisationReviewLevelTwo(getRequestInfo);
            return Ok(PractitionerInfo);
        }




        /// <summary>
        ///   Get List Of Requests For Government Organization Level One
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType(typeof(ResultOfAction<List<RequestsOfPractionerMainOrgResponse>>), 200)]
        [ProducesResponseType(typeof(ResultOfAction<string>), 400)]
        [ProducesResponseType(typeof(ResultOfAction<string>), 404)]
        [ProducesResponseType(typeof(ResultOfAction<string>), 500)]
        [HttpGet("get-list-requests-government-organization-level-one")]
        [Authorize((int)Permissions.ApproveOrRejectDPRequest)]
        public async Task<IActionResult> GetListRequestsGovOrganization()
        {
            var List = await _govOrganizationServiceService.GetListRequestsOfPractionerLevelOneOrg();
            return Ok(List);
        }

        /// <summary>
        ///   Get List Of Requests For Government Organization Level Two
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType(typeof(ResultOfAction<List<RequestsOfPractionerMainOrgResponse>>), 200)]
        [ProducesResponseType(typeof(ResultOfAction<string>), 400)]
        [ProducesResponseType(typeof(ResultOfAction<string>), 404)]
        [ProducesResponseType(typeof(ResultOfAction<string>), 500)]
        [HttpGet("get-list-requests-government-organization-level-two")]
        [Authorize((int)Permissions.FinalApproveOrRejectDPRequest)]
        public async Task<IActionResult> GetListRequestsGovOrganizationTow()
        {
            var List = await _govOrganizationServiceService.GetListRequestsOfPractionerLevelTwoOrg();
            return Ok(List);
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
        [HttpPost("cancel-request-by-government-org")]
        [Authorize((int)Permissions.ApproveOrRejectDPRequest)]
        public async Task<IActionResult> CancelRequestGovlvl1Org([FromForm] RequestServiceCode requestServiceCode)
        {
            var result = await _govOrganizationServiceService.CancelRequestByGovOrg(requestServiceCode);
            return Ok(result);
        }
    }
}
