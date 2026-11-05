# 📦 Controle de Estoque API

API REST para controle de estoque e vendas de uma loja, desenvolvida em **ASP.NET Core** com **Entity Framework Core** e **SQL Server**. Conta com autenticação via **JWT** e controle de acesso por perfil (Caixa, Estoquista e Gerente).

## 📑 Sumário

- [Funcionalidades](#-funcionalidades)
- [Tecnologias](#-tecnologias)
- [Arquitetura](#-arquitetura)
- [Modelo de dados](#-modelo-de-dados)
- [Perfis e permissões](#-perfis-e-permissões)
- [Regras de negócio](#-regras-de-negócio)
- [Endpoints](#-endpoints)
- [Como executar](#-como-executar)
- [Configuração](#-configuração)
- [Melhorias planejadas](#-melhorias-planejadas)

## ✨ Funcionalidades

- Autenticação com JWT (token válido por 2 horas) e alteração de senha
- Cadastro de usuários com perfis de acesso
- CRUD de categorias, fornecedores e produtos
- Registro de movimentações de estoque (entrada e saída) com histórico
- Consulta de movimentações por produto e por período
- Registro de vendas com cálculo automático do total e baixa de estoque
- Criação automática do banco (migrations) e de um usuário Gerente inicial
- Documentação interativa com Swagger (ambiente de desenvolvimento)

## 🛠 Tecnologias

- C# / ASP.NET Core Web API
- Entity Framework Core 9 (Code First + Migrations)
- SQL Server
- Autenticação JWT Bearer
- BCrypt.Net (hash de senhas)
- Swagger / Swashbuckle

> A versão exata do .NET SDK e dos pacotes está definida no arquivo `Controle_De_estoqueAPI.csproj`.

## 🏗 Arquitetura

O projeto segue uma organização em camadas simples:

```
Controle_De_estoqueAPI/
├── Constantes/        # PerfilConst (nomes dos perfis)
├── Controllers/       # Endpoints HTTP
├── Data/              # DbContext e Factory de design-time
├── Dtos/              # Objetos de entrada/saída, agrupados por entidade
├── Migrations/        # Migrations do EF Core
├── Models/            # Entidades do domínio
├── Service/
│   ├── Interfaces/    # Contratos dos serviços
│   └── Implementações/# Regras de negócio
├── Validations/       # Validadores customizados (CNPJ e telefone)
├── Program.cs         # Configuração, DI, JWT, Swagger e seed
└── appsettings.json
```

Os controllers apenas recebem a requisição e delegam para os **services**, que concentram as regras de negócio e acessam o banco via `ControleEstoqueDbContexto`.

## 🗄 Modelo de dados

| Entidade | Descrição |
|---|---|
| `Usuario` | Login e e-mail únicos, senha em hash BCrypt, perfil, status ativo, datas de criação e último login |
| `Perfil` | Perfis de acesso (seed: Caixa, Estoquista, Gerente) |
| `Categoria` | Categorias de produtos |
| `Fornecedor` | Nome, CNPJ, telefone, e-mail e endereço |
| `Produto` | Nome, descrição, preço, quantidade em estoque, status ativo, categoria e fornecedor |
| `MovimentacaoEstoque` | Entrada, saída ou ajuste de estoque, com usuário responsável e observação |
| `Venda` | Data, usuário, forma de pagamento e total |
| `ItemVenda` | Produto, quantidade, preço unitário e subtotal de cada item da venda |

**Formas de pagamento:** Dinheiro, CartaoCredito, CartaoDebito, Pix, ValeAlimentacao, Outros.

**Tipos de movimentação:** Entrada, Saída, Ajuste.

## 🔐 Perfis e permissões

| Recurso | Caixa | Estoquista | Gerente |
|---|:---:|:---:|:---:|
| Login / alterar senha | ✅ | ✅ | ✅ |
| Categorias | ❌ | ✅ | ✅ |
| Produtos | ❌ | ✅ | ✅ |
| Movimentações de estoque | ❌ | ✅ | ✅ |
| Fornecedores | ❌ | ❌ | ✅ |
| Usuários e perfis | ❌ | ❌ | ✅ |
| Registrar venda | ✅ | ❌ | ✅ |
| Consultar vendas | ❌ | ❌ | ✅ |

## 📋 Regras de negócio

- **Login:** só é permitido para usuários ativos. Credenciais inválidas retornam `401`. A data do último login é atualizada a cada acesso.
- **Token JWT:** carrega o ID, o nome e o perfil do usuário, com validade de 2 horas.
- **Usuários:** e-mail e login são únicos. A exclusão é **lógica** (o usuário é apenas marcado como inativo). Senhas têm de 8 a 72 caracteres.
- **Sessão:** a cada requisição o token é revalidado no banco; usuário desativado ou com perfil alterado perde o acesso imediatamente.
- **Login:** limitado a 5 tentativas por minuto por IP.
- **Fornecedores:** não é possível cadastrar dois fornecedores com o mesmo CNPJ. O CNPJ é validado pelos dígitos verificadores e o telefone deve ter DDD válido (10 ou 11 dígitos).
- **Categorias e fornecedores:** não podem ser excluídos se houver produtos associados.
- **Produtos:** nascem ativos e com estoque zerado; o nome deve ser único. O estoque só é alterado por movimentações. A exclusão é **lógica** (desativa o produto), preservando vendas e histórico.
- **Entrada de estoque:** soma a quantidade ao estoque do produto e registra a movimentação.
- **Saída de estoque:** valida se há estoque suficiente, subtrai a quantidade e registra a movimentação. Não é possível baixar duas vezes o estoque de uma mesma venda; saídas avulsas exigem observação.
- **Concorrência:** o produto usa `RowVersion`; operações simultâneas sobre o mesmo estoque resultam em `409` para a segunda.
- **Vendas:**
  - Deve conter ao menos um item; itens repetidos do mesmo produto são agrupados.
  - Produtos inexistentes, inativos ou sem estoque suficiente impedem a venda.
  - O preço unitário é sempre o preço oficial do cadastro do produto.
  - Venda e baixa de estoque são executadas dentro de uma **transação** (rollback em caso de erro).

## 🔌 Endpoints

Todos os endpoints (exceto o login) exigem o header `Authorization: Bearer {token}`.

**Paginação:** as listagens de produtos, vendas e movimentações aceitam `?pagina=1&tamanhoPagina=20` (máximo 100) e retornam `{ itens, pagina, tamanhoPagina, total, totalPaginas }`.

**Erros:** respondem em `ProblemDetails` com `400` (regra de negócio), `401`, `404`, `409` (conflito/duplicidade) ou `429` (excesso de tentativas de login).

### Auth — `/api/Auth`

| Método | Rota | Descrição |
|---|---|---|
| POST | `/Login` | Autentica e retorna o token (público) |
| PUT | `/AlterarSenha` | Altera a senha do próprio usuário autenticado |

### Categoria — `/api/Categoria`

| Método | Rota | Descrição |
|---|---|---|
| GET | `/` | Lista categorias |
| GET | `/{id}` | Busca por ID |
| POST | `/` | Cria categoria |
| PUT | `/{id}` | Atualiza categoria |
| DELETE | `/{id}` | Exclui categoria |

### Fornecedor — `/api/Fornecedor`

| Método | Rota | Descrição |
|---|---|---|
| GET | `/` | Lista fornecedores |
| GET | `/{id}` | Busca por ID |
| POST | `/` | Cria fornecedor |
| PUT | `/{id}` | Atualiza fornecedor |
| DELETE | `/{id}` | Exclui fornecedor |

### Produtos — `/api/Produtos`

| Método | Rota | Descrição |
|---|---|---|
| GET | `/?incluirInativos=false` | Lista produtos (paginada; por padrão só ativos) |
| GET | `/{id}` | Busca por ID |
| POST | `/` | Cria produto |
| PUT | `/{id}` | Atualiza produto |
| DELETE | `/{id}` | Desativa o produto (exclusão lógica) |

### Movimentação de estoque — `/api/MovimentacaoEstoque`

| Método | Rota | Descrição |
|---|---|---|
| GET | `/` | Lista movimentações |
| GET | `/{id}` | Busca por ID |
| GET | `/movimentacao/produto/{produtoId}` | Histórico de um produto |
| GET | `/movimentacao/periodo?dataInicio=&dataFim=` | Movimentações em um período |
| POST | `/entrada` | Registra entrada de estoque |
| POST | `/saida` | Registra saída de estoque |

### Venda — `/api/Venda`

| Método | Rota | Perfis | Descrição |
|---|---|---|---|
| GET | `/` | Gerente | Lista vendas |
| GET | `/{id}` | Gerente | Busca venda por ID |
| POST | `/` | Caixa, Gerente | Registra uma venda |

### Usuário — `/api/Usuario` (Gerente)

| Método | Rota | Descrição |
|---|---|---|
| GET | `/` | Lista usuários |
| GET | `/{id}` | Busca por ID |
| POST | `/` | Cria usuário |
| PUT | `/{id}` | Atualiza usuário |
| DELETE | `/{id}` | Desativa usuário |

### Perfil — `/api/Perfil` (Gerente)

Lista os perfis cadastrados.

### Exemplo: registrar uma venda

A data é gerada pelo servidor e o usuário é o autenticado no token.

```http
POST /api/Venda
Authorization: Bearer {token}
Content-Type: application/json

{
  "formaPagamento": 4,
  "itensVenda": [
    { "produtoId": 1, "quantidade": 2 },
    { "produtoId": 3, "quantidade": 1 }
  ]
}
```

## 🚀 Como executar

### Pré-requisitos

- [.NET SDK](https://dotnet.microsoft.com/download) compatível com o `.csproj`
- SQL Server (local, Docker ou Azure)

### Passo a passo

```bash
# 1. Clone o repositório
git clone https://github.com/SEU_USUARIO/SEU_REPOSITORIO.git
cd SEU_REPOSITORIO

# 2. Configure os segredos (veja a seção Configuração)
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=ControleEstoque;User Id=sa;Password=SUA_SENHA;TrustServerCertificate=True"
dotnet user-secrets set "Jwt:Key" "uma-chave-secreta-com-pelo-menos-32-caracteres"

# 3. Execute (em Development as migrations são aplicadas automaticamente na inicialização)
dotnet run
```

Em ambiente de desenvolvimento, o Swagger fica disponível em `/swagger`. Para autenticar, faça login em `POST /api/Auth/Login`, copie o token e informe-o no botão **Authorize** (apenas o token, sem a palavra `Bearer`).

### Usuário inicial

Na primeira execução, se não houver nenhum usuário no banco, é criado automaticamente um Gerente:

| Campo | Valor |
|---|---|
| Login | `admin` |
| E-mail | `admin@loja.com` |
| Senha | valor de `SeedGerente:SenhaInicial` (ou uma senha aleatória exibida no console na primeira execução) |

> ⚠️ **Altere a senha após o primeiro login.**

## ⚙️ Configuração

| Chave | Descrição | Obrigatória |
|---|---|:---:|
| `ConnectionStrings:DefaultConnection` | String de conexão do SQL Server | Sim |
| `Jwt:Key` | Chave de assinatura do token (mínimo de 32 caracteres) | Sim |
| `Jwt:Issuer` | Emissor do token (padrão: `ControleDeEstoqueAPI`) | Sim |
| `Jwt:Audience` | Audiência do token (padrão: `ControleDeEstoqueAPI`) | Sim |
| `SeedGerente:SenhaInicial` | Senha do Gerente inicial (se ausente, uma senha aleatória é gerada e exibida uma vez no console) | Recomendada |
| `Database:MigrateOnStartup` | Aplica migrations ao iniciar fora de Development (padrão: `false`) | Não |

Os valores sensíveis **não devem** ser versionados. Em desenvolvimento use `dotnet user-secrets`; em produção use variáveis de ambiente (por exemplo, `ConnectionStrings__DefaultConnection` e `Jwt__Key`).

### Migrations manuais

```bash
dotnet ef migrations add NomeDaMigration
dotnet ef database update
```

## 🧭 Melhorias planejadas

- Testes automatizados (fluxos de venda e movimentação de estoque)
- Normalização do CNPJ (com/sem máscara)
- Conferir, na saída vinculada a uma venda, se os produtos batem com os itens da venda
- Endpoint de ajuste de estoque (`TipoMovimentacao.Ajuste`)
- Refresh token e logout

## 📄 Licença

## 📄 Licença

Distribuído sob a licença MIT. Consulte o arquivo [LICENSE](LICENSE) para mais detalhes.
