using System.Collections.Generic;
namespace Kartverket.Produktark.Models
{
    public interface IProductSheetService
    {
        ProductSheet CreateProductSheetFromMetadata(string uuid);

        Logo FindLogoForOrganization(string organization);

        List<ProductSheet> FindProductSheetsForOrganization(string organization);
    }

}
