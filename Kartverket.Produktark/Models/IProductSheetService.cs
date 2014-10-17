using System.Collections.Generic;
namespace Kartverket.Produktark.Models
{
    public interface IProductSheetService
    {
        ProductSheet CreateProductSheetFromMetadata(string uuid);
        ProductSheet UpdateProductSheetFromMetadata(string uuid, ProductSheet productSheet);

        Logo FindLogoForOrganization(string organization);

        List<ProductSheet> FindProductSheetsForOrganization(string organization);
    }

}
