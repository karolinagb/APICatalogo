using APICatalogo.Models;
using APICatalogo.Pagination;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace APICatalogo.Repositories.Interfaces
{
    public interface ICategoriaRepository : IRepository<Categoria>
    {
        Task<List<Categoria>> GetCategoriasProdutos();
        Task<PagedList<Categoria>> GetCategorias(CategoriasParameters categoriasParameters);
    }
}
