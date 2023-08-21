using DTO.DTOs.Parameters;
using DTO.DTOs.Responses;
using DTO.DTOs.SehaIntegration;
using System.Threading.Tasks;

namespace Services.ExternalServices.SehaEndPoint
{
    public interface  ISehaService
    {
        Task<OrganizationDataResponse> GetOrganizationData(string accessToken);
        Task<MedicalOrgSubCategoryResponse> GetMedicalOrganizationSubCategory(string accessToken);
        Task<ServicePointsResponse> PaymentGetServicePoint(string ServiceProcessId, string accessToken);
        Task<InquireBalanceResponse> InquireBalance(InquireBalanceRequest request);
        Task<UsersInfoResponse> GetUsersInfo(UsersInfoRequest usersInfoRequest);

    }
}
