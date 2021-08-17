using APICatalogo.Data;
using APICatalogo.Models;
using APICatalogo.Services;
using APICatalogo.Transactions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APICatalogo.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class CategoriasController : ControllerBase
    {
        private readonly APICatalogoDbContext _aPICatalogoDbContext;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public CategoriasController(APICatalogoDbContext aPICatalogoDbContext, IUnitOfWork unitOfWork, ILogger<CategoriasController> logger)
        {
            _aPICatalogoDbContext = aPICatalogoDbContext;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [HttpGet("saudacao/{nome}")]
        public ActionResult<string> GetSaudacao([FromServices] IMeuServico meuServico, string nome)
        {
            _logger.LogInformation("================ GET api/categorias/saudacao/nome ====================");
            return meuServico.Saudação(nome);
        }

        //api/categorias
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Categoria>>> Get()
        {
            try
            {
                _logger.LogInformation("================ GET api/categorias ====================");
                //throw new Exception(); Para testar o tratamento do erro 500
                return await _unitOfWork.CategoriaRepository.Get();
            }
            catch (Exception) //Geralmente erros de exceção estão ligados a erros de servidor e acesso a bd
            {
                //Usar a classe StatusCode para tratamento dos erros
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao tentar obter as categorias do banco de dados");
            }
            
        }

        //api/categorias/id
        [HttpGet("{id}", Name = "ObterCategoria")]
        public ActionResult<Categoria> Get(int? id)
        {
            try
            {
                //throw new Exception();
                if(id == null)
                {
                    return BadRequest("Id não informado");
                }

                var categoria = _unitOfWork.CategoriaRepository.GetById(x => x.Id == id);

                _logger.LogInformation($"================ GET api/categorias/id = {id} ====================");

                if (categoria == null)
                {
                    _logger.LogInformation($"================ GET api/categorias/id = {id} NOT FOUND ====================");
                    return NotFound($"A categoria com Id = {id} não foi encontrada"); //404
                }

                return categoria;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao tentar obter a categoria do banco de dados de ID = {id}");
            }


        }

        //api/produtos
        [HttpPost] //Os métodos actions atendem as requisições para os respectivos verbos Http's
        public ActionResult Post([FromBody] Categoria categoria)
        {
            try
            {
                _unitOfWork.CategoriaRepository.Add(categoria);
                _unitOfWork.Commit();

                return new CreatedAtRouteResult("ObterCategoria", new { id = categoria.Id }, categoria);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao tentar criar uma nova categoria");
            }

            
        }

        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] Categoria categoria)
        {
            try
            {
                if (id != categoria.Id)
                {
                    return BadRequest($"Id não correspondente. Id da URI = {id} e Id da categoria passada = {categoria.Id}"); //400
                }

                _unitOfWork.CategoriaRepository.Update(categoria);
                _unitOfWork.Commit();

                return Ok($"A categoria com Id = {id} foi atualizada com sucesso"); //Http200
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao tentar alterar a categoria com Id = {id}");
            }
            
        }

        [HttpDelete("{id}")]
        public ActionResult<Categoria> Delete(int id)
        {
            try
            {
                var categoria = _aPICatalogoDbContext.Categorias.FirstOrDefault(x => x.Id == id);

                if (categoria == null)
                {
                    return NotFound($"Categoria com Id = {id} não encontrada"); //404
                }

                _unitOfWork.CategoriaRepository.Delete(categoria);
                _unitOfWork.Commit();
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
