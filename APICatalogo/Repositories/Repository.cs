using APICatalogo.Data;
using APICatalogo.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace APICatalogo.Repositories
{
    public class Repository<T> : IRepository<T> where T : class //T (tipo genérico) só pode ser uma classe
    {
        protected APICatalogoDbContext _aPICatalogoDbContext;

        public Repository(APICatalogoDbContext aPICatalogoDbContext)
        {
            _aPICatalogoDbContext = aPICatalogoDbContext;
        }

        public void Add(T entity)
        {
            _aPICatalogoDbContext.Set<T>().Add(entity);
            //_aPICatalogoDbContext.SaveChanges();
        }

        public void Delete(T entity)
        {
            _aPICatalogoDbContext.Set<T>().Remove(entity);
            //_aPICatalogoDbContext.SaveChanges();
        }

        public async Task<List<T>> Get()
        {
            //Entity Framework rastreia a consulta para ver se teve alteração no objeto
            //Isso pode deixar o desempenho ruim, então em ações Get nós desabilitamos isso através do
            //AsNoTracking
            return await _aPICatalogoDbContext.Set<T>().AsNoTracking().ToListAsync();
        }

        public T GetById(Expression<Func<T, bool>> predicate) // Essa expressão seria: x => x.Id == id
        {
            return _aPICatalogoDbContext.Set<T>().SingleOrDefault(predicate); //Nessa classe T genérica se eu colocasse no
                                                                              //SingleOrDefault assim x => x.Id == id,
                                                                              //não iria funcionar por a classe T não ser
                                                                              //uma tabela nem nada e sim algo genérico
        }

        public void Update(T entity)
        {
            _aPICatalogoDbContext.Entry(entity).State = EntityState.Modified;
            _aPICatalogoDbContext.Set<T>().Update(entity);
            //_aPICatalogoDbContext.SaveChanges();
        }

        //obs: O Set<T>() é um método do contexto que retorna uma instância DbSet<T> para o acesso a entidades de determinado
        // tipo no contexto. Ele acessa a tabela. É como se eu fizesse assim:
        // _context.Produtos.Update  (Eu tenho que fazer isso pois T é genérico, não existe nenhuma tabela T)
    }
}
