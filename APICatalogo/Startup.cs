using APICatalogo.Data;
using APICatalogo.Models;
using APICatalogo.Models.ViewModels;
using APICatalogo.Services;
using AutoMapper;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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

            //A instância do serviço vai ser criada cada vez que ela for solicitada
            services.AddTransient<IMeuServico, MeuServico>();
            //AddScoped = a instância vai ser criada uma vez só para cada requisição
            //AddSingleton = a instância é criada uma vez em toda a aplicação

            /*Para resolver o problema de referência cíclica do Json (categorias <-> produtos): 
             * Instale o pacote = Microsoft.AspNetCore.Mvc.NewtonsoftJson
             Adicione o código abaixo:*/
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
                cfg.CreateMap<ProdutoViewModel, Produto>();
            });

            IMapper mapper = config.CreateMapper();
            services.AddSingleton(mapper);  
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "APICatalogo v1"));
            }

            //Adiciona o middleware para redirecionar para https
            app.UseHttpsRedirection();

            //adiciona o middleware de roteamento
            app.UseRouting();

            //adiciona o middleware que habilita a autorização
            app.UseAuthorization();

            //Adiciona o middleware que executa o endpoint
            //do request atual
            app.UseEndpoints(endpoints =>
            {
                //Adiciona os endpoints para as actions dos controladores sem especificar rotas
                endpoints.MapControllers();
            });
        }
    }
}
