using Data;
using Microsoft.EntityFrameworkCore;

namespace CrashLogger.Utils
{
    public static class DatabaseServiceManagement
    {
        public static void MigrationInitialization(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                serviceScope.ServiceProvider.GetService<ApplicationContext>().Database.Migrate();
            }
        }
    }
}
