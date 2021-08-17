using ApiCatalogo.Filters;
using APICatalogo.Data;
using APICatalogo.Logging;
using APICatalogo.Models;
using APICatalogo.Models.ViewModels;
using APICatalogo.Repositories;
using APICatalogo.Repositories.Interfaces;
using APICatalogo.Services;
using APICatalogo.Transactions;
using AutoMapper;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace APICatalogo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string connectionString = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<APICatalogoDbContext>(options =>
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

            //A inst�ncia do servi�o vai ser criada cada vez que ela for solicitada
            services.AddTransient<IMeuServico, MeuServico>();
            //AddScoped = a inst�ncia vai ser criada uma vez s� para cada requisi��o
            //AddSingleton = a inst�ncia � criada uma vez em toda a aplica��o

            services.AddTransient<IUnitOfWork, UnitOfWork>();

            /*Para resolver o problema de refer�ncia c�clica do Json (categorias <-> produtos): 
             * Instale o pacote = Microsoft.AspNetCore.Mvc.NewtonsoftJson
             Adicione o c�digo abaixo:*/
            services.AddControllers()
                .AddNewtonsoftJson(x => x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "APICatalogo", Version = "v1" });
            });

            //Adicionando o Fluent Validation ao pipeline
            services.AddMvc().AddFluentValidation(x => x.RegisterValidatorsFromAssemblyContaining<Startup>());

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ProdutoViewModel, Produto>()
                .IgnoreAllPropertiesWithAnInaccessibleSetter()
                    .ForPath(x => x.DataCadastro, option => option.Ignore());
            });

            IMapper mapper = config.CreateMapper();
            services.AddSingleton(mapper);

            services.AddScoped<ApiLoggingFilter>(); //COnfigurando o servi�o de filtro personalizado
            //AddScoped = uma inst�ncia para cada requisi��o
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        //ApplicationBuilder � uma classe que fornece um mecanismo para configurar o pipeline da requisi��o
        //IWebHostEnvironment = Permite definir em qual ambiente estamos trabalhando
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "APICatalogo v1"));
            }

            //Para ativar o log
            loggerFactory.AddProvider(new CustomLoggerProvider(new CustomLoggerProviderConfiguration
            {
                LogLevel = LogLevel.Information
            }));

            //adiciona o middleware de tratamento de erros
            //app.ConfigureExceptionHandler;

            //Adiciona o middleware para redirecionar para https
            app.UseHttpsRedirection();

            //adiciona o middleware de roteamento
            app.UseRouting();

            //Adiciona o middleware de autentica��o ao pipeline
            app.UseAuthentication();

            //adiciona o middleware que habilita a autoriza��o
            app.UseAuthorization();

            //Adiciona o middleware que executa o endpoint
            //do request atual
            app.UseEndpoints(endpoints =>
            {
                //Adiciona os endpoints para as actions dos controladores sem especificar rotas
                endpoints.MapControllers();
            });

            //Al�m de habilitar um middleware atrav�s do Use (app.UseEndpoints), tamb�m podemos incluir middlewares
            //customizados atrav�s do Run (app.Run) . O Middlewares definidos com Run s�o middlewares finais, ap�s eles,
            //nenhum outro middleware ser� chamado.

            //app.Run(async (context) =>
            //{
            //    await context.Response.WriteAsync("Middleware final");
            //});
        }
    }
}
