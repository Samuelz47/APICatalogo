namespace APICatalogo.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private IProductRepository? _productRepo;

    private ICategoryRepository? _categoryRepo;
    public AppDbContext _context;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public IProductRepository ProductRepository
    {
        get 
        { 
            if(_productRepo is null)
            {
                _productRepo = new ProductRepository(_context);             // Caso o repositorio de produtos esteja vazio esse código criaria um novo
            }
            return _productRepo;
        }
    }

    public ICategoryRepository CategoryRepository
    {
        get
        {
            if (_categoryRepo is null)
            {
                _categoryRepo = new CategoryRepository(_context);               //código serve para não ser criado varias instancias do mesmo repositorio evitando erros   
            }
            return _categoryRepo;          
        }
    }

    public void Commit()
    {
        _context.SaveChanges();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
