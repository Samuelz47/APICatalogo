namespace APICatalogo.Shared.Pagination;

public class ProductsParameters
{
    const int maxPageSize = 50;
    public int PageNumber { get; set; } = 1;
    private int _pageSize;
    public int PageSize
    {
        get
        {
            return _pageSize;
        }
        set
        {
            _pageSize = (value > maxPageSize) ? maxPageSize : value;    //se o numero de produtos for maior que o maxPageSize ele mostrara 50, se for menor mostrará apenas a quantidade
        }
    }
}
