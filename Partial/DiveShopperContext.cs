using Microsoft.EntityFrameworkCore;

namespace diveWebAPI.Partial
{
        public partial class DiveShopperContext : DbContext
        {
            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                if (!optionsBuilder.IsConfigured)//讀取設定檔的資料連線
                {
                    IConfiguration Config = new ConfigurationBuilder()
                        .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                        .AddJsonFile("appsettings.json")
                        .Build();
                    optionsBuilder.UseSqlServer(Config.GetConnectionString("DiveShopper"));
                }
            }

        }
}
