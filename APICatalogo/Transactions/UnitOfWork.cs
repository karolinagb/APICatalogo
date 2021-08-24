using APICatalogo.Data;
using APICatalogo.Repositories;
using APICatalogo.Repositories.Interfaces;
using System.Threading.Tasks;

namespace APICatalogo.Transactions
{
    public class UnitOfWork : IUnitOfWork
    {
        private ProdutoRepository _produtoRepository;
        private CategoriaRepository _categoriaRepository;

        private readonly APICatalogoDbContext _aPICatalogoDbContext;

        public UnitOfWork(APICatalogoDbContext aPICatalogoDbContext)
        {
            _aPICatalogoDbContext = aPICatalogoDbContext;
        }

        public IProdutoRepository ProdutoRepository
        {
            get
            {
                return _produtoRepository = _produtoRepository ?? new ProdutoRepository(_aPICatalogoDbContext);
            }
        }

        public ICategoriaRepository CategoriaRepository
        {
            get
            {
                return _categoriaRepository = _categoriaRepository ?? new CategoriaRepository(_aPICatalogoDbContext);
            }
        }

        public async Task Commit()
        {
            await _aPICatalogoDbContext.SaveChangesAsync();
        }

        public void Dispose()
        {
            _aPICatalogoDbContext.Dispose();
        }
    }
}
