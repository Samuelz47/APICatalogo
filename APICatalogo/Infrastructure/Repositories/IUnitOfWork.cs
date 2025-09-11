namespace APICatalogo.Infrastructure.Repositories;

public interface IUnitOfWork
{
    IProductRepository ProductRepository { get; }
    ICategoryRepository CategoryRepository { get; }         //Usamos os repositorios especializados para ter acesso aos métodos personalizados, além do Crud que recebemos da herança
    Task CommitAsync();                  //método que confirma as alterações
}
