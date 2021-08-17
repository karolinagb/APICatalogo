using APICatalogo.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace APICatalogo.Extensions
{
    public static class ApiExceptionMiddlewareExtensions
    {
        //Método para definir o tratamento de erros
        public static void ConfigureExceptionHandler(this IApplicationBuilder app) //Recebe instância da aplicação
        {//Tem que colocar o this para dizer que é um método de extensão
            //Para usar o middleware de tratamento de exceções
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError; //Obtendo o StatusCode
                    context.Response.ContentType = "application/json"; //Definindo o tipo de retorno que eu vou usar

                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>(); //Obtendo informações e detalhes do erro
                    if (contextFeature != null)
                    {
                        await context.Response.WriteAsync(new ErrorDetails()
                        {
                            //No retorno da mensagem irá:
                            StatusCode = context.Response.StatusCode,
                            Message = contextFeature.Error.Message,
                            Trace = contextFeature.Error.StackTrace  //Pilha de erros
                        }.ToString());
                    }
                });
            });
        }
    }
}
