using DTO.DTOs.Parameters;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Services.Common
{
    public interface ICommonService
    {
        DateTime GetCurrentDateTime();
        bool IsArabicLangHeader();
        string GetNIdHeader(bool withException);
        int GetNextCode();
        string MapPath(string path);
        Task AddActionRequestLog(AddLogRequest addLogRequest);

    }
}
