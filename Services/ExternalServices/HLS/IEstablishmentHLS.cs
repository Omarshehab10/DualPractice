using DTO.DTOs.EstablishmentHLS;
using System.Threading.Tasks;

namespace Services.ExternalServices.HLS
{
    public interface IEstablishmentHLS
    {
        Task<HlsResponse> GetEstablishmentData(string licenseNumber);
    }
}
