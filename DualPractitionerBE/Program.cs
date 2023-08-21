using DTO.DTOs.EmailSetting;
using DualPractitionerBE;
using DualPractitionerBE.DI_Extensions;
using Lean.Framework.Entities.Integration;
using Lean.Integration.Seha.AspNetCore.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.Globalization;
using System;
//using Elastic.Apm.NetCoreAll;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureAPI();
builder.ConfigureLocalization();
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.ConfigureSwagger();
builder.ConfigureCORS();
builder.ConfigureDependancyInjection();
builder.Services.AddControllersWithViews();
builder.Services.AddMvcCore();
builder.Services.AddMvc();
builder.Services.Configure<CookieTempDataProviderOptions>(options =>
{
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.Always;
    options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.None;
});
builder.Services.AddSingleton<SehaIntegrationConfig>(builder.Configuration.GetSection("SehaIntegrationConfig").Get<SehaIntegrationConfig>());
builder.Services.AddSehaService<SehaUserService>(builder.Configuration, builder.Environment.IsDevelopment());

var app = builder.Build();

//Content of Configure() Function
//if (env.IsDevelopment())
//{
//    app.UseDeveloperExceptionPage();
//}

app.UseForwardedHeaders();

app.UseExceptionHandler("/error");


AppDomain.CurrentDomain.SetData("ContentRootPath", builder.Environment.ContentRootPath);


app.UseMiddleware<Lean.Integration.Seha.AspNetCore.Middleware.AuthenticationMiddleware>();


IList<CultureInfo> supportedCultures = new List<CultureInfo>
            {
                new CultureInfo("ar"),
                new CultureInfo("en"),
                new CultureInfo("ar-SA")
            };

app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("en"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
});

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors("Default");
app.UseRouting();

//app.UseAuthorization();
//app.UseAllElasticApm(builder.Configuration);

app.UseEndpoints(endpoints =>
{
    if (!builder.Environment.IsDevelopment())
    {
        endpoints.MapControllerRoute(
           name: "default",
           pattern: "{controller=SehaAccount}/{action=Login}/{id?}");
    }
    //endpoints.MapRazorPages();
    endpoints.MapDefaultControllerRoute();
    endpoints.MapControllers();
});

//app.UseSwagger();
//app.UseSwaggerUI(options =>
//{
//    options.SwaggerEndpoint("V1/swagger.json", "Dual Practitioner API");
//});
app.UseSwagger(c =>
{
    c.RouteTemplate = "api/swagger/{documentname}/swagger.json";
});

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/api/swagger/v1/swagger.json", "Dual Practitioner API");
    c.RoutePrefix = "api/swagger";
});
app.Run();
