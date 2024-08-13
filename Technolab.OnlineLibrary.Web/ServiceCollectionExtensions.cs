using Technolab.OnlineLibrary.Web.Models;

namespace Technolab.OnlineLibrary.Web
{
    public static class ServiceCollectionExtensions
    {
        public static void AddDbContextFactory(this IServiceCollection services)
        {
            services.AddSingleton<ILibraryDbContextFactory>(x =>
                new FileBasedLibraryDbContextFactory(directoryPath: x.GetRequiredService<IConfiguration>()[ConfigurationKeys.FileBasedDbContextDirectory]));
        }
    }
}
