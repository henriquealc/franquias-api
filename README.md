# Franquias.Api

API REST desenvolvida em C# / ASP.NET Core para gestão de uma rede de franquias, contemplando cadastro de unidades, produtos, controle de estoque, vendas, cálculo de royalties, fornecedores, chamados de suporte e relatórios gerenciais.

Trabalho acadêmico da disciplina de Desenvolvimento Back-end - Uninter

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
- Cadastro de franqueadora e unidades franqueadas
- Catálogo de produtos e serviços
- Controle de estoque por unidade, com bloqueio de saldo negativo
- Registro de vendas com múltiplos itens, cálculo automático de total e baixa automática de estoque
- Cálculo de royalties por unidade e período
- Cadastro de fornecedores
- Abertura e acompanhamento de chamados de suporte
- Relatórios: faturamento por unidade, produtos mais vendidos, chamados por status, estoque crítico

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

5. Acesse a documentação interativa (Swagger) em:
http://localhost:5035/swagger


## Autenticação (JWT)
A API usa autenticação via JSON Web Token (JWT). A maioria dos endpoints exige um token válido no cabeçalho `Authorization`. Endpoints de cadastro/edição de usuários e franqueadoras exigem, além disso, perfil "Administrador".

### 1. Cadastrar um usuário
`POST /api/Usuarios`

```json
{
  "nome": "Admin Franquias",
  "email": "admin@franquias.com",
  "senhaHash": "senha123",
  "perfil": "Administrador",
  "ativo": true
}
```

> O campo `senhaHash` recebe a senha em texto puro no cadastro, o servidor transforma automaticamente em hash (bcrypt) antes de salvar no banco. A senha original nunca é armazenada.

Resposta (`201 Created`):
```json
{
  "id": 1,
  "nome": "Admin Franquias",
  "email": "admin@franquias.com",
  "senhaHash": "$2a$11$...",
  "perfil": "Administrador",
  "ativo": true
}
```

### 2. Fazer login
`POST /api/Auth/login`

```json
{
  "email": "admin@franquias.com",
  "senha": "senha123"
}
```

Resposta (`200 OK`):
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "nome": "Admin Franquias",
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
| Gestor / Operador | Endpoints de operação (unidades, produtos, estoque, vendas, fornecedores, royalties, chamados, relatórios) exigem apenas estar autenticado |

## Estrutura do projeto
Franquias.Api/
  - Controllers/ -> Endpoints da API
  - Models/ -> Entidades do banco de dados
  - DTOs/ -> Objetos de transferência de dados
  - Data/ -> Contexto do Entity Framework Core
  - Migrations/ -> Histórico de alterações do banco
  - Program.cs -> Configuração da aplicação
