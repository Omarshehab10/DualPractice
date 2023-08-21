using Common.Types;
using DTO.DTOs;
using DTO.DTOs.Parameters;
using DTO.DTOs.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.GovOrganization
{
    public interface IGovOrganizationService
    {
        Task<ResultOfAction<DefaultModel>> GovOrganisationReview(GetRequestInfo getRequestInfo);
        Task<ResultOfAction<DefaultModel>> GovOrganisationReviewLevelTwo(GetRequestInfo getRequestInfo);
        Task<ResultOfAction<List<RequestsOfPractionerMainOrgResponse>>> GetListRequestsOfPractionerLevelOneOrg();
        Task<ResultOfAction<List<RequestsOfPractionerMainOrgResponse>>> GetListRequestsOfPractionerLevelTwoOrg();
        Task<ResultOfAction<DefaultModel>> CancelRequestByGovOrg(RequestServiceCode requestServiceCode);

    }
}
