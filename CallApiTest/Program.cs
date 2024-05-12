using CallApiTest;
using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Cryptography.X509Certificates;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.Configure<KestrelServerOptions>(serverOptions =>
{
    serverOptions.ConfigureHttpsDefaults(httpsOptions =>
    {
       
        httpsOptions.ClientCertificateMode = ClientCertificateMode.RequireCertificate;
        // Optionally, you can also validate the certificate here
        httpsOptions.SslProtocols = System.Security.Authentication.SslProtocols.Tls12;
       

    });
});


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddSingleton<CertificateValidationService>();
//Add services to the container.

builder.Services.AddAuthentication(CertificateAuthenticationDefaults.AuthenticationScheme)
    .AddCertificate(options =>
    {
        options.AllowedCertificateTypes = CertificateTypes.All;
        options.ChainTrustValidationMode = X509ChainTrustMode.CustomRootTrust;
        //options.CustomTrustStore = new X509Certificate2Collection { rootCert };
        options.RevocationMode = X509RevocationMode.NoCheck;

        options.RevocationMode = System.Security.Cryptography.X509Certificates.X509RevocationMode.NoCheck;
        options.Events = new CertificateAuthenticationEvents
        {
            OnCertificateValidated = context =>
                {
                    var validationService = context.HttpContext.RequestServices.GetRequiredService<CertificateValidationService>();


                    if (validationService.ValidateCertificate(context.ClientCertificate))
                    {
                        Console.WriteLine("Success");
                        context.Success();
                    }
                    else
                    {
                        Console.WriteLine("Invalid cert");
                        context.Fail("Invalid cert");
                    }
                    return Task.CompletedTask;
                },

                OnAuthenticationFailed = context =>
                {
                    context.Fail("invalid cert");
                    return Task.CompletedTask;
                }
        };

    }
);
   

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

