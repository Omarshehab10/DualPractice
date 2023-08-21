using DTO.DTOs.PractitionersRegistry;
using System.Threading.Tasks;

namespace Services.ExternalServices.PractitionersRegistry
{
    public interface IPractitionersRegistryService
    {
        Task<PractitionerDataV3> GetPractitionerData(string NationalId);
    }
}
