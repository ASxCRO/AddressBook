using AddressBook.Data;
using AddressBook.Data.Repositories.Abstraction;
using AddressBook.Data.Repositories.Implementation;
using AddressBook.Localization;
using AddressBook.Utils;
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using System.Reflection;

namespace AddressBook
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddScoped<DbConnectionFactory>();
            builder.Services.AddScoped<IContactsRepository, ContactsRepository>();

            builder.Services.AddSingleton<LanguageService>();

            builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
            builder.Services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[] { "hr", };
                options.SetDefaultCulture(supportedCultures[0])
                    .AddSupportedCultures(supportedCultures)
                    .AddSupportedUICultures(supportedCultures);
            });


            builder.Services.AddControllersWithViews().AddViewLocalization().AddDataAnnotationsLocalization(options => {
                options.DataAnnotationLocalizerProvider = (type, factory) => {
                    var assemblyName = new AssemblyName(typeof(SharedResource).GetTypeInfo().Assembly.FullName);
                    return factory.Create("SharedResource", assemblyName.Name);
                };
            });

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            //app.UseGlobalExceptionHandler();

            var supportedCultures = new[]
            {
                new CultureInfo("hr"),
            };

            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("hr"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            });

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Contacts}/{action=Index}/{id?}");


            app.Run();
        }
    }
}
