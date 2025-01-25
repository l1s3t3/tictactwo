using DAL;
using Microsoft.EntityFrameworkCore;

namespace ConsoleApp
{
    class Program
    {
        static void Main()
        {
            bool useDatabase = false; // true to use database, false to use JSON files

            if (useDatabase)
            {
                InitializeDatabase();

                var connectionString = $"Data Source={FileHelper.BasePath}app.db";
                var contextOptions = new DbContextOptionsBuilder<AppDbContext>()
                    .UseSqlite(connectionString)
                    .EnableDetailedErrors()
                    .EnableSensitiveDataLogging()
                    .Options;

                var dbContext = new AppDbContext(contextOptions);
                
                GameController.SetGameRepository(new GameRepositoryDb(dbContext));
            }
            else
            {
                GameController.SetGameRepository(new GameRepositoryJson());
            }

            string result;
            
            do
            {
                result = Menus.MainMenu.Run();
            } while (result != "E");

            Console.WriteLine("Thank you for playing Tic-Tac-Two!");
        }

        private static void InitializeDatabase() 
        { 
            var connectionString = $"Data Source={FileHelper.BasePath}app.db"; 
            Console.WriteLine("Database is in: " + connectionString); 
         
            var contextOptions = new DbContextOptionsBuilder<AppDbContext>() 
                .UseSqlite(connectionString) 
                .EnableDetailedErrors() 
                .EnableSensitiveDataLogging() 
                .Options; 
         
            using var ctx = new AppDbContext(contextOptions); 
            ctx.Database.Migrate(); 
        } 
    }
}