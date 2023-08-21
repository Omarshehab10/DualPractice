using Common.Enums;
using Lean.Framework.Entities.Integration;
using Lean.Integration.Seha.AspNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace DualPractitionerBE.Controllers
{
    /// <summary>
    /// Base Controller
    /// </summary>
    public class BaseController<T> : Controller
    {
        protected IServiceProvider _serviceProvider { get; }
        protected T _currentService { get; }
        protected ILogger _logger { get; }
        protected ISehaIntegrationWebService _sehaIntegrationService { get; }
        protected readonly IHttpContextAccessor _httpContextAccessor;
        /// <summary>
        /// Constructor for Base controller
        /// </summary>
        public BaseController(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _currentService = serviceProvider.GetRequiredService<T>();
            _logger = serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger(GetType());
            _sehaIntegrationService = serviceProvider.GetRequiredService<ISehaIntegrationWebService>();
            _httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
        }
    }

    /// <summary>
    /// Authintication Base Controller
    /// </summary>
    /// <typeparam name="T">IPractitionerService</typeparam>
    public class AuthinticationBaseController<T> : BaseController<T>
    {
        public readonly IServiceProvider serviceProvider;
        /// <summary>
        /// Constructor for Authintication Base Controller
        /// </summary>
        public AuthinticationBaseController(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            this.serviceProvider = serviceProvider;

        }

        /// <summary>
        /// Get Identity User
        /// </summary>
        protected IndentityUser GetIdentityUser()
        {
            try
            {
                var userData = _sehaIntegrationService.GetUserData();

                if (userData != null)
                {
                    var model = userData;
                    var data = new IndentityUser
                    {
                        Id = model.UserId,
                        ExternalUserId = model.ExternalUserId,
                        Role = model.PermissionsList.Cast<Permissions>().ToArray()
                    };
                    return data;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"an error occurring while Get Identity User : { ex.Message }");
            }

        }
        /// <summary>
        /// Get Identity User Role
        /// </summary>
        protected Permissions GetIdentityUserRole()
        {
            if (GetIdentityUser().Role != null && GetIdentityUser().Role.Length > 1)
            {
                var userRole = GetUserRoleSelected();
                _logger.LogInformation($"User is {userRole.ToString()}");
                if (GetIdentityUser().Role.Contains(userRole))
                {
                    return userRole;
                }
            }
            else
            {
                return GetIdentityUser().Role.FirstOrDefault();
            }
            return Permissions.Unknown;
        }
        /// <summary>
        /// Get All User Role
        /// </summary>
        protected Permissions[] GetAllUserRole()
        {
            var userRole = GetIdentityUser();
            if (userRole != null && userRole.Role != null && userRole.Role.Length > 1)
            {
                return GetIdentityUser().Role;
            }

            return null;
        }
        /// <summary>
        /// Get User Role Selected
        /// </summary>
        private Permissions GetUserRoleSelected()
        {
            var userRole = GetCookie("UserRoleSelected");
            if (!String.IsNullOrWhiteSpace(userRole))
            {
                return (Permissions)Convert.ToInt32(userRole);
            }
            else
            {
                return Permissions.Unknown;
            }
        }

        /// <summary>
        ///   //read cookie from IHttpContextAccessor  
        /// </summary>
        /// <param name="key"></param>
        private string GetCookie(string key)
        {

            return _httpContextAccessor.HttpContext.Request.Cookies[key];
        }
        /// <summary>
        ///   Remove User Role Cookie
        /// </summary>
        protected void RemoveUserRoleCookie()
        {
            var key = "UserRoleSelected";
            if (GetCookie(key) != null)
                Remove(key);
        }

        /// <summary>  
        /// Delete the key  
        /// </summary>  
        /// <param name="key">Key</param>  
        private void Remove(string key)
        {
            _httpContextAccessor.HttpContext.Response.Cookies.Delete(key);
        }

        //protected async Task<Organization> GetOrgtInfoForCurrentUser()
        //{
        //    var userData = _sehaIntegrationService.GetUserData();

        //    if (userData != null)
        //    {
        //        var data = await _unitOfWork.Repository<Organization>().Get(a => a.Id == userData.RequestInfoId).FirstOrDefaultAsync();

        //        return data;
        //    }
        //    return null;
        //}

        /// <summary>
        /// Get Request Info Id For CurrentUser
        /// </summary>
        protected int GetRequestInfoIdForCurrentUser()
        {
            var userData = (UserTokenData)_httpContextAccessor.HttpContext.Items["UserData"];
            return userData.RequestInfoId;
        }
        /// <summary>
        /// Is Private Organization
        /// </summary>
        protected bool IsPrivateOrganization()
        {
            var userData = (UserTokenData)_httpContextAccessor.HttpContext.Items["UserData"];
            return userData.CategoryId == 2;
        }
    }

}
/// <summary>
/// Indentity User
/// </summary>
public class IndentityUser
{
    /// <summary>
    /// Id
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// External User Id
    /// </summary>
    public int ExternalUserId { get; set; }
    /// <summary>
    /// Role
    /// </summary>
    public Permissions[] Role { get; set; }
}

