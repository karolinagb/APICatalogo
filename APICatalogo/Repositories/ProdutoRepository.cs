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
    public class ProdutoRepository : Repository<Produto>, IProdutoRepository
    {
        public ProdutoRepository(APICatalogoDbContext aPICatalogoDbContext) : base(aPICatalogoDbContext) { }

        public async Task<List<Produto>> GetProdutoPorPreco()
        {
            return await _aPICatalogoDbContext.Produtos.OrderBy(x => x.Preco).AsNoTracking().ToListAsync();
            //ou
            //return Get().OrderBy(c => c.Preco).ToList();
        }

        public async Task<PagedList<Produto>> GetProdutos(ProdutosParameters produtosParameters)
        {

            return await PagedList<Produto>.ToPagedList(_aPICatalogoDbContext.Produtos
                .OrderBy(x => x.Nome), produtosParameters.PageNumber, produtosParameters.PageSize);
        }
    }
}
