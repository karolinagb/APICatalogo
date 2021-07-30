using System;

namespace APICatalogo.Services
{
    public class MeuServico : IMeuServico
    {
        public string Saudação(string nome)
        {
            return $"Bem-vindo, {nome} \n\n{DateTime.Now}";
        }
    }
}
