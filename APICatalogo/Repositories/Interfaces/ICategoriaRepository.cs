using APICatalogo.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace APICatalogo.Repositories.Interfaces
{
    public interface ICategoriaRepository : IRepository<Categoria>
    {
        Task<List<Categoria>> GetCategoriasProdutos();
    }
}
