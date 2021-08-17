using APICatalogo.Repositories.Interfaces;

namespace APICatalogo.Transactions
{
    public interface IUnitOfWork
    {
        IProdutoRepository ProdutoRepository { get; }
        ICategoriaRepository CategoriaRepository { get; }
        void Commit();
        void Dispose();
    }
}
