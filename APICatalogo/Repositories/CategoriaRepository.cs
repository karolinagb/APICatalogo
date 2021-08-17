using APICatalogo.Data;
using APICatalogo.Models;
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

        public async Task<List<Categoria>> GetCategoriasProdutos()
        {
            return await _aPICatalogoDbContext.Categorias.Include(x => x.Produtos).AsNoTracking().ToListAsync();
            //ou
            //return Get().Include(x => x.Produtos).ToList();
        }
    }
}
