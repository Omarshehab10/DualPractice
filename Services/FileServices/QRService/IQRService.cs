using System.Threading.Tasks;

namespace Services
{
    public interface IQRService
    {
        public Task<byte[]> GetQrCode(string text);
    }
}
