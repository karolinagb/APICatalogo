using APICatalogo.Controllers;
using APICatalogo.Data;
using APICatalogo.Models;
using APICatalogo.Models.ViewModels.Mappings;
using APICatalogo.Pagination;
using APICatalogo.Transactions;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Xunit;

namespace APICatalogoTestes.Testes
{
    public class CategoriasUnitTestController
    {
        private readonly IUnitOfWork _unitOfWork;

        //Propriedade estática que define trabalha com uma instância do meu contexto:
        public static DbContextOptions<APICatalogoDbContext> dbContextOptions { get; }

        //String de conexão
        public static string connectionString = "server=localhost;userid=root;password=root;database=catalogodb";

        //Construtor estático para inicializar o dbContextOptions
        //É utilizado para inicializar qualquer dado estático
        //É chamado antes de qualquer codigo.
        static CategoriasUnitTestController()
        {
            dbContextOptions = new DbContextOptionsBuilder<APICatalogoDbContext>()
                .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
                .Options;
        }

        public CategoriasUnitTestController()
        {
            var context = new APICatalogoDbContext(dbContextOptions);

            _unitOfWork = new UnitOfWork(context);
            //DBUnitTestsMockInitializer db = new DBUnitTestsMockInitializer();
            //db.Seed(context);
        }

        //Testes unitários ===============================

        //Testar o método GET
        [Fact]
        public void GetCategorias_Return_OkResult()
        {
            //Arrange ==== Fase de preparação
            var controller = new CategoriasController(_unitOfWork);
            var model = new CategoriasParameters();

            //Act ==== Ação / Execução
            var data = controller.Get(model);

            //Assert ==== Verificação do retorno
            //Assert.IsType<List<Categoria>>(data.Value);
        }
    }
}
