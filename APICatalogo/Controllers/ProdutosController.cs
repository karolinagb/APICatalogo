using ApiCatalogo.Filters;
using APICatalogo.Models;
using APICatalogo.Models.ViewModels;
using APICatalogo.Pagination;
using APICatalogo.Transactions;
using AutoMapper;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace APICatalogo.Controllers
{
    //Vai Aplicar as convenções da API definindo os tipos de retorno específico para cada metodo action
    [ApiConventionType(typeof(DefaultApiConventions))]
    [Produces("application/json")] //Definindo o retorno padrão como json
    [Route("api/[Controller]")] //Para definir a rota de acesso a API. Sem esse atributo nao tem como acessar as APIs. Ele faz um processo de mapeamento das
    //requisições recebidas para a lógica dos métodos actions do controlador. Mapeia para o metodo action correspondente.
    [ApiController] //Habilita mais recursos que facilitam o desenvolvimento
    [EnableCors("PermitirApiRequest")]
    public class ProdutosController : ControllerBase //Herda de controller base para ter recursos de atender requisições HTTP
                                                     //A classe ControllerBase vai incluir diversas funcionalidade para uma API e vai omitir as funcionalidades de suporte as Views
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProdutosController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        //Para ignorar a rota padrão
        //[HttpGet("/primeiro")] // primeiro
        //[HttpGet("primeiro")] //Uma action pode atender dois endpoints
        //[HttpGet("{valor:alpha:length(5)}")] //Parâmetro alfanumérico de tamanho 5
        //public ActionResult<Produto> Get2(string valor)
        //{
        //    var v = valor;
        //    return _aPICatalogoDbContext.Produtos.FirstOrDefault();
        //}

        /// <summary>
        /// Exibe uma relação dos produtos
        /// </summary>
        /// <param name="produtosParameters"></param>
        /// <returns>Retorna uma lista de objetos produto</returns>
        [HttpGet]
        [ServiceFilter(typeof(ApiLoggingFilter))] //Essa anotação resolver a classe de container e temos que utiliza-la
        //porque estamos utilizando a injeção de dependência
        //Retorna todos os produtos
        public async Task<ActionResult<IEnumerable<Produto>>> Get([FromQuery] ProdutosParameters produtosParameters)
        {
            //Entity Framework rastreia a consulta para ver se teve alteração no objeto
            //Isso pode deixar o desempenho ruim, então em ações Get nós desabilitamos isso através do
            //AsNoTracking
            //return await _aPICatalogoDbContext.Produtos.AsNoTracking().ToListAsync();

            var produtos = await _unitOfWork.ProdutoRepository.GetProdutos(produtosParameters);

            var metadata = new
            {
                produtos.TotalCount,
                produtos.PageSize,
                produtos.CurrentPage,
                produtos.TotalPages,
                produtos.HasNext,
                produtos.HasPrevious
            };

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

            return produtos;
        }

        /// <summary>
        /// Obtém um produto pelo seu identificado ID
        /// </summary>
        /// <param name="id">Código do produto</param>
        /// <returns>Um objeto produto</returns>
        //api/produtos/id
        //Name cria uma rota que permite vincular uma resposta Http
        //id:int:min(1) => especifica que o id tem que ser inteiro e no minimo 1
        [HttpGet("{id:int:min(1)}", Name = "ObterProduto")] //Api recebendo dois parametros - Interrogação para dizer que o segundo parâmetro é opcional
        public async Task<ActionResult<Produto>> Get(int id) //O param2 também pode receber um valor padrão que eu definir
        {
            //throw new Exception("Exception ao retornar produto pelo id");

            var produto = await _unitOfWork.ProdutoRepository.GetById(x => x.Id == id);

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
        public async Task<ActionResult> Post([FromBody]ProdutoViewModel produtoViewModel)
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

            //produtoViewModel.DataCadastro = DateTime.Now;

            var produto = _mapper.Map<Produto>(produtoViewModel);
            produto.DataCadastro = DateTime.Now;

            _unitOfWork.ProdutoRepository.Add(produto);
            await _unitOfWork.Commit();

            //Retornar o codigo 201 CreatedAt
            //Quando criamos um produto, temos que retornar a localização de um produto
            return new CreatedAtRouteResult("ObterProduto", new { id = produto.Id}, produto);
            //Acima estou passando id como parametro para a action get e o produto que virá no corpo
        }

        //Atributo aplica automaticamente todo os tipos de retornos possiveis para o metodo PUT
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id,[FromBody] Produto produto)
        {
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}

            if(id != produto.Id)
            {
                return BadRequest();
            }

            var _produto = await _unitOfWork.ProdutoRepository.GetById(x => x.Id == id);

            if (_produto == null)
            {
                return NotFound();
            }

            //Para alterar o estado dessa entidade com Modified
            _unitOfWork.ProdutoRepository.Update(produto);
            await _unitOfWork.Commit();

            return Ok(); //Http200
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Produto>> Delete(int id)
        {
            var produto = await _unitOfWork.ProdutoRepository.GetById(x => x.Id == id);

            if(produto == null)
            {
                return NotFound();
            }

            _unitOfWork.ProdutoRepository.Delete(produto);
            await _unitOfWork.Commit();
            return produto;
        }

        [HttpGet("menorpreco")]
        public async Task<ActionResult<IEnumerable<Produto>>> GetProdutosPrecos()
        {
            return await _unitOfWork.ProdutoRepository.GetProdutoPorPreco();
        }
    }
}
