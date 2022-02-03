using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SalesWG.Shared.Admin.Models.Catalog.Category;

namespace SalesWG.Shared.Admin.Responses.Catalog.Category
{
    public class CategoryResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ParentCategory ParentCategory { get; set; }
    }
}
