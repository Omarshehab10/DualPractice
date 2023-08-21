using Common.Enums;
using Common.Types;
using DTO.DTOs.Parameters;
using DTO.DTOs.Responses;
using Microsoft.AspNetCore.Mvc;
using Services.PaymentService;
using System;
using System.Threading.Tasks;
using AuthorizeAttribute = Lean.Integration.Seha.AspNetCore.Attribute.AuthorizeAttribute;

namespace DualPractitionerBE.Controllers
{
    /// <summary>
    /// PaymentService controller
    /// </summary>
    [Route("api/payment/")]
    [ApiController]
    public class PaymentController : AuthinticationBaseController<IPaymentService>
    {
        private readonly IPaymentService _paymentService;

        /// <summary>
        /// Constructor for PaymentService controller
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="paymentService"></param>
        public PaymentController(IServiceProvider serviceProvider, IPaymentService paymentService) : base(serviceProvider)
        {
            _paymentService = paymentService;
        }

        /// <summary>
        /// Get Service Points
        /// </summary>
        /// <param name="requestServiceCode"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(ResultOfAction<ServicePointsResponse>), 200)]
        [ProducesResponseType(typeof(ResultOfAction<string>), 400)]
        [ProducesResponseType(typeof(ResultOfAction<string>), 404)]
        [ProducesResponseType(typeof(ResultOfAction<string>), 500)]
        [HttpGet("get-service-points")]
        [Authorize((int)Permissions.AddDualPracticeRequest)]
        public async Task<IActionResult> GetServicePoints([FromQuery] RequestServiceCode requestServiceCode)
        {
            var result = await _paymentService.GetServicePoints(requestServiceCode.ReqServiceCode);
            return Ok(result);
        }
    }
}
