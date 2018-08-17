using HomePage3.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.IO.Compression;

namespace HomePage3
{
    public class Startup
    {
        public string EmailAccount = null;
        public string EmailPsWord = null;

        public Startup (IConfiguration config, IHostingEnvironment env)
        {
            ConfigurationBuilder builder = new ConfigurationBuilder();

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

        private IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices (IServiceCollection services)
        {
            services.AddResponseCaching();
            services.AddMvc(options =>
            {
                options.CacheProfiles.Add("Default",
                    new CacheProfile() {
                        Duration = 60
                    });
                options.CacheProfiles.Add("Never",
                    new CacheProfile() {
                        Location = ResponseCacheLocation.None,
                        NoStore = true
                    });
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddResponseCompression(options =>
            {
                options.Providers.Add(new BrotliCompressionProvider());
            });

            services.AddOptions();
            services.AddHsts(options =>
            {
                options.MaxAge = TimeSpan.FromDays(1);
                options.IncludeSubDomains = true;
                options.Preload = true;
            });
            services.AddCors(options => options.AddPolicy("MyCors", policy => policy.AllowAnyOrigin()));
            services.AddSingleton<IEmailConfiguration>(Configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>());
            services.AddTransient<IEmailService, EmailService>();
            services.AddSingleton(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure (IApplicationBuilder app, IHostingEnvironment env)
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

            app.UseResponseCompression();
            app.UseHsts();

            app.Use(async (context, next) =>
            {
                context.Response.GetTypedHeaders().CacheControl =
                    new Microsoft.Net.Http.Headers.CacheControlHeaderValue() {
                        Public = true,
                        MaxAge = TimeSpan.FromSeconds(10)
                    };
                context.Response.Headers[Microsoft.Net.Http.Headers.HeaderNames.Vary] =
                    new string[] { "Accept-Encoding" };

                string styles = "style-src 'self' 'unsafe-inline' " +
                    "https://use.fontawesome.com " +
                     "https://fonts.gstatic.com " +
                     "https://fonts.googleapis.com " +
                    "https://cdnjs.cloudflare.com ; ";

                string fonts = "font-src 'self' data: " +
                     "https://cdnjs.cloudflare.com " +
                     "https://assets-cdn.github.com " +
                     "https://fonts.googleapis.com " +
                     "https://fonts.gstatic.com " +
                     "https://use.fontawesome.com ; ";

                string scripts = "script-src 'self' 'unsafe-inline' " +
                        "https://cdnjs.cloudflare.com " +
                        "https://use.fontawesome.com " +
                        "https://www.brainyquote.com " +
                        "https:  ; ";

                context.Response.Headers.Add(
                    "Content-Security-Policy",
                    "frame-ancestors 'self' https:   ; " +
                    "default-src 'self' https:   azure.net https://*.googleapis.com ; " +
                    "child-src 'self' https:  ; " +
                    styles +
                    fonts +
                    scripts +
                    "connect-src 'self' wss: https: ; " +
                    "img-src 'self' ; ");

                context.Response.Headers.Add(
                    "Strict-Transport-Security",
                    "max-age=86400; includeSubDomains");

                context.Response.Headers.Add("X-Frame-Options", "SAMEORIGIN");
                context.Response.Headers.Add("X-Xss-Protection", "1");
              //  context.Response.Headers.Add("Cache-Control", "max-age=86400");

                await next();
            });
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseResponseCaching();
            app.UseCors("MyCors");

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });
        }
    }
    public class BrotliCompressionProvider : ICompressionProvider
    {
        public string EncodingName => "br";
        public bool SupportsFlush => true;

        public Stream CreateStream (Stream outputStream)
        {
            return new BrotliStream(
                outputStream,
                CompressionLevel.Optimal,
                false);
        }
    }
    public class PrefixKeyVaultSecretManager : IKeyVaultSecretManager
    {
        private readonly string _prefix;

        public PrefixKeyVaultSecretManager (string prefix)
        {
            _prefix = $"{prefix}";
        }

        public bool Load (SecretItem secret)
        {
            // Load a vault secret when its secret name starts with the 
            // prefix. Other secrets won't be loaded.
            return secret.Identifier.Name.StartsWith(_prefix);
        }

        public string GetKey (SecretBundle secret)
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


