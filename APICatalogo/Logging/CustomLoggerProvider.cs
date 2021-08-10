using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace APICatalogo.Logging
{
    //Classe responsável por criar as instâncias do log
    public class CustomLoggerProvider : ILoggerProvider
    {
        private readonly CustomLoggerProviderConfiguration _loggerConfig;

        //Instância para armazenar as informações
        private readonly ConcurrentDictionary<string, CustomerLogger> loggers =
            new ConcurrentDictionary<string, CustomerLogger>();

        //Instância da configuração do provider
        public CustomLoggerProvider(CustomLoggerProviderConfiguration loggerConfig)
        {
            _loggerConfig = loggerConfig;
        }


        //Cria uma instância do meu log customizado por nome de categoria e vai armazenar isso em um dicionário
        public ILogger CreateLogger(string categoryName)
        {
            return loggers.GetOrAdd(categoryName, name => new CustomerLogger(name, _loggerConfig));
        }

        public void Dispose()
        {
            //Liberar os recursos
            loggers.Clear();
        }
    }
}
