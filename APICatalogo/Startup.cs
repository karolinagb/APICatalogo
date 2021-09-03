using ApiCatalogo.Filters;
using APICatalogo.Data;
using APICatalogo.Logging;
using APICatalogo.Models.ViewModels.Mappings;
using APICatalogo.Services;
using APICatalogo.Transactions;
using AutoMapper;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using System.Reflection;
using System.Text;

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

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<APICatalogoDbContext>()
                .AddDefaultTokenProviders();

            //JWT
            //adiciona o manipulador de autenticacao e define o 
            //esquema de autenticacao usado : Bearer
            //valida o emissor, a audiencia e a chave
            //usando a chave secreta valida a assinatura
            services.AddAuthentication(
                JwtBearerDefaults.AuthenticationScheme).
                AddJwtBearer(options =>
                 options.TokenValidationParameters = new TokenValidationParameters
                 {
                     ValidateIssuer = true,
                     ValidateAudience = true,
                     ValidateLifetime = true,
                     ValidAudience = Configuration["TokenConfiguration:Audience"],
                     ValidIssuer = Configuration["TokenConfiguration:Issuer"],
                     ValidateIssuerSigningKey = true,
                     IssuerSigningKey = new SymmetricSecurityKey(
                         Encoding.UTF8.GetBytes(Configuration["TokenConfiguration:key"]))
                 });

            //A inst�ncia do servi�o vai ser criada cada vez que ela for solicitada
            services.AddTransient<IMeuServico, MeuServico>();
            //AddScoped = a inst�ncia vai ser criada uma vez s� para cada requisi��o
            //AddSingleton = a inst�ncia � criada uma vez em toda a aplica��o

            services.AddTransient<IUnitOfWork, UnitOfWork>();

            //Incluindo o servi�o cors POR ATRIBUTO
            services.AddCors(options =>
            {
                options.AddPolicy("PermitirApiRequest",
                    builder =>
                    builder.WithOrigins("http://www.apirequest.io")
                                .WithMethods("GET")
                    );
            });

            /*Para resolver o problema de refer�ncia c�clica do Json (categorias <-> produtos): 
             * Instale o pacote = Microsoft.AspNetCore.Mvc.NewtonsoftJson
             Adicione o c�digo abaixo:*/
            services.AddControllers()
                .AddNewtonsoftJson(x => x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "APICatalogo",
                    Version = "v1",
                    Description = "Cat�logo de produtos e categorias",
                    TermsOfService = new Uri("https://www.karolina.com/terms"),
                    Contact = new OpenApiContact
                    {
                        Name = "Karolina",
                        Email = "karolina@exemplo.com",
                        Url = new Uri("https://www.linkedin.com/feed/")
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Licen�a da Karolina",
                        Url = new Uri("https://www.karolina.com/license2")
                    }
                });

                //vai gerar um arquixo xml a partir do executavel da aplica��o
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                //Caminho onde o arquivo acima vai ser armazenado
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                //Para ler e injetar os comentarios xml
                c.IncludeXmlComments(xmlPath);
            });

            services.AddApiVersioning(options =>
            {
                //Vai assumir a vers�o padr�o quando nenhuma for informada
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0); //Vers�o padr�o
                //Adiciona no header do responde se a vers�o � compat�vel
                options.ReportApiVersions = true;
                //Suporte a passar versao da api no cabe�alho do request atraves da string definida
                options.ApiVersionReader = new HeaderApiVersionReader("api-version");
            });

            services.AddVersionedApiExplorer(
               options =>
               {
                   options.GroupNameFormat = "'v'VVV";
                   options.SubstituteApiVersionInUrl = true;
               });

            //Adicionando o Fluent Validation ao pipeline
            services.AddMvc().AddFluentValidation(x => x.RegisterValidatorsFromAssemblyContaining<Startup>());

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile());
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

            //Pol�tica Cors definida via Middleware
            //app.UseCors(option => option
            //    .WithOrigins("http://www.apirequest.io")
            //    .WithMethods("GET"));

            app.UseCors();

            //Swagger
            app.UseSwagger();

            //SwaggerUI
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "APICatalogo");
            });

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
