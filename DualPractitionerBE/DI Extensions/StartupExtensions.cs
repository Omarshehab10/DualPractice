using Common.Localization;
using DAL.Models;
using DAL.Repository;
using DinkToPdf;
using DinkToPdf.Contracts;
using DTO.Validation;
using DualPractitionerBE.Custom;
using DualPractitionerBE.Mapping_Profiles;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Services;
using Services.BaseHttpService;
using Services.Common;
using Services.ExternalServices.Anat;
using Services.ExternalServices.HLS;
using Services.ExternalServices.OrganizationRegistry;
using Services.ExternalServices.PractitionersRegistry;
using Services.ExternalServices.SehaEndPoint;
using Services.GovOrganization;
using Services.Integration;
using Services.PaymentService;
using Services.PractitionerService;
using Services.PrivateOrganization;
using Services.S3Storage;
using Services.UnitOfWork;
using System;
using System.IO;
using System.Reflection;
using Services.IdentityServices;
using Lean.Framework.Entities.Interface;
using Services.ExternalServices.SendEmailService;

namespace DualPractitionerBE.DI_Extensions
{
    public static class StartupExtensions
    {
        public static void ConfigureAPI(this WebApplicationBuilder builder)
        {
            builder.Services.AddRazorPages();

            builder.Services.AddDbContext<DualPracticeContext>(options => options.UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole().AddDebug()))
                                                                                     .UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddControllers().AddFluentValidation(x =>

            {
                x.DisableDataAnnotationsValidation = false;
                x.RegisterValidatorsFromAssemblyContaining<ValidationLocation>();
            }

            );

            builder.Services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });

            builder.Services.AddAutoMapper(typeof(MappingProfile));
            builder.Services.AddHttpClient();
            builder.Services.AddHttpContextAccessor();
            builder.Services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();
        }

        public static void ConfigureSwagger(this WebApplicationBuilder builder)
        {
            builder.Services.AddSwaggerGen(options =>
            {
                #region xmlComments
                //var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                //var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                //options.IncludeXmlComments(xmlPath);

                ////include xml comments for DTOs
                //var DtoXmlPath = Path.Combine(AppContext.BaseDirectory, "DTO.xml");
                //options.IncludeXmlComments(DtoXmlPath);
                #endregion

                //var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";

                //var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                //options.IncludeXmlComments(xmlPath);

                //options.SwaggerDoc("V1", new OpenApiInfo()
                //{
                //    Title = "Dual Practitioner API",
                //    Description = "Documentation for the API with UI to simulate the http requests",
                //    Version = "V1",
                //    //Contact = new OpenApiContact() { Email = "e-o.shehab@lean.sa", Name = "Omar Shehab" }
                //});
                //services.AddSwaggerGen();
                options.DocumentFilter<SwaggerDocumentFilter>();
                //options.OperationFilter<SwaggerOperationFilter>();

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme= "https"
                        },
                        new string[] {"https" }
                    }
                });
            });
        }

        public static void ConfigureDependancyInjection(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            builder.Services.AddScoped<IIntegrationService, IntegrationService>();
            builder.Services.AddScoped<IAppResourceService, AppResourceService>();
            builder.Services.AddScoped<IBaseHttpService, BaseHttpService>();
            builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
            builder.Services.AddTransient<IPractitionerService, PractitionerService>();
            builder.Services.AddTransient<IGovOrganizationService, GovOrganizationService>();
            builder.Services.AddTransient<IPractitionersRegistryService, PractitionersRegistryService>();
            builder.Services.AddTransient<IEstablishmentHLS, EstablishmentHLS>();
            builder.Services.AddTransient<ISehaService, SehaService>();
            builder.Services.AddTransient<IPaymentService, PaymentService>();
            builder.Services.AddTransient<IPrivateOrganizationService, PrivateOrganizationService>();
            builder.Services.AddTransient<IOrganizationRegistryService, OrganizationRegistryService>();
            builder.Services.AddTransient<IQRService, QRService>();
            builder.Services.AddTransient<IS3StorageService, StorageService>();

            builder.Services.AddTransient<IUserService, UserService>();

            builder.Services.AddTransient<ICommonService, CommonService>();
            builder.Services.AddTransient<IRepositoryBase, RepositoryBase>();
            builder.Services.AddTransient<IAnatService, AnatService>();
            builder.Services.AddTransient<IEmailService, EmailService>();

            #region UI builder.Services
            builder.Services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));
            builder.Services.AddTransient<IRazorRenderer, RazorRenderer>();
            builder.Services.AddTransient<IPdfService, PdfService>();
            #endregion

        }

        public static void ConfigureLocalization(this WebApplicationBuilder builder)
        {
            builder.Services.AddLocalization();
        }

        public static void ConfigureCORS(this WebApplicationBuilder builder)
        {
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("Default", builder =>
                {
                    builder.AllowAnyOrigin();
                    builder.AllowAnyHeader();
                    builder.AllowAnyMethod();
                });
            });
        }
    }
}
