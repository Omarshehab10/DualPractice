using Common.Localization;
using Common.Types;
using DAL.Models;
using DAL.Repository;
using DTO.DTOs;
using DTO.DTOs.EmailSetting;
using DTO.DTOs.Parameters;
using DTO.DTOs.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Services.ExternalServices.SehaEndPoint;
using Services.ExternalServices.SendEmailService;
using Services.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Services.Common
{
    public class CommonService : ICommonService
    {

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAppResourceService _resourceService;
        private readonly ILogger<CommonService> _logger;
        private readonly IRepositoryBase repository;
        public CommonService(IRepositoryBase repository, IHttpContextAccessor httpContextAccessor, 
            IConfiguration configuration, IUnitOfWork unitOfWork, IAppResourceService resourceService,
            ILogger<CommonService> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _unitOfWork = unitOfWork;
            _resourceService = resourceService;
            _logger = logger;
            this.repository = repository;
        }
        public int GetNextCode()
        {
            var _params = new List<SqlParameter> {
             new SqlParameter("@nextServiceCode",SqlDbType.VarChar,20,ParameterDirection.Output,true,0,0,"",DataRowVersion.Default,"")
            };

            var result = repository.ExecuteStoredProcedureOutPut("[dbo].[CalculateNextCode]", _params.ToArray());

            var res = result["@nextServiceCode"].ToString();
            return
                int.Parse(res) > 0 ? int.Parse(res) : -1;
        }
        public bool IsArabicLangHeader()
        {
            string langHeader = _configuration["Headers:Lang"];

            _logger.LogInformation($"IsArabicLangHeader - langheader is {langHeader}");

            _httpContextAccessor.HttpContext.Request.Headers.TryGetValue(langHeader, out var lang);

            if (string.IsNullOrEmpty(lang))
                return false;

            if (lang.ToString().ToLower().Contains("ar"))
                return true;
            else
                return false;
        }

        public string GetNIdHeader(bool withException)
        {
            _logger.LogInformation($"***** GetNIdHeader - starting getting the practitioner id from header, with exception is {withException}");

            string nIdHeader = _configuration["Headers:NId"];

            _logger.LogInformation($"***** GetNIdHeader - nIdHeader value is {nIdHeader}");

            _httpContextAccessor.HttpContext.Request.Headers.TryGetValue(nIdHeader, out var nId);

            if (withException)
            {
                if (string.IsNullOrEmpty(nId))
                {
                    _logger.LogError($"***** GetNIdHeader - practitioner id from header can't be null");

                    throw new CustomHttpException(HttpStatusCode.BadRequest, _resourceService.GetResource("UserNotFound"));
                }
            }

            _logger.LogInformation($"***** GetNIdHeader - nid value is {nId}");

            return nId;
        }
        public DateTime GetCurrentDateTime()
        {
            return DateTime.Now;
        }
        public string MapPath(string path)
        {
            return Path.Combine(
            (string)AppDomain.CurrentDomain.GetData("ContentRootPath"),
            path);
        }

        public async Task AddActionRequestLog(AddLogRequest addLogRequest)
        {
            try
            {
                Logs logs = new Logs
                {
                    ActionType = (int)addLogRequest.ActionType,
                    Date = GetCurrentDateTime(),
                    UserId = addLogRequest.UserId,
                    NormalizedServiceCode = addLogRequest.NormalizedServiceCode
                };
                await _unitOfWork.Repository<Logs>().InsertAsync(logs);
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.LogError($"AddActionRequestLog - A problem occurred while Insirting New Record To Logs Table -  {e.Message}");
            }
        }

    }
}
