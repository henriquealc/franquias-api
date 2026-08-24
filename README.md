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


### Autenticação

A maioria dos endpoints exige autenticação. Para testar:

1. Crie um usuário via `POST /api/Usuarios` (perfil "Administrador" para acesso total).
2. Faça login via `POST /api/Auth/login` com o e-mail e senha cadastrados.
3. Copie o `token` retornado.
4. No Swagger, clique no botão "Authorize" (canto superior direito) e cole o token.

## Estrutura do projeto
Franquias.Api/
  - Controllers/ -> Endpoints da API
  - Models/ -> Entidades do banco de dados
  - DTOs/ -> Objetos de transferência de dados
  - Data/ -> Contexto do Entity Framework Core
  - Migrations/ -> Histórico de alterações do banco
  - Program.cs -> Configuração da aplicação
