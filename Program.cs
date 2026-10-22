using Controle_De_estoqueAPI.Data;
using Controle_De_estoqueAPI.Models;
using Controle_De_estoqueAPI.Service.Implementações;
using Controle_De_estoqueAPI.Service.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;
using Controle_De_estoqueAPI.Constantes;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi


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

// Autenticação JWT
// Autenticação JWT
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
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey))
        };
    });

// Autorização (necessário para o [Authorize(Roles = "...")] funcionar)
builder.Services.AddAuthorization();

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

// ===================== SEED DO GERENTE INICIAL =====================
// Garante que o sistema sempre tenha pelo menos 1 usuário Gerente no banco,
// mesmo em uma instalação totalmente nova (banco vazio). Roda uma única vez
// por inicialização, direto no banco, sem passar pela API — por isso não
// exige token nem esbarra em nenhum [Authorize].
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ControleEstoqueDbContexto>();
    var hashService = scope.ServiceProvider.GetRequiredService<IGerarHashService>();


    db.Database.Migrate(); // aplica migrations pendentes automaticamente

    if (!db.Usuarios.Any())
    {
        // Busca o Perfil "Gerente" existente, ou cria se a tabela de perfis também estiver vazia
        var perfilGerente = db.Perfis.FirstOrDefault(p => p.PerfilNome == PerfilConst.Gerente);

        if (perfilGerente == null)
        {
            perfilGerente = new Perfil { PerfilNome = PerfilConst.Gerente };
            db.Perfis.Add(perfilGerente);
            db.SaveChanges(); // salva o Perfil antes de vincular ao Usuario
        }

        var senhaInicial = builder.Configuration["SeedGerente:SenhaInicial"] ?? "SenhaTemporaria123!";

        var gerenteInicial = new Usuario
        {
            NomeUsuario = "Administrador",
            EmailUsuario = "admin@loja.com",
            Login = "admin", // precisa bater com a RegularExpression: só letras, números, ponto ou underline
            SenhaHash = hashService.GerarHash(senhaInicial),
            Perfil = perfilGerente,
            Ativo = true,
            DataCriacao = DateTime.UtcNow
        };

        db.Usuarios.Add(gerenteInicial);
        db.SaveChanges();

        Console.WriteLine("Usuário Gerente inicial criado com sucesso.");
    }
}
// =====================================================================

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication(); // SEMPRE antes de UseAuthorization
app.UseAuthorization();

app.MapControllers();

app.Run();