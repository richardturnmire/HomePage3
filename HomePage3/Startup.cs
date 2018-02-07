using System;
using System.IO;
using HomePage3.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using Microsoft.Extensions.DependencyInjection;

namespace HomePage3
{
    public class Startup
    {
        public string EmailAccount = null;
        public string EmailPsWord = null;

        public Startup(IConfiguration config, IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder();

            builder.SetBasePath(Directory.GetCurrentDirectory());

            if (env.IsDevelopment())
            {
                builder
                    .AddUserSecrets<EmailConfiguration>()
                    .AddJsonFile("appsettings.development.json", optional: false);
            }

            builder.AddEnvironmentVariables();

            if (!env.IsDevelopment())
            {
                builder
                    .AddAzureKeyVault(
                       $"https://{config["vault"]}vault.vault.azure.net/",
                       config["ClientId"],
                       config["ClientSecret"],
                       new PrefixKeyVaultSecretManager(""))
                    .AddJsonFile("appsettings.json", optional: false);

            }

            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddOptions();
            services.AddCors(options => options.AddPolicy("MyCors", policy => policy.AllowAnyOrigin()));
            services.AddSingleton<IEmailConfiguration>(Configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>());
            services.AddTransient<IEmailService, EmailService>();
            services.AddSingleton<IConfiguration>(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.Use(async (context, next) =>
            {
                context.Response.Headers.Add(
                    "Content-Security-Policy",
                    "frame-ancestors 'self' https:   ; " +
                    "default-src 'self' https:   azure.net https://*.googleapis.com ; " +
                    "child-src 'self' https:  ; " +
                    "style-src 'self' 'unsafe-inline' https://maxcdn.bootstrapcdn.com  https://*.googleapis.com ; " +
                    "font-src 'self' https://assets-cdn.github.com data: https://*.googleapis.com https://*.gstatic.com ; " +
                    "script-src 'self' https://maxcdn.bootstrapcdn.com https://cdnjs.cloudflare.com https://use.fontawesome.com " +
                          "https://www.brainyquote.com https: 'unsafe-inline' ; " +
                    "img-src 'self' ; ");

                context.Response.Headers.Add(
                    "Strict-Transport-Security",
                    "max-age=86400; includeSubDomains");

                context.Response.Headers.Add("X-Frame-Options", "SAMEORIGIN");
                context.Response.Headers.Add("X-Xss-Protection", "1");
                await next();
            });

            app.UseStaticFiles();
            app.UseCors("MyCors");

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });
        }
    }

    public class PrefixKeyVaultSecretManager : IKeyVaultSecretManager
    {
        private readonly string _prefix;

        public PrefixKeyVaultSecretManager(string prefix)
        {
            _prefix = $"{prefix}";
        }

        public bool Load(SecretItem secret)
        {
            // Load a vault secret when its secret name starts with the 
            // prefix. Other secrets won't be loaded.
            return secret.Identifier.Name.StartsWith(_prefix);
        }

        public string GetKey(SecretBundle secret)
        {
            // Remove the prefix from the secret name and replace two 
            // dashes in any name with the KeyDelimiter, which is the 
            // delimiter used in configuration (usually a colon). Azure 
            // Key Vault doesn't allow a colon in secret names.
            return secret.SecretIdentifier.Name
                .Substring(_prefix.Length)
                .Replace("--", ConfigurationPath.KeyDelimiter);
        }
    }
}


