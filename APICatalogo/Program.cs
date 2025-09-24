using APICatalogo.Domain.Entities;
using APICatalogo.DTOs.Mappings;
using APICatalogo.Filters;
using APICatalogo.Infrastructure;
using APICatalogo.Infrastructure.Repositories;
using APICatalogo.Services;
using APICatalogo.Shared.Extensions;
using APICatalogo.Transformer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);
//buscando no appsettings a secret key
var secretKey = builder.Configuration["JWT:SecretKey"] ?? throw new ArgumentException("Invalid secret key");

//ReferenceHandler ignorando os ciclos do Json por ter Referencia de Produto em Categoria e vice-versa.
builder.Services.AddControllers(options =>
{
    options.Filters.Add(typeof(ApiExceptionFilter));        //Adicionando Filtro de exceção Global
})
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
}).AddNewtonsoftJson();

//Adicionando Cors
builder.Services.AddCors(options => options.AddPolicy(name: "OrigensComAcessoPermitido", policy =>   
{
    policy.WithOrigins("https://localhost:7022")     //Definindo o acesso a origem
          .WithMethods("GET", "POST")               //Métodos permitidos
          .AllowAnyHeader();                        //Permitindo todos os cabeçalhos
}));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "http://localhost:5127",
            ValidAudience = "https://localhost:7125",
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes("Esta_e_uma_chave_secreta_longa_e_segura_com_no_minimo_32_caracteres_e_numeros_12345"))
        };
    });
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));      //Exige que o usuario tenha a role de Admin
    options.AddPolicy("SuperAdminOnly", policy => policy.RequireRole("Admin").RequireClaim("id", "samuquinha")); //Exige que o usuario tenha além da role, a claim expecifica
    options.AddPolicy("UserOnly", policy => policy.RequireRole("User"));
    options.AddPolicy("ExclusiveOnly", policy => policy.RequireAssertion(context =>
                                                                         context.User.HasClaim(claim =>
                                                                         claim.Type == "id" &&
                                                                         claim.Value == "samuquinha") ||
                                                                         context.User.IsInRole("SuperAdmin")));
    //Com RequireAssertion temos uma requisição mais personalizada
});
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
});
builder.Services.AddSwaggerGen();

/*builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter(policyName: "fixedWindow", optionsWindow =>
    {
        optionsWindow.PermitLimit = 1;      //Permite 1 requisição
        optionsWindow.Window = TimeSpan.FromSeconds(5);     //A cada 5 segundos
        optionsWindow.QueueLimit = 2;       //Se o limite for atingido aceita 2 filas
        optionsWindow.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;  //Ordem da fila do mais antigo primeiro
    });
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});*/
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpcontext =>       //Criando um limite de taxa particionado
                            RateLimitPartition.GetFixedWindowLimiter(partitionKey: httpcontext.User.Identity?.Name ??       //Obtendo partitionKey
                                                                                   httpcontext.Request.Headers.Host.ToString(),     //Caso o usuario esteja disponivel usa ele caso não usa o Host
                                                                                   factory: partition => new FixedWindowRateLimiterOptions          //Configurando
                                                                                   {
                                                                                       AutoReplenishment = true,        //Permite reabastecimento de limite
                                                                                       PermitLimit = 5,                 //Permitindo até 5 requisições
                                                                                       QueueLimit = 0,                  //Não permite Fila
                                                                                       Window = TimeSpan.FromSeconds(10)    //10 segundos por janela
                                                                                   }));
});

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()                  //chamada a identity para geração de tabelas
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();                

string mySqlConnection = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options => options.UseMySql(mySqlConnection, ServerVersion.AutoDetect(mySqlConnection)));
builder.Services.AddScoped<ApiLoggingFilter>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddAutoMapper(typeof(ProductDTOMappingProfile), typeof(CategoryDTOMappingProfile));

var app = builder.Build();

// Configure the HTTP request pipeline. onde é configurado os middlewares
if (app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.MapOpenApi();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "AuthJwt v1"));                    //referencia ao middleware
    app.ConfigureExceptionHandler();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseRateLimiter();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
