using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CSharpFunctionalExtensions;
using CSN_Energy_Task.Entity;
using CSN_Energy_Task.Model;
using CSN_Energy_Task.ValueObjects;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema;

namespace CSN_Energy_Task
{
    public class BookStore : IStore
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(BookStore));
        private readonly JSchema _jsonSchema;
        private readonly ICalculationRule _calculationRule;
        private IEnumerable<Category> _categories;
        private IEnumerable<Catalog> _catalogs;


        private BookStore(JSchema schema, ICalculationRule calculationRule)
        {
            _jsonSchema = schema;
            _calculationRule = calculationRule;
        }

        public static Result<BookStore> Create(string schemaFilePath, ICalculationRule calculationRule)
        {
            return FileValidation.Validate<BookStore>(schemaFilePath, () =>
            {
                using (StreamReader file = File.OpenText(schemaFilePath))
                using (JsonTextReader reader = new JsonTextReader(file))
                {
                    JSchema schema = JSchema.Load(reader);
                    return Result.Ok(new BookStore(schema, calculationRule));
                }
            });
        }

        public Result Import(string catalogAsJson)
        {
            using (var reader = new JsonTextReader(new StringReader(catalogAsJson)))
            using (var validatingReader = new JSchemaValidatingReader(reader))
            {
                validatingReader.Schema = _jsonSchema;

                IList<Result> validationErrors = new List<Result>();
                validatingReader.ValidationEventHandler += (o, a) => validationErrors.Add(Result.Fail(a.Message));

                JsonSerializer serializer = new JsonSerializer();
                BookStoreInventoryDto inventory = null;

                try
                {
                    inventory = serializer.Deserialize<BookStoreInventoryDto>(validatingReader);
                }
                catch (JsonReaderException e)
                {
                    validationErrors.Add(Result.Fail(e.Message));
                }

                if (validationErrors.Count > 0)
                    return Result.Combine(Environment.NewLine, validationErrors.ToArray());

                return DtoToObjectMapper(inventory, out _categories, out _catalogs);
            }
        }

        public Result<int> Quantity(string name)
        {
            return Result.Ok()
                .Ensure(() => _catalogs != null && _catalogs.Any(), string.Format("Catalog is empty. Please import a catalog before asking for quantity."))
                .Map(() => this._catalogs.FirstOrDefault(c => c.Item.Name == name))
                .Ensure(item => item != null, string.Format("A book with name {0} doesn't exisit in the catalog", name))
                .Map(item => item.Quantity);
        }

        public Result<decimal> Buy(params string[] basketByNames)
        {
            var selectedCatalogs = basketByNames.Select(name =>
                Result.Ok(_catalogs.FirstOrDefault(c => c.Item.Name == name))
                .Ensure(catalog => catalog != null, string.Format("Catalog with name {0} can not be found", name)));

            if (selectedCatalogs.HasFailure())
                return Result.Fail<decimal>(selectedCatalogs.GetFailure().Error);
    
            return _calculationRule.Calculate(selectedCatalogs.Select(x => x.Value));
        }

        // Mapper/Factory or whatever you like to call it.
        private Result DtoToObjectMapper(BookStoreInventoryDto inventory, out IEnumerable<Category> categories, out IEnumerable<Catalog> catalogs)
        {
            categories = new HashSet<Category>();
            catalogs = new HashSet<Catalog>();
            var categoriesMappingResult = inventory.Category.Select(c => Category.Create(c.Name, c.Discount));
            if (categoriesMappingResult.HasFailure())
                return categoriesMappingResult.GetFailure();

            var categoriesValues = categoriesMappingResult.Select(r => r.Value);

            var catalogMappingResult = inventory.Catalog.Select(catalog =>
                Result.Ok(categoriesValues.FirstOrDefault(category => category.Name == catalog.Category))
                    .Ensure(category => category != null, string.Format("The category specified {0} doesn't match to any of the provided categories", catalog.Category))
                    .Map(category => Catalog.Create(catalog.Name, category, catalog.Price, catalog.Quantity))
                    .ToSingleResult());

            if (catalogMappingResult.HasFailure())
                return categoriesMappingResult.GetFailure();

            categories = categoriesValues;
            catalogs = catalogMappingResult.Select(r => r.Value);
            return Result.Ok();
        }
    }
}
