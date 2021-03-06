using APICatalogo.Models;
using APICatalogo.Pagination;
using APICatalogo.Services;
using APICatalogo.Transactions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace APICatalogo.Controllers
{
    [Produces("application/json")]
    [Authorize(AuthenticationSchemes = "Bearer")] //Definindo o authorize e o esquema utilizado para
    //autorização
    [Route("api/[Controller]")]
    [ApiController]
    public class CategoriasController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        //private readonly ILogger _logger;

        public CategoriasController(IUnitOfWork unitOfWork/*, ILogger<CategoriasController> logger*/)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("saudacao/{nome}")]
        public ActionResult<string> GetSaudacao([FromServices] IMeuServico meuServico, string nome)
        {
            //_logger.LogInformation("================ GET api/categorias/saudacao/nome ====================");
            return meuServico.Saudação(nome);
        }

        //api/categorias
        [ProducesResponseType(typeof(List<Categoria>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Categoria>>> Get([FromQuery] CategoriasParameters categoriasParameters)
        {
            try
            {
                //_logger.LogInformation("================ GET api/categorias ====================");
                //throw new Exception(); Para testar o tratamento do erro 500
                var categorias = await _unitOfWork.CategoriaRepository.GetCategorias(categoriasParameters);

                var metadata = new
                {
                    categorias.TotalCount,
                    categorias.PageSize,
                    categorias.CurrentPage,
                    categorias.TotalPages,
                    categorias.HasNext,
                    categorias.HasPrevious
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                return categorias;
            }
            catch (Exception) //Geralmente erros de exceção estão ligados a erros de servidor e acesso a bd
            {
                //Usar a classe StatusCode para tratamento dos erros
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao tentar obter as categorias do banco de dados");
            }
            
        }

        /// <summary>
        /// Obter uma categoria pelo seu Id
        /// </summary>
        /// <param name="id">Código da categoria</param>
        /// <returns>Objeto categoria</returns>
        //api/categorias/id
        [ProducesResponseType(typeof(Categoria), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("{id}", Name = "ObterCategoria")]
        public async Task<ActionResult<Categoria>> Get(int? id)
        {
            try
            {
                //throw new Exception();
                if(id == null)
                {
                    return BadRequest("Id não informado");
                }

                var categoria = await _unitOfWork.CategoriaRepository.GetById(x => x.Id == id);

                //_logger.LogInformation($"================ GET api/categorias/id = {id} ====================");

                if (categoria == null)
                {
                    //_logger.LogInformation($"================ GET api/categorias/id = {id} NOT FOUND ====================");
                    return NotFound($"A categoria com Id = {id} não foi encontrada"); //404
                }

                return categoria;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao tentar obter a categoria do banco de dados de ID = {id}");
            }


        }

        /// <summary>
        /// Inclui uma nova categoria
        /// </summary>
        /// <remarks>
        /// Exemplo de request:
        /// 
        /// POST api/categorias
        /// {
        ///     "id" : 1,
        ///     "nome" : "categoria1",
        ///     "imagemurl": "https://www.krol.com/imagens"
        /// }
        /// </remarks>
        /// <param name="categoria">Objeto categoria</param>
        /// <returns>O objeto categoria incluído</returns>
        /// <remarks>Retorna um objeto categoria incluído</remarks>

        //api/produtos
        [ProducesResponseType(typeof(Categoria), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost] //Os métodos actions atendem as requisições para os respectivos verbos Http's
        public async Task<ActionResult> Post([FromBody] Categoria categoria)
        {
            try
            {
                _unitOfWork.CategoriaRepository.Add(categoria);
                await _unitOfWork.Commit();

                return new CreatedAtRouteResult("ObterCategoria", new { id = categoria.Id }, categoria);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao tentar criar uma nova categoria");
            }

            
        }

        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Put))]
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] Categoria categoria)
        {
            try
            {
                if (id != categoria.Id)
                {
                    return BadRequest($"Id não correspondente. Id da URI = {id} e Id da categoria passada = {categoria.Id}"); //400
                }

                _unitOfWork.CategoriaRepository.Update(categoria);
                await _unitOfWork.Commit();

                return Ok($"A categoria com Id = {id} foi atualizada com sucesso"); //Http200
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao tentar alterar a categoria com Id = {id}");
            }
            
        }

        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpDelete("{id}")]
        public async Task<ActionResult<Categoria>> Delete(int id)
        {
            try
            {
                var categoria = await _unitOfWork.CategoriaRepository.GetById(x => x.Id == id);

                if (categoria == null)
                {
                    return NotFound($"Categoria com Id = {id} não encontrada"); //404
                }

                _unitOfWork.CategoriaRepository.Delete(categoria);
                await _unitOfWork.Commit();
                return categoria;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao tentar deletar a categoria com Id = {id}");
            }

            
        }

        [HttpGet("produtos")]
        public async Task<ActionResult<IEnumerable<Categoria>>> GetCategoriasProdutos()
        {
            return await _unitOfWork.CategoriaRepository.GetCategoriasProdutos();
        }
    }
}
