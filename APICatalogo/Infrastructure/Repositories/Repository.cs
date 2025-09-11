using APICatalogo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq.Expressions;

namespace APICatalogo.Infrastructure.Repositories;

public class Repository<T> : IRepository<T> where T : class     //Onde T é uma classe
{
    protected readonly AppDbContext _context;

    public Repository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _context.Set<T>().AsNoTracking().ToListAsync();                  //o Set serve para pegar uma coleção ou tabela do tipo T(no caso a classe)
    }

    public async Task<T?> GetAsync(Expression<Func<T, bool>> predicate)
    {
        return await _context.Set<T>().FirstOrDefaultAsync(predicate);
    }

    public T Create(T entity)
    {
        _context.Set<T>().Add(entity);

        return entity;
    }

    public T Update(T entity)
    {
        _context.Entry(entity).State = EntityState.Modified;

        return entity;
    }

    public T Delete(T entity)
    {
        _context.Set<T>().Remove(entity);

        return entity;
    }
}
