using ApiCatalogo.Filters;
using APICatalogo.Data;
using APICatalogo.Models;
using APICatalogo.Models.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APICatalogo.Controllers
{
    [Route("api/[Controller]")] //Para definir a rota de acesso a API. Sem esse atributo nao tem como acessar as APIs. Ele faz um processo de mapeamento das
    //requisições recebidas para a lógica dos métodos actions do controlador. Mapeia para o metodo action correspondente.
    [ApiController] //Habilita mais recursos que facilitam o desenvolvimento
    public class ProdutosController : ControllerBase //Herda de controller base para ter recursos de atender requisições HTTP
                                                     //A classe ControllerBase vai incluir diversas funcionalidade para uma API e vai omitir as funcionalidades de suporte as Views
    {
        private readonly APICatalogoDbContext _aPICatalogoDbContext;
        private readonly IMapper _mapper;

        public ProdutosController(APICatalogoDbContext aPICatalogoDbContext, IMapper mapper)
        {
            _aPICatalogoDbContext = aPICatalogoDbContext;
            _mapper = mapper;
        }

        //Para ignorar a rota padrão
        //[HttpGet("/primeiro")] // primeiro
        //[HttpGet("primeiro")] //Uma action pode atender dois endpoints
        [HttpGet("{valor:alpha:length(5)}")] //Parâmetro alfanumérico de tamanho 5
        public ActionResult<Produto> Get2(string valor)
        {
            var v = valor;
            return _aPICatalogoDbContext.Produtos.FirstOrDefault();
        }

        [HttpGet]
        [ServiceFilter(typeof(ApiLoggingFilter))]
        //Retorna todos os produtos
        public async Task<ActionResult<IEnumerable<Produto>>> Get()
        {
            //Entity Framework rastreia a consulta para ver se teve alteração no objeto
            //Isso pode deixar o desempenho ruim, então em ações Get nós debilitamos isso através do
            //AsNoTracking
            return await _aPICatalogoDbContext.Produtos.AsNoTracking().ToListAsync();
        }

        //api/produtos/id/valor
        //Name cria uma rota que permite vincular uma resposta Http
        //id:int:min(1) => especifica que o id tem que ser inteiro e no minimo 1
        [HttpGet("{id:int:min(1)}/{param2=Karol}", Name = "ObterProduto")] //Api recebendo dois parametros - Interrogação para dizer que o segundo parâmetro é opcional
        public async Task<ActionResult<Produto>> Get([FromQuery] int id, [FromQuery]string param2) //O param2 também pode receber um valor padrão que eu definir
        {
            var segundoParametro = param2;

            var produto = await _aPICatalogoDbContext.Produtos.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if(produto == null)
            {
                return NotFound(); //404
            }
            return produto;
        }

        //FromBody = para Asp.Net Core saber que ela tem que obter essas informações de produto
        //do corpo da requisição
        //O ModelBind vincula os parÂmetros do corpo do request com os parâmetros da action post
        [HttpPost]
        public ActionResult Post([FromBody]ProdutoViewModel produtoViewModel)
        {
            //O ModelState é uma propriedade da classe controller que representa uma coleção de pares "nome" "valor"
            //que são submetidos no servidor durante o post. Ele cnontém uma cleção de mensagens de erro para cada valor submetido.
            //Essa propriedade não tem conhecimento do nosso modelo. Ela só tem nomes, valores e erros. Ela serve para armazenar o valor submetido ao servidor
            //e para armazenar os erros.
            //OBS: Desde a versão 2.0 do .NET, para controladores com atributo´[ApiController], essa validação é feita automaticamente.
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}

            produtoViewModel.DataCadastro = DateTime.Now;

            var produto = _mapper.Map<Produto>(produtoViewModel);

            _aPICatalogoDbContext.Produtos.Add(produto);
            _aPICatalogoDbContext.SaveChanges();

            //Retornar o codigo 201 CreatedAt
            //Quando criamos um produto, temos que retornar a localização de um produto
            return new CreatedAtRouteResult("ObterProduto", new { id = produto.Id}, produto);
            //Acima estou passando id como parametro para a action get e o produto que virá no corpo
        }

        [HttpPut("{id}")]
        public ActionResult Put(int id,[FromBody] Produto produto)
        {
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}

            if(id != produto.Id)
            {
                return BadRequest();
            }

            //Para alterar o estado dessa entidade com Modified
            _aPICatalogoDbContext.Entry(produto).State = EntityState.Modified;
            _aPICatalogoDbContext.SaveChanges();

            return Ok(); //Http200
        }

        [HttpDelete("{id}")]
        public ActionResult<Produto> Delete(int id)
        {
            var produto = _aPICatalogoDbContext.Produtos.FirstOrDefault(x => x.Id == id);

            if(produto == null)
            {
                return NotFound();
            }

            _aPICatalogoDbContext.Produtos.Remove(produto);
            _aPICatalogoDbContext.SaveChanges();
            return produto;
        }
    }
}
