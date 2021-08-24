using APICatalogo.Data;
using APICatalogo.Models;
using APICatalogo.Pagination;
using APICatalogo.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APICatalogo.Repositories
{
    public class CategoriaRepository : Repository<Categoria>, ICategoriaRepository
    {
        public CategoriaRepository(APICatalogoDbContext aPICatalogoDbContext) : base(aPICatalogoDbContext)
        {
        }

        public async Task<PagedList<Categoria>> GetCategorias(CategoriasParameters categoriasParameters)
        {
            return await PagedList<Categoria>.ToPagedList(_aPICatalogoDbContext.Categorias.OrderBy(x => x.Id), categoriasParameters.PageNumber,
                categoriasParameters.PageSize);
        }

        public async Task<List<Categoria>> GetCategoriasProdutos()
        {
            return await _aPICatalogoDbContext.Categorias.Include(x => x.Produtos).AsNoTracking().ToListAsync();
            //ou
            //return Get().Include(x => x.Produtos).ToList();
        }
    }
}
