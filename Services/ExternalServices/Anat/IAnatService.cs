using DTO.DTOs.Parameters;
using DTO.DTOs.Responses;
using System.Threading.Tasks;

namespace Services.ExternalServices.Anat
{
    public interface IAnatService
    {
        Task<NotificationToAnatResponse> SendNotificationToAnat(NotificationToAnatRequest notificationToAnatRequest);
        // DUP-21
        //Task GetAllPractitionerDPrequests();
        // DUP-19
        //Task UpdateRequestStatusByPrac();
    }
}
