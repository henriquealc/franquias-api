# Franquias.Api

API REST desenvolvida em C# / ASP.NET Core para gestão de uma rede de franquias, contemplando cadastro de unidades, produtos, controle de estoque, vendas, cálculo de royalties, fornecedores, chamados de suporte e relatórios gerenciais.

Trabalho acadêmico da disciplina de Desenvolvimento Back-end - Uninter.

## Tecnologias utilizadas

- C# / .NET 10
- ASP.NET Core Web API
- Entity Framework Core
- SQLite
- Autenticação JWT (JSON Web Token)
- BCrypt (hash de senhas)
- Swagger / Swashbuckle (documentação e testes de API)

## Funcionalidades

- Cadastro e autenticação de usuários, com perfis de acesso (Administrador / Gestor / Operador)
- Cadastro de franqueadora e unidades franqueadas, com busca por nome, cidade, CNPJ ou responsável
- Catálogo de produtos e serviços, com busca por nome, categoria ou status
- Controle de estoque por unidade, com bloqueio de saldo negativo
- Registro de vendas com múltiplos itens, cálculo automático de total e baixa automática de estoque
- Cálculo de royalties por unidade e período
- Cadastro de fornecedores, com busca por nome, CNPJ ou status
- Abertura e acompanhamento de chamados de suporte (categoria, prioridade, status)
- Paginação e ordenação nos principais endpoints de listagem (Usuarios, UnidadesFranqueadas, ProdutosServicos, Vendas, Fornecedores)
- Relatórios: faturamento por unidade, ranking de unidades, total de royalties, produtos mais vendidos, chamados por status, estoque crítico
- Seed automático de dados de exemplo na primeira execução (3 usuários de perfis diferentes, franqueadora, unidades, produtos, fornecedor, movimentações de estoque e chamado)

## Como executar o projeto

### Pré-requisitos
- .NET SDK 10 instalado

### Passos

1. Clone o repositório:

git clone https://github.com/henriquealc/franquias-api.git
cd franquias-api


2. Restaure as dependências:

dotnet restore


3. Aplique as migrations (cria o banco de dados SQLite):

dotnet ef database update


4. Rode a API:

dotnet run


Na primeira execução, o sistema cria automaticamente um conjunto de dados de exemplo (seed) caso o banco esteja vazio, não é necessário cadastrar nada manualmente para começar a testar.

5. Acesse a documentação interativa (Swagger) em:

http://localhost:5035/swagger


## Autenticação (JWT)

A API usa autenticação via JSON Web Token (JWT). A maioria dos endpoints exige um token válido no cabeçalho `Authorization`. Endpoints de cadastro/edição de usuários e franqueadoras exigem perfil "Administrador"; algumas ações de gestão (editar unidade, pagar royalty, excluir fornecedor) exigem "Administrador" ou "Gestor".

### 1. Usuário administrador padrão (criado pelo seed)

Email: admin@franquias.com
Senha: Admin@123


### 2. Fazer login

`POST /api/Auth/login`

```json
{
  "email": "admin@franquias.com",
  "senha": "Admin@123"
}
```

