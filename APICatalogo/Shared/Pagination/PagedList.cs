namespace APICatalogo.Shared.Pagination;

public class PagedList<T> : List<T> where T : class
{
    public int CurrentPage { get; private set; }    //pagina atual
    public int TotalPages { get; private set; }     //total de paginas
    public int PageSize { get; private set; }       //numero de elementos exibidos 
    public int TotalCount { get; private set; }     //numero total de elementos
    public bool HasPrevious => CurrentPage > 1;
    public bool HasNexts => CurrentPage < TotalPages;
    
    public PagedList(List<T> items, int count, int pageNumber, int pageSize)
    {
        TotalCount = count;
        PageSize = pageSize;
        CurrentPage = pageNumber;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);

        AddRange(items);
    }

    public static PagedList<T> ToPagedList(IQueryable<T> source, int pageNumber, int pageSize)
    {
        var count = source.Count();
        var items = source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

        return new PagedList<T>(items, count, pageNumber, pageSize);
    }
}
