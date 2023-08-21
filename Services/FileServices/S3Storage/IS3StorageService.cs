using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;

namespace Services.S3Storage
{
    public interface IS3StorageService
    {
        Task<string> UploadToS3Async(string key, Stream stream);
        Task<Stream> GetObjectAsync(string key);
        Task<string> GetUrlAsync(string key);
        Task<bool> DeleteObjectAsync(string keyn);
    }
}