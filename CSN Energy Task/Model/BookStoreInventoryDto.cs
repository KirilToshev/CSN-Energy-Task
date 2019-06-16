using System;
using System.Collections.Generic;
using System.Text;

namespace CSN_Energy_Task.Model
{
    public class BookStoreInventoryDto
    {
        public CategoryDto[] Category { get; set; }
        public CatalogDto[] Catalog { get; set; }
    }
}
