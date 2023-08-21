using DinkToPdf;
using DinkToPdf.Contracts;
using System.Threading.Tasks;
using Services.PractitionerService;
using System;
using Services.S3Storage;
using System.IO;
using Common.Types;
using DTO.DTOs.Responses;
using System.Net;
using Microsoft.Extensions.Logging;
using Common.Localization;
using System.Linq;
using Common.Enums;

namespace Services
{
    public class PdfService : IPdfService
    {
        // Pdf library logic
        private readonly IConverter _converter;
        private readonly IRazorRenderer _razorRenderer;
        private readonly IPractitionerService _practitionerService;
        private readonly IQRService _qrService;
        private readonly IS3StorageService _storageService;
        private readonly ILogger<PdfService> _logger;
        private readonly IAppResourceService _resourceService;

        public PdfService(
            IConverter converter,
            IRazorRenderer razorRenderer,
            IPractitionerService practitionerService,
            IQRService qrService,
            IS3StorageService storageService, 
            ILogger<PdfService> logger,
            IAppResourceService resourceService)
        {
            _converter = converter;
            _razorRenderer = razorRenderer;
            _practitionerService = practitionerService;
            _qrService = qrService;
            _storageService = storageService;
            _logger = logger;
            _resourceService = resourceService;
        }

        public async Task<byte[]> GeneratePdfRazorAsync(string serviceCode, string QrUrl, int status)
        {
            try
            {
                var model = await _practitionerService.GetDualPracticeReport(serviceCode);
                _logger.LogInformation($"********** GeneratePdfRazorAsync, model : {model} ******");
                var QrBytes = await _qrService.GetQrCode(QrUrl);
                _logger.LogInformation($"********** GeneratePdfRazorAsync, QrBytes : {QrBytes} ******");
                model.QrCode = Convert.ToBase64String(QrBytes);
                _logger.LogInformation($"********** GeneratePdfRazorAsync, model.QrCode : {model.QrCode} ******");


                var htmlView = "/Views/DualPractitioner/DP_report.cshtml";
                if (status == (int)RequestStatus.Cancelled_Pr1 || (status == (int)RequestStatus.Cancelled_Pu2 && ((DateTime)model.to).Date <= DateTime.Now.Date))
                    htmlView = "/Views/DualPractitioner/DP_report_canceled.cshtml";

                var htmlContent = _razorRenderer.RenderPartialToString(htmlView, model);
                _logger.LogInformation($"********** GeneratePdfRazorAsync, htmlContent : {htmlContent} ******");

                return GeneratePdf(htmlContent);
            }
            catch (Exception e)
            {
                _logger.LogError($"Error while GeneratePdfRazorAsync Catch *** , Exception Message : {e.Message} ");
                throw new CustomHttpException(HttpStatusCode.BadRequest, _resourceService.GetResource("Error_Generate_Pdf"));
            }

        }

        public async Task<ResultOfAction<GetPdfUrl>> GetDualPracticeReportUrl(string serviceCode)
        {
            var model = await _practitionerService.GetDualPracticeReport(serviceCode);
            var status = model.status;

            // TODO: check if canceled file generated b4
            if (model.reportUrl != null && model.reportUrl != "" && status == (int)RequestStatus.Approved)
            {
                _logger.LogInformation($"GetDualPracticeReportUrl == {model.reportUrl}  ");
                return new ResultOfAction<GetPdfUrl>((int)HttpStatusCode.OK, null, new GetPdfUrl { PdfUrl = model.reportUrl });
            }

            // Upload to S3
            var url = await _storageService.UploadToS3Async(serviceCode, new MemoryStream());

            try
            {
                var PdfBytes = await GeneratePdfRazorAsync(serviceCode, url, status);
                bool hasAllZeroes = PdfBytes.All(singleByte => singleByte == 0);

                if (hasAllZeroes)
                    _logger.LogInformation($"************** while GetDualPracticeReportUrl , PdfBytes has All Zeroes");

                var PdfStream = new MemoryStream(PdfBytes);
                var requestUrl = await _storageService.UploadToS3Async(serviceCode, PdfStream);
                await _practitionerService.SaveDualPracticeUrl(serviceCode, requestUrl, status);

                return new ResultOfAction<GetPdfUrl>((int)HttpStatusCode.OK, null, new GetPdfUrl { PdfUrl = requestUrl });
            }
            catch (Exception e)
            {
                _logger.LogError($"Error while GetDualPracticeReportUrl , Exception Message : {e.Message} ");
                throw new CustomHttpException(HttpStatusCode.BadRequest, _resourceService.GetResource("Error_Generate_Pdf"));
            }
        }

        private byte[] GeneratePdf(string htmlContent)
        {
            try
            {
                if (htmlContent == "" || htmlContent == null)
                {
                    _logger.LogError($"Error while GeneratePdf , htmlContent null or empty");
                    throw new CustomHttpException(HttpStatusCode.BadRequest, _resourceService.GetResource("Error_Generate_Pdf"));
                }
                var globalSettings = new GlobalSettings
                {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Portrait,
                    //PaperSize = PaperKind.A4,
                    Margins = new MarginSettings(0, 0, 0, 0)
                };

                var objectSettings = new ObjectSettings
                {
                    PagesCount = true,
                    HtmlContent = htmlContent,
                    WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet = "wwwroot/css/style.css" }
                };

                var htmlToPdfDocument = new HtmlToPdfDocument()
                {
                    GlobalSettings = globalSettings,
                    Objects = { objectSettings },
                };
                return _converter.Convert(htmlToPdfDocument);
            }
            catch (Exception e)
            {
                _logger.LogError($"Error while GeneratePdf , Exception Message : {e.Message} ");
                throw new CustomHttpException(HttpStatusCode.BadRequest, _resourceService.GetResource("Error_Generate_Pdf"));
            }
           
        }
    }
}
