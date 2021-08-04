using System.Collections.Generic;

namespace APICatalogo.Models
{
    public class Categoria
    {
        //[Key] //Atributo para chave primária
        public int Id { get; set; }

        //[Required]
        //[MaxLength(80)]
        public string Nome { get; set; }

        //[Required]
        //[MaxLength(300)]
        public string ImagemUrl { get; set; }

        public ICollection<Produto> Produtos { get; set; }
    }
}
