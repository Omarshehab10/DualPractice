using Common.Types;
using DTO.DTOs.Responses;
using System.Threading.Tasks;

namespace Services
{
    public interface IPdfService
    {
        Task<byte[]> GeneratePdfRazorAsync(string serviceCode, string QrUrl, int status);
        Task<ResultOfAction<GetPdfUrl>> GetDualPracticeReportUrl(string serviceCode);     
    }
}
