using ECommerce.Core.Entities;

namespace ECommerce.Core.Specifications;

public class ProductsWithCategoriesSpecification : BaseSpecification<Product>
{
    public ProductsWithCategoriesSpecification(string? sort, int? categoryId, string? search)
        : base(x => 
            (string.IsNullOrEmpty(search) || x.Name.ToLower().Contains(search.ToLower()) || x.Description.ToLower().Contains(search.ToLower())) &&
            (!categoryId.HasValue || x.CategoryId == categoryId)
        )
    {
        AddInclude(x => x.Category);
        AddInclude(x => x.Images);
        AddOrderBy(x => x.Name);

        if (!string.IsNullOrEmpty(sort))
        {
            switch (sort)
            {
                case "priceAsc":
                    AddOrderBy(p => p.Price);
                    break;
                case "priceDesc":
                    AddOrderByDescending(p => p.Price);
                    break;
                default:
                    AddOrderBy(n => n.Name);
                    break;
            }
        }
    }

    public ProductsWithCategoriesSpecification(int id) 
        : base(x => x.Id == id)
    {
        AddInclude(x => x.Category);
        AddInclude(x => x.Images);
    }
}
