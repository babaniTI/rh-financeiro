using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ouroeprata.comprarapida.Data.UnitOfWorks;
using rh.financeiro.Data.Context.Default;
using rh.financeiro.Data.Repository;
using rh.financeiro.Domain.Interfaces.Repository;
using rh.financeiro.Domain.Interfaces.Service.Auth;
using rh.financeiro.Domain.Interfaces.Service.CategoriaFinanceiras;
using rh.financeiro.Domain.Interfaces.Service.Conciliacao;
using rh.financeiro.Domain.Interfaces.Service.ContaFinanceiras;
using rh.financeiro.Domain.Interfaces.Service.DocumentoFiscal;
using rh.financeiro.Domain.Interfaces.Service.Jobs;
using rh.financeiro.Domain.Interfaces.Service.MovimentacaoBancaria;
using rh.financeiro.Domain.Interfaces.Service.Nfe;
using rh.financeiro.Domain.Interfaces.Service.Participantes;
using rh.financeiro.Domain.Interfaces.Service.Titulo;
using rh.financeiro.Domain.Interfaces.UnitOfWorks;
using rh.financeiro.Services.Services.Auth;
using rh.financeiro.Services.Services.CategoriaFinanceiras;
using rh.financeiro.Services.Services.Conciliacao;
using rh.financeiro.Services.Services.ContasFinanceiras;
using rh.financeiro.Services.Services.DocumentoFiscais;
using rh.financeiro.Services.Services.Jobs;
using rh.financeiro.Services.Services.MovimentacaoBancaria;
using rh.financeiro.Services.Services.Nfe;
using rh.financeiro.Services.Services.Participantes;
using rh.financeiro.Services.Services.Titulos;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
string environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Default";
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{environmentName}.json", optional: true);

// Adicionando CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// Swagger + JWT
builder.Services.AddSwaggerGen(c =>
{
    // Adiciona a definição de segurança (Bearer)
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Digite 'Bearer' [espaço] e depois o token JWT.\n\nExemplo: \"Bearer 12345abcdef\""
    });

    // Exige o uso da segurança em todas as operações
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["JWT:SecretKey"]))
        };
    });

builder.Services.AddAuthorization();


builder.Services.AddDbContext<DefaultContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
});




builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IParticipantesService, ParticipantesService>();
builder.Services.AddScoped<IJobsService, JobsService>();
builder.Services.AddScoped<ICategoriaFinanceiraService, CategoriaFinanceiraService>();
builder.Services.AddScoped<IContasFinanceirasService, ContasFinanceirasService>();
builder.Services.AddScoped<INfeService, NfeService>();
builder.Services.AddScoped<IDocumentoFiscalService, DocumentoFiscalService>();
builder.Services.AddScoped<ITituloService, TituloService>();
builder.Services.AddScoped<IMovimentacaoBancariaService, MovimentacaoBancariaService>();
builder.Services.AddScoped<IConciliacaoService, ConciliacaoService>();



var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseCors("AllowAll");


app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();