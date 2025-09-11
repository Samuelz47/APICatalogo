namespace APICatalogo.Shared.Pagination;

public class FilterProductsPrice : QueryStringParameters
{
    public decimal? Price { get; set; }
    public string? PriceCriterion { get; set; }      //"maior", "menor" ou "igual
}
