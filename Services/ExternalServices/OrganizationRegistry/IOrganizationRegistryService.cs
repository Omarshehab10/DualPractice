using DTO.DTOs.OrganizationRegistry;
using System.Threading.Tasks;

namespace Services.ExternalServices.OrganizationRegistry
{
    public interface IOrganizationRegistryService
    {
        Task<OrganizationData> GetOrganizationData(string parameter); // parameter : can be Health ID or License number or Seha ID
        Task<OrganizationDataV2> GetOrganizationDataV2(string parameter); 
    }
}
