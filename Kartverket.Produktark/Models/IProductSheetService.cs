namespace Kartverket.Produktark.Models
{
    public interface IProductSheetService
    {
        ProductSheet CreateProductSheetFromMetadata(string uuid);

        Logo FindLogoForOrganization(string organization);
    }

}
