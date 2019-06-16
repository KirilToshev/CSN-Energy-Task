using CSharpFunctionalExtensions;
using log4net;
using log4net.Config;
using System;
using System.IO;
using System.Reflection;

namespace CSN_Energy_Task
{
    class Program
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));
        static void Main(string[] args)
        {
            BasicConfigurator.Configure(LogManager.CreateRepository("AppLogRepository"));
            var storeResult = BookStore.Create(Path.Combine(AssemblyDirectory, "Assets", "bookStoreInventorySchema.json"), new PriceCalculationRule())
                .OnSuccess(bookstore =>
                {
                    var exampleFilePath = Path.Combine(AssemblyDirectory, "Assets", "bookStoreInventoryExample.json");
                    var jsonExampleResult = FileValidation.Validate<string>(exampleFilePath, () => Result.Ok(File.ReadAllText(exampleFilePath)));
                    if (jsonExampleResult.IsFailure)
                    {
                        Console.WriteLine(jsonExampleResult.Error);
                        return;
                    }

                    bookstore.Import(jsonExampleResult.Value)
                        .OnFailure(error => Console.WriteLine(error));
                })
                .OnFailure(error => Console.WriteLine(error));
            if (storeResult.IsFailure)
                return;

            var store = storeResult.Value;

            //store.Quantity("Ayn Rand - FountainHead").WriteToConsole();
            //store.Buy("Isaac Asimov - Foundation", "Isaac Asimov - Foundation").WriteToConsole();
            //store.Buy("J.K Rowling - Goblet Of fire", "J.K Rowling - Goblet Of fire").WriteToConsole();
            //store.Buy("J.K Rowling - Goblet Of fire", "Robin Hobb - Assassin Apprentice", "Robin Hobb - Assassin Apprentice").WriteToConsole();
            //store.Buy("Ayn Rand - FountainHead",
            //    "Isaac Asimov - Foundation",
            //    "Isaac Asimov - Robot series",
            //    "J.K Rowling - Goblet Of fire",
            //    "J.K Rowling - Goblet Of fire",
            //    "Robin Hobb - Assassin Apprentice",
            //    "Robin Hobb - Assassin Apprentice"
            //    ).WriteToConsole();
        }

        public static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                codeBase = codeBase.Remove(codeBase.IndexOf("bin"));
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                var test = Path.GetDirectoryName(path);
                return Path.GetDirectoryName(path);
            }
        }
    }
}
