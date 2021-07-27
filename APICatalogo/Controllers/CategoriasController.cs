using APICatalogo.Data;
using APICatalogo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace APICatalogo.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class CategoriasController : ControllerBase
    {
        private readonly APICatalogoDbContext _aPICatalogoDbContext;

        public CategoriasController(APICatalogoDbContext aPICatalogoDbContext)
        {
            _aPICatalogoDbContext = aPICatalogoDbContext;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Categoria>> Get()
        {
            return _aPICatalogoDbContext.Categorias.AsNoTracking().ToList();
        }

        [HttpGet("{id}", Name = "ObterCategoria")]
        public ActionResult<Categoria> Get(int id)
        {
            var categoria = _aPICatalogoDbContext.Categorias.AsNoTracking().FirstOrDefault(x => x.Id == id);

            if(categoria == null)
            {
                return NotFound(); //404
            }

            return categoria;
        }

        [HttpPost]
        public ActionResult Post([FromBody] Categoria categoria)
        {
            _aPICatalogoDbContext.Categorias.Add(categoria);
            _aPICatalogoDbContext.SaveChanges();

            return new CreatedAtRouteResult("ObterCategoria", new { id = categoria.Id }, categoria);
        }

        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] Categoria categoria)
        {
            if(id != categoria.Id)
            {
                return BadRequest();
            }

            _aPICatalogoDbContext.Entry(categoria).State = EntityState.Modified;
            _aPICatalogoDbContext.SaveChanges();

            return Ok(); //Http200
        }

        [HttpDelete("{id}")]
        public ActionResult<Categoria> Delete(int id)
        {
            var categoria = _aPICatalogoDbContext.Categorias.FirstOrDefault(x => x.Id == id);

            if (categoria == null)
            {
                return NotFound(); //404
            }

            _aPICatalogoDbContext.Categorias.Remove(categoria);
            _aPICatalogoDbContext.SaveChanges();
            return categoria;
        }
    }
}
