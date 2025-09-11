using System.Linq.Expressions;

namespace APICatalogo.Infrastructure.Repositories;

public interface IRepository<T>
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetAsync(Expression<Func<T, bool>> predicate);                    //Dessa forma podemos buscar por nome, id e etc. podendo aceitar como parametro uma funçao lambda
    T Create(T entity);
    T Update(T entity);
    T Delete(T entity );
}
