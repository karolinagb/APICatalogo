﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APICatalogo.Models
{
    public class Produto
    {
        //[Key]
        public int Id { get; set; }

        //[Required(ErrorMessage = "O nome é obrigatór")]
        //[MaxLength(80)]
        public string Nome { get; set; }

        //[Required]
        public string Descricao { get; set; }

        //[Required]
        //[Column(TypeName = "decimal(18,2)")]
        public decimal Preco { get; set; }

        //[Required]
        //[MaxLength(500)]
        public string ImagemUrl { get; set; }

        public float Estoque { get; set; }

        public DateTime DataCadastro { get; set; }

        public Categoria Categoria { get; set; }

        //O lado N guarda a chave
        public int CategoriaId { get; set; }
    }
}
