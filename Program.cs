using Controle_De_estoqueAPI.Constantes;
using Controle_De_estoqueAPI.Data;
using Controle_De_estoqueAPI.Middleware;
using Controle_De_estoqueAPI.Models;
using Controle_De_estoqueAPI.Service.Implementações;
using Controle_De_estoqueAPI.Service.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<ControleEstoqueDbContexto>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Registro dos serviços
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICategoriaService, CategoriaService>();
builder.Services.AddScoped<IFornecedorService, FornecedorService>();
builder.Services.AddScoped<IGerarHashService, GerarHashService>();
builder.Services.AddScoped<IMovimentacaoService, MovimentacaoEstoqueService>();
builder.Services.AddScoped<IPerfilService, PerfilService>();
builder.Services.AddScoped<IProdutoService, ProdutoService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IVendaService, VendaService>();

// ===================== Autenticação JWT =====================
var jwtKey = builder.Configuration["Jwt:Key"];

if (string.IsNullOrWhiteSpace(jwtKey) || jwtKey.Length < 32)
    throw new InvalidOperationException(
        "Jwt:Key não configurada ou muito curta (mínimo 32 caracteres). " +
        "Use dotnet user-secrets ou variável de ambiente.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };

        options.Events = new JwtBearerEvents
        {
            // Revalida o usuário a cada requisição: um usuário desativado, removido
            // ou com o perfil alterado perde o acesso imediatamente, sem esperar o token expirar.
            // Custo: 1 consulta leve por requisição autenticada.
            OnTokenValidated = async context =>
            {
                var idTexto = context.Principal?.FindFirstValue(ClaimTypes.NameIdentifier);
                var perfilNoToken = context.Principal?.FindFirstValue(ClaimTypes.Role);

                if (!int.TryParse(idTexto, out var usuarioId))
                {
                    context.Fail("Token inválido.");
                    return;
                }

                var db = context.HttpContext.RequestServices.GetRequiredService<ControleEstoqueDbContexto>();

                var usuario = await db.Usuarios
                    .AsNoTracking()
                    .Where(u => u.Id == usuarioId)
                    .Select(u => new { u.Ativo, Perfil = u.Perfil.PerfilNome })
                    .FirstOrDefaultAsync();

                if (usuario is null || !usuario.Ativo || usuario.Perfil != perfilNoToken)
                    context.Fail("Usuário inativo, removido ou com perfil alterado.");
            }
        };
    });

// Autorização (necessário para o [Authorize(Roles = "...")] funcionar)
builder.Services.AddAuthorization();

// ===================== Rate limiting (força bruta no login) =====================
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    // 5 tentativas por minuto, por IP. Atrás de proxy/load balancer, configure
    // ForwardedHeaders para que RemoteIpAddress seja o IP real do cliente.
    options.AddPolicy("login", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "desconhecido",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0
            }));
});

builder.Services.AddEndpointsApiExplorer();

const string schemeId = "Bearer";

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition(schemeId, new OpenApiSecurityScheme
    {
        Description = "Digite apenas o token JWT (sem a palavra Bearer)",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(document =>
    {
        return new OpenApiSecurityRequirement
        {
            { new OpenApiSecuritySchemeReference(schemeId, document), new List<string>() }
        };
    });
});

var app = builder.Build();

// ===================== MIGRATIONS + SEED DO GERENTE INICIAL =====================
// Migrations automáticas: sempre em Development; em outros ambientes só se
// Database:MigrateOnStartup = true. Em produção, prefira aplicar as migrations
// como etapa separada do deploy (dotnet ef database update / script SQL).
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ControleEstoqueDbContexto>();
    var hashService = scope.ServiceProvider.GetRequiredService<IGerarHashService>();

    var migrarNaInicializacao = app.Environment.IsDevelopment()
        || builder.Configuration.GetValue<bool>("Database:MigrateOnStartup");

    if (migrarNaInicializacao)
        db.Database.Migrate();

    // Garante pelo menos 1 usuário Gerente em uma instalação nova (banco sem usuários).
    if (!db.Usuarios.Any())
    {
        var perfilGerente = db.Perfis.FirstOrDefault(p => p.PerfilNome == PerfilConst.Gerente);

        if (perfilGerente == null)
        {
            perfilGerente = new Perfil { PerfilNome = PerfilConst.Gerente };
            db.Perfis.Add(perfilGerente);
            db.SaveChanges();
        }

        // Sem senha fixa no código: se SeedGerente:SenhaInicial não estiver configurada,
        // gera uma senha aleatória e a exibe UMA vez no console.
        var senhaInicial = builder.Configuration["SeedGerente:SenhaInicial"];
        var senhaFoiGerada = false;

        if (string.IsNullOrWhiteSpace(senhaInicial))
        {
            senhaInicial = GerarSenhaAleatoria();
            senhaFoiGerada = true;
        }

        var gerenteInicial = new Usuario
        {
            NomeUsuario = "Administrador",
            EmailUsuario = "admin@loja.com",
            Login = "admin",
            SenhaHash = hashService.GerarHash(senhaInicial),
            Perfil = perfilGerente,
            Ativo = true,
            DataCriacao = DateTime.UtcNow
        };

        db.Usuarios.Add(gerenteInicial);
        db.SaveChanges();

        Console.WriteLine("Usuário Gerente inicial criado (login: admin).");
        if (senhaFoiGerada)
            Console.WriteLine($"Senha gerada (exibida apenas agora, troque-a após o primeiro login): {senhaInicial}");
    }
}
// =====================================================================

// O tratamento de erros vem primeiro para capturar exceções de todo o restante do pipeline.
app.UseMiddleware<TratamentoDeErrosMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRateLimiter();

app.UseAuthentication(); // SEMPRE antes de UseAuthorization
app.UseAuthorization();

app.MapControllers();

app.Run();

static string GerarSenhaAleatoria()
    => Convert.ToBase64String(RandomNumberGenerator.GetBytes(12)); // 16 caracteres