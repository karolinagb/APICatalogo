using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace APICatalogo.Logging
{
    //Essa classe vai criar as instâncias do meu log por nome de categoria e provedor
    public class CustomerLogger : ILogger
    {
        private  readonly string _loggerName;
        private readonly CustomLoggerProviderConfiguration _loggerConfig;

        public CustomerLogger(string loggerName, CustomLoggerProviderConfiguration loggerConfig)
        {
            _loggerName = loggerName;
            _loggerConfig = loggerConfig;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel == _loggerConfig.LogLevel;
        }

        //O Log está sendo feito na definição desse método
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, 
            Func<TState, Exception, string> formatter)
        {
            //Logando o nivel de log, o evento e formata as mensagens
            string mensagem = $"{logLevel.ToString()}: {eventId.Id} - {formatter(state, exception)}";

            EscreverTextoNoArquivo(mensagem);
        }

        //Grava no arquivo texto
        private void EscreverTextoNoArquivo(string mensagem)
        {
            string caminhoArquivoLog = @"c:\projetos\APICatalogo\log\Karolina_Log.txt"; //Ele vai criar o arquivo com esse nome

            //Verifica se o caminho existe, se não existir ele vai lançar uma exception
            using (StreamWriter streamWriter = new StreamWriter(caminhoArquivoLog, true))
            {
                try
                {
                    streamWriter.WriteLine(mensagem); //Escrevendo a mensagem no arquivo texto
                    streamWriter.Close();
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
    }
}