Resposta (`200 OK`):
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "nome": "Administrador Padrão",
  "perfil": "Administrador"
}
```

O token expira em 120 minutos (configurável em `appsettings.json`, seção `Jwt:ExpiraEmMinutos`).

### 3. Usar o token nas próximas requisições

Envie o token no cabeçalho `Authorization` de cada requisição:

Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...


**Testando pelo Swagger:**
1. Clique no botão "Authorize" (cadeado, canto superior direito da página `/swagger`).
2. Cole apenas o valor do token (sem a palavra "Bearer", o Swagger adiciona isso automaticamente).
3. Clique em "Authorize" e depois "Close".
4. Todos os endpoints protegidos passam a funcionar normalmente enquanto o token for válido.

Sem token, qualquer endpoint protegido retorna `401 Unauthorized`.

### Perfis de acesso

| Perfil | Acesso |
|---|---|
| Administrador | Todos os endpoints, incluindo cadastro/edição de usuários e franqueadoras |
| Gestor | Endpoints operacionais + ações de gestão restritas (editar unidade, pagar royalty, excluir fornecedor) |
| Operador | Endpoints operacionais do dia a dia (vendas, estoque, chamados, consultas) |

## Paginação, ordenação e busca

Os endpoints de listagem de Usuarios, UnidadesFranqueadas, ProdutosServicos, Vendas e Fornecedores aceitam:

- `pagina` (padrão: 1)
- `tamanhoPagina` (padrão: 10)
- `orderBy` (varia por entidade, ex: `nome`, `email`, `data`, `valor`, `preco`)

Exemplo:

GET /api/UnidadesFranqueadas?pagina=1&tamanhoPagina=10&orderBy=nome


Além disso, UnidadesFranqueadas, ProdutosServicos e Fornecedores possuem endpoints `/buscar` com filtros específicos (nome, cidade, CNPJ, responsável, categoria, status).

## Exemplos de uso

Abaixo, exemplos reais de requisições para os principais fluxos da API. Faça login primeiro (veja seção "Autenticação" acima) e use o token retornado no cabeçalho `Authorization` de cada chamada seguinte.

### Cadastrar uma franqueadora

`POST /api/Franqueadoras`

```json
{
  "nomeFantasia": "Franquias Brasil",
  "razaoSocial": "Franquias Brasil LTDA",
  "cnpj": "11222333000144",
  "contato": "(81) 3333-0000",
  "endereco": "Av. Central, 500",
  "ativa": true,
  "percentualRoyalty": 5
}
```

### Cadastrar uma unidade franqueada

`POST /api/UnidadesFranqueadas`

```json
{
  "nomeUnidade": "Franquias Centro",
  "cnpj": "99988877000166",
  "cidade": "Recife",
  "endereco": "Rua Principal, 100",
  "nomeResponsavel": "Maria Silva",
  "contatoResponsavel": "(81) 99999-0000",
  "dataInicio": "2024-01-15",
  "ativa": true,
  "franqueadoraId": 1
}
```

### Buscar unidades por cidade, nome, CNPJ ou responsável

GET /api/UnidadesFranqueadas/buscar?cidade=Recife


### Cadastrar um produto ou serviço

`POST /api/ProdutosServicos`

```json
{
  "nome": "Combo Lanche + Refrigerante",
  "categoria": "Alimentação",
  "descricao": "Combo padrão da rede",
  "precoBase": 25.90,
  "ativo": true
}
```

### Registrar entrada de estoque

`POST /api/Estoque/movimentar`

```json
{
  "tipo": "Entrada",
  "quantidade": 50,
  "observacao": "Estoque inicial",
  "produtoServicoId": 1,
  "unidadeFranqueadaId": 1
}
```

### Consultar saldo de estoque

GET /api/Estoque/saldo/1/1


### Registrar uma venda com itens

`POST /api/Vendas`

```json
{
  "unidadeFranqueadaId": 1,
  "itens": [
    { "produtoServicoId": 1, "quantidade": 5 }
  ]
}
```

O valor total é calculado automaticamente a partir dos itens, e o estoque é debitado na mesma operação.

### Calcular o royalty de uma unidade em um período

POST /api/Royalties/calcular?unidadeFranqueadaId=1&periodoInicio=2026-01-01&periodoFim=2026-12-31


### Consultar relatórios gerenciais

GET /api/Relatorios/faturamento-por-unidade
GET /api/Relatorios/ranking-unidades
GET /api/Relatorios/royalties-total
GET /api/Relatorios/produtos-mais-vendidos
GET /api/Relatorios/chamados-por-status
GET /api/Relatorios/estoque-critico?minimo=20


### Cadastrar um fornecedor

`POST /api/Fornecedores`

```json
{
  "nome": "Distribuidora ABC",
  "cnpj": "98765432000111",
  "contato": "(81) 3222-1111",
  "categoria": "Bebidas",
  "ativo": true
}
```

### Abrir um chamado de suporte

`POST /api/ChamadosSuporte`

```json
{
  "categoria": "Financeiro",
  "descricao": "Dúvida sobre cálculo de royalty do mês",
  "prioridade": "Media",
  "unidadeFranqueadaId": 1
}
```

## Estrutura do projeto

Franquias.Api/
  - Controllers/ -> Endpoints da API
  - Models/ -> Entidades do banco de dados
  - DTOs/ -> Objetos de transferência de dados
  - Data/ -> Contexto do Entity Framework Core
  - Migrations/ -> Histórico de alterações do banco
  - Program.cs -> Configuração da aplicação e seed de dados
