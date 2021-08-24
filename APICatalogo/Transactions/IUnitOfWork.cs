using APICatalogo.Repositories.Interfaces;
using System.Threading.Tasks;

namespace APICatalogo.Transactions
{
    public interface IUnitOfWork
    {
        IProdutoRepository ProdutoRepository { get; }
        ICategoriaRepository CategoriaRepository { get; }
        Task Commit();
        void Dispose();
    }
}
