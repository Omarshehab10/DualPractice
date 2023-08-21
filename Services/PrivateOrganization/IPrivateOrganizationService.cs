using Common.Types;
using DTO.DTOs;
using DTO.DTOs.Parameters;
using DTO.DTOs.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.PrivateOrganization
{
    public interface  IPrivateOrganizationService
    {
        Task<ResultOfAction<DefaultModel>> AcceptOrCancelRequest(GetRequestInfo getRequestInfo);
        Task<ResultOfAction<List<RequestsOfRequestingOrgResponse>>> GetListRequestsOfRequestingOrg();
        Task<ResultOfAction<GetLicenseToHLSResponse>> GetDPlicensetoHLS(ParctitionerNId parctitionerNId);
        Task<ResultOfAction<DefaultModel>> CancelRequestByPrivateOrg(RequestServiceCode requestServiceCode);
    }
}
