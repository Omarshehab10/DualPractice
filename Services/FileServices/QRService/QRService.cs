using Common.Localization;
using Common.Types;
using Microsoft.Extensions.Logging;
using SkiaSharp;
using SkiaSharp.QrCode;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;


namespace Services
{
    public class QRService : IQRService
    {
        private readonly ILogger<QRService> _logger;
        private readonly IAppResourceService _resourceService;
        public QRService( ILogger<QRService> logger, IAppResourceService resourceService)
        {
            _logger = logger;
            _resourceService = resourceService;
        }

        public Task<byte[]> GetQrCode(string text)
        {
            byte[] result = null;

            try
            {
                var content = text;
                var generator = new QRCodeGenerator();

                var level = ECCLevel.H;
                var qr = generator.CreateQrCode(content, level);

                var info = new SKImageInfo(512, 512);
                var surface = SKSurface.Create(info);

                var canvas = surface.Canvas;
                canvas.Render(qr, 512, 512);

                var image = surface.Snapshot();
                var data = image.Encode(SKEncodedImageFormat.Png, 100);

                using (var memoryStream = new MemoryStream())
                {
                    data.SaveTo(memoryStream);
                    result = memoryStream.ToArray();
                }
            }
            catch (Exception ex)
            {

                _logger.LogError($"QRCodeGenerator Error,  Error Message: {ex.Message}. \n error details: {ex.InnerException}");
                throw new CustomHttpException(HttpStatusCode.BadRequest, _resourceService.GetResource("Error_Generate_Pdf"));
            }


            return Task.Run(() => result);
        }
    }
}
