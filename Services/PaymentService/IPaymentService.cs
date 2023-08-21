using Common.Types;
using DAL.Models;
using DTO.DTOs.Responses;
using System.Threading.Tasks;

namespace Services.PaymentService
{
    public interface IPaymentService
    {
        Task<ResultOfAction<ServicePointsResponse>> GetServicePoints(string serviceCode);
        Task<ResultOfAction<PaymentTransactionDetail>> DeductPoints(DpRequest dpRequest);
    }
}
