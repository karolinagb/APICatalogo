using System.Collections.Generic;

namespace APICatalogo.Models
{
    public class Categoria
    {
        public int Id { get; set; }

        public string Nome { get; set; }

        public string ImagemUrl { get; set; }

        public ICollection<Produto> Produtos { get; set; }
    }
}
