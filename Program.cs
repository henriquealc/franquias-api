using Franquias.Api.Data;
using Franquias.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

var jwtKey = builder.Configuration["Jwt:Key"]!;

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
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
});

builder.Services.AddDbContext<AppDbContext>(options =>
options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.ParameterLocation.Header,
        Description = "Cole aqui o token JWT obtido no login (sem a palavra 'Bearer')"
    });

    options.AddSecurityRequirement(document => new Microsoft.OpenApi.OpenApiSecurityRequirement
    {
        [new Microsoft.OpenApi.OpenApiSecuritySchemeReference("Bearer", document)] = []
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    if (!db.Usuarios.Any())
    {
        db.Usuarios.Add(new Usuario
        {
            Nome = "Administrador Padrão",
            Email = "admin@franquias.com",
            SenhaHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
            Perfil = "Administrador",
            Ativo = true
        });

        db.Usuarios.Add(new Usuario
        {
            Nome = "Gestor Exemplo",
            Email = "gestor@franquias.com",
            SenhaHash = BCrypt.Net.BCrypt.HashPassword("Gestor@123"),
            Perfil = "Gestor",
            Ativo = true
        });

        db.Usuarios.Add(new Usuario
        {
            Nome = "Operador Exemplo",
            Email = "operador@franquias.com",
            SenhaHash = BCrypt.Net.BCrypt.HashPassword("Operador@123"),
            Perfil = "Operador",
            Ativo = true
        });

        var franqueadora = new Franqueadora
        {
            NomeFantasia = "Franquias Brasil",
            RazaoSocial = "Franquias Brasil LTDA",
            Cnpj = "11222333000144",
            Contato = "(81) 3333-0000",
            Endereco = "Av. Central, 500",
            Ativa = true,
            PercentualRoyalty = 5
        };
        db.Franqueadoras.Add(franqueadora);
        db.SaveChanges();

        var unidade1 = new UnidadeFranqueada
        {
            NomeUnidade = "Franquias Centro",
            Cnpj = "99988877000166",
            Cidade = "Recife",
            Endereco = "Rua Principal, 100",
            NomeResponsavel = "Maria Silva",
            ContatoResponsavel = "(81) 99999-0000",
            DataInicio = new DateTime(2024, 1, 15),
            Ativa = true,
            FranqueadoraId = franqueadora.Id
        };

        var unidade2 = new UnidadeFranqueada
        {
            NomeUnidade = "Franquias Norte",
            Cnpj = "55544433000177",
            Cidade = "Olinda",
            Endereco = "Av. Norte, 200",
            NomeResponsavel = "João Souza",
            ContatoResponsavel = "(81) 98888-1111",
            DataInicio = new DateTime(2024, 6, 1),
            Ativa = true,
            FranqueadoraId = franqueadora.Id
        };

        db.UnidadesFranqueadas.AddRange(unidade1, unidade2);
        db.SaveChanges();

        var produto1 = new ProdutoServico
        {
            Nome = "Combo Lanche + Refrigerante",
            Categoria = "Alimentação",
            Descricao = "Combo padrão da rede",
            PrecoBase = 25.90m,
            Ativo = true
        };

        var produto2 = new ProdutoServico
        {
            Nome = "Café Expresso",
            Categoria = "Bebidas",
            Descricao = "Café expresso tradicional",
            PrecoBase = 6.50m,
            Ativo = true
        };

        db.ProdutosServicos.AddRange(produto1, produto2);
        db.SaveChanges();

        db.Fornecedores.Add(new Fornecedor
        {
            Nome = "Distribuidora ABC",
            Cnpj = "98765432000111",
            Contato = "(81) 3222-1111",
            Categoria = "Bebidas",
            Ativo = true
        });

        db.MovimentacoesEstoque.Add(new MovimentacaoEstoque
        {
            Tipo = TipoMovimentacao.Entrada,
            Quantidade = 100,
            Data = DateTime.Now,
            Observacao = "Estoque inicial (seed)",
            ProdutoServicoId = produto1.Id,
            UnidadeFranqueadaId = unidade1.Id
        });

        db.MovimentacoesEstoque.Add(new MovimentacaoEstoque
        {
            Tipo = TipoMovimentacao.Entrada,
            Quantidade = 50,
            Data = DateTime.Now,
            Observacao = "Estoque inicial (seed)",
            ProdutoServicoId = produto2.Id,
            UnidadeFranqueadaId = unidade1.Id
        });

        db.ChamadosSuporte.Add(new ChamadoSuporte
        {
            Categoria = "Financeiro",
            Descricao = "Dúvida sobre cálculo de royalty do mês",
            Prioridade = PrioridadeChamado.Media,
            Status = StatusChamado.Aberto,
            DataAbertura = DateTime.Now,
            UnidadeFranqueadaId = unidade1.Id
        });

        db.SaveChanges();
    }
}

app.Run();