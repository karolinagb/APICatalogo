﻿using APICatalogo.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace APICatalogo.Repositories.Interfaces
{
    public interface IProdutoRepository : IRepository<Produto>
    {
        //Não preciso escrever novamente os método que tem em IRepository exatamente porque ela está herdando
        //Posso colocar métodos adicionais
        public Task<List<Produto>> GetProdutoPorPreco();
    }
}
