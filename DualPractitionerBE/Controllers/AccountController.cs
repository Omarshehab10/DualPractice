using Common.Types;
using DTO.DTOs.Responses;
using DTO.DTOs.SehaIntegration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Services.IdentityServices;
using System;
using System.Net;
using System.Threading.Tasks;

namespace DualPractitionerBE.Controllers
{
    /// <summary>
    /// AccountController
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AccountController : AuthinticationBaseController<IUserService>
    {
        /// <summary>
        /// AccountController
        /// </summary>
        public AccountController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }


        // Only if diff domain
        #region Public :: Login API
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ApiResponse<UserDto>> Login([Bind(nameof(AuthenticationDto.Code))] AuthenticationDto model)
        {
            try
            {
                var userdate = await _sehaIntegrationService.Login(model.Code);

                _logger.LogDebug($"----------------------------------------userdate----------------------------------------\n{userdate}");
                if (userdate != null)
                {
                    var userModel = new UserDto
                    {
                        Id = userdate.UserInfoModel.Id,
                        Role = userdate.Permissions,
                        Ticket = userdate.SehaToken,
                        UserId = userdate.UserInfoModel.ExternalUserId
                    };
                    return await Task.FromResult(new ApiResponse<UserDto>(userModel));
                }


                _logger.LogError($"Complete Login From Seha - UserNotExist");

                throw new CustomHttpException(HttpStatusCode.BadRequest, "UserNotExist");
            }
            catch (Exception ex)
            {

                _logger.LogError($"Complete Login From Seha - = (" + ex.Message + ")",ex.Message);

                throw new CustomHttpException(HttpStatusCode.BadRequest, ex.Message);
            }
        }
        #endregion
    }
}
