using Common.Types;
using DTO.DTOs;
using DTO.DTOs.Parameters;
using DTO.DTOs.Responses;
using DTO.Parameters.PractitionerInfo;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.PractitionerService
{
    public interface IPractitionerService
    {
        Task<ResultOfAction<initialDualPracticeResponse>> GetPractitionerInfoCheck(PractitionerInfoRequest practitionerInfoRequest);
        Task<ResultOfAction<AddNewDualPractitionerResponse>> AddNewDualPractitionerRequest(AddNewDualPractitionerRequest addNewDualPractitionerRequest);
        Task<ResultOfAction<DefaultModel>> GetPractitionerReviewFromAnat(GetRequestInfo getRequestInfo);
        Task<ResultOfAction<DefaultModel>> GetPractitionerReviewFromAnatV2(GetRequestInfoV2 getRequestInfo);
        Task<DP_Request> GetDualPracticeReport(string serviceCode);
        Task<string> SaveDualPracticeUrl(string serviceCode, string QrUrl, int reportStatus);
        Task<ResultOfAction<PractitionerAllRequestsResponse>> GetPractitionerAllRequests();
        Task<ResultOfAction<DetailsOfRequestsResponse>> GetDetailsOfRequest(RequestServiceCode requestServiceCode);
        Task<ResultOfAction<List<RejectionReasonsResponse>>> GetRejectionReasonsList();
        Task<ResultOfAction<DefaultModel>> SendAnatSmsAndNotificationAfterFiveDays(SendAnatNotificationParameters sendAnatNotificationParameters);


    }
}