# 📦 Controle de Estoque API

API REST para controle de estoque e vendas de pequenos comércios, desenvolvida em **ASP.NET Core** com **Entity Framework Core** e **SQL Server**. Conta com autenticação via **JWT** e controle de acesso por perfil (Caixa, Estoquista e Gerente).

## ✨ Funcionalidades

- Autenticação com JWT (token válido por 2 horas) e senhas protegidas com **BCrypt**
- Controle de acesso por perfil (`Caixa`, `Estoquista`, `Gerente`)
- CRUD de **Produtos**, **Categorias**, **Fornecedores** e **Usuários**
- **Movimentações de estoque** (entrada, saída e ajuste) com consulta por produto e por período
- **Vendas** com cálculo automático do total usando o preço oficial do produto e baixa de estoque
- Validação de **CNPJ** (com dígitos verificadores) e **telefone** brasileiro (com DDD)
- Exclusão lógica de usuários (o usuário é marcado como inativo)
- Migrations aplicadas automaticamente na inicialização
- Criação automática de um **Gerente inicial** em banco vazio
- Documentação interativa com **Swagger** (ambiente de desenvolvimento)

## 🛠️ Tecnologias

- .NET 9 / ASP.NET Core Web API
- Entity Framework Core (SQL Server)
- JWT Bearer Authentication
- BCrypt.Net (hash de senhas)
- Swagger / Swashbuckle

## 🏗️ Arquitetura

O projeto segue uma arquitetura em camadas simples: **Controllers → Services → DbContext**, com injeção de dependência (serviços registrados como `Scoped`) e DTOs separados para entrada e saída.

```
Controle_De_estoqueAPI/
├── Constantes/          # PerfilConst (nomes dos perfis)
├── Controllers/         # Endpoints da API
├── Data/                # DbContext e DesignTimeFactory
├── Dtos/                # Objetos de entrada e resposta, por módulo
│   ├── Categoria/
│   ├── Fornecedor/
│   ├── MovimentacaoEstoque/
│   ├── Perfil/
│   ├── Produto/
│   ├── Usuario/
│   └── Venda/
├── Migrations/          # Migrations do EF Core
├── Models/              # Entidades do banco
├── Service/
│   ├── Interfaces/      # Contratos dos serviços
│   └── Implementações/  # Regras de negócio
├── Validations/         # Atributos CnpjValido e TelefoneValido
├── appsettings.json
└── Program.cs
```

## 🗃️ Modelo de dados

| Entidade | Descrição |
|---|---|
| `Perfil` | Perfis de acesso (Caixa, Estoquista, Gerente), populados via seed |
| `Usuario` | Usuários do sistema (login e e-mail únicos) |
| `Categoria` | Categorias de produtos |
| `Fornecedor` | Fornecedores (CNPJ, telefone, e-mail, endereço) |
| `Produto` | Produtos com preço, estoque, categoria e fornecedor |
| `MovimentacaoEstoque` | Histórico de entradas, saídas e ajustes |
| `Venda` | Cabeçalho da venda (usuário, data, total, forma de pagamento) |
| `ItemVenda` | Itens de cada venda |

**Relacionamentos principais:** `Perfil 1:N Usuario`, `Categoria 1:N Produto`, `Fornecedor 1:N Produto`, `Usuario 1:N Venda`, `Venda 1:N ItemVenda`, `Produto 1:N MovimentacaoEstoque`.

### Enums

| `TipoMovimentacao` | Valor |
|---|---|
| Entrada | 1 |
| Saida | 2 |
| Ajuste | 3 |

| `FormaPagamento` | Valor |
|---|---|
| Dinheiro | 1 |
| CartaoCredito | 2 |
| CartaoDebito | 3 |
| Pix | 4 |
| ValeAlimentacao | 5 |
| Outros | 6 |

## 🚀 Como executar

### Pré-requisitos

- [.NET SDK 9](https://dotnet.microsoft.com/download)
- SQL Server (local, Docker ou Azure SQL)
- (Opcional) [EF Core CLI](https://learn.microsoft.com/ef/core/cli/dotnet): `dotnet tool install --global dotnet-ef`

### 1. Clonar o repositório

```bash
git clone https://github.com/<seu-usuario>/<seu-repositorio>.git
cd <seu-repositorio>
```

### 2. Configurar as variáveis

O `appsettings.json` vem com a connection string e a chave JWT **vazias** de propósito. Nunca versione segredos. Em desenvolvimento, use o **User Secrets**:

```bash
dotnet user-secrets init

dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=ControleEstoque;User Id=sa;Password=SuaSenhaForte;TrustServerCertificate=True"

dotnet user-secrets set "Jwt:Key" "uma-chave-secreta-com-no-minimo-32-caracteres"

# Opcional: senha do Gerente inicial
dotnet user-secrets set "SeedGerente:SenhaInicial" "UmaSenhaForte123!"
```

Em produção, use variáveis de ambiente (o `__` substitui o `:`):

```
ConnectionStrings__DefaultConnection
Jwt__Key
Jwt__Issuer
Jwt__Audience
SeedGerente__SenhaInicial
```

| Configuração | Obrigatória | Descrição |
|---|---|---|
| `ConnectionStrings:DefaultConnection` | ✅ | Connection string do SQL Server |
| `Jwt:Key` | ✅ | Chave de assinatura do token (mínimo de **32 caracteres**; a aplicação não inicia sem ela) |
| `Jwt:Issuer` | — | Padrão: `ControleDeEstoqueAPI` |
| `Jwt:Audience` | — | Padrão: `ControleDeEstoqueAPI` |
| `SeedGerente:SenhaInicial` | — | Senha do Gerente inicial. **Se não informada, usa uma senha padrão temporária** |

### 3. Rodar a aplicação

```bash
dotnet run
```

Ao iniciar, a aplicação aplica as migrations automaticamente (`Database.Migrate()`), então não é necessário rodar `dotnet ef database update`.

Com `ASPNETCORE_ENVIRONMENT=Development`, o Swagger fica disponível em `https://localhost:<porta>/swagger`.

### 🔑 Usuário inicial

Se o banco não tiver nenhum usuário, é criado um Gerente:

| Campo | Valor |
|---|---|
| Login | `admin` |
| E-mail | `admin@loja.com` |
| Senha | valor de `SeedGerente:SenhaInicial` (ou a senha padrão temporária, caso não configurada) |
| Perfil | Gerente |

> ⚠️ **Segurança:** sempre defina `SeedGerente:SenhaInicial` antes do primeiro start e troque a senha após o primeiro acesso.

## 🔐 Autenticação e perfis

1. Faça login em `POST /api/Auth/Login` e copie o `token` da resposta.
2. Envie o token nas demais requisições: `Authorization: Bearer <token>`.
3. No Swagger, clique em **Authorize** e cole apenas o token (sem a palavra `Bearer`).

| Perfil | Acesso |
|---|---|
| **Gerente** | Acesso total |
| **Estoquista** | Produtos, Categorias e Movimentações de estoque |
| **Caixa** | Registrar vendas |

## 📡 Endpoints

### Auth — `/api/Auth`

| Método | Rota | Acesso | Descrição |
|---|---|---|---|
| POST | `/api/Auth/Login` | Público | Autentica e retorna o token JWT |
| PUT | `/api/Auth/Login?id={id}` | Autenticado | Altera a senha do usuário |

**Exemplo de login**

```json
POST /api/Auth/Login
{
  "login": "admin",
  "senha": "sua-senha"
}
```

```json
{
  "sucesso": true,
  "token": "eyJhbGciOi...",
  "usuarioId": 1,
  "nome": "Administrador",
  "perfilNome": "Gerente",
  "mensagem": "Login bem-sucedido"
}
```

### Usuários — `/api/Usuario` (Gerente)

| Método | Rota | Descrição |
|---|---|---|
| GET | `/api/Usuario` | Lista usuários |
| GET | `/api/Usuario/{id}` | Busca usuário por ID |
| POST | `/api/Usuario` | Cria usuário |
| PUT | `/api/Usuario/{id}` | Atualiza nome, e-mail e perfil |
| DELETE | `/api/Usuario/{id}` | Desativa o usuário (exclusão lógica) |

```json
POST /api/Usuario
{
  "nome": "Maria Silva",
  "email": "maria@loja.com",
  "login": "maria.silva",
  "senha": "senha123",
  "perfilId": 1
}
```

`perfilId`: `1` = Caixa, `2` = Estoquista, `3` = Gerente.

### Categorias — `/api/Categoria` (Gerente, Estoquista)

| Método | Rota | Descrição |
|---|---|---|
| GET | `/api/Categoria` | Lista categorias |
| GET | `/api/Categoria/{id}` | Busca por ID |
| POST | `/api/Categoria` | Cria categoria |
| PUT | `/api/Categoria/{id}` | Atualiza categoria |
| DELETE | `/api/Categoria/{id}` | Exclui (bloqueado se houver produtos associados) |

```json
{ "nomeCategoria": "Bebidas" }
```

### Fornecedores — `/api/Fornecedor` (Gerente)

| Método | Rota | Descrição |
|---|---|---|
| GET | `/api/Fornecedor` | Lista fornecedores |
| GET | `/api/Fornecedor/{id}` | Busca por ID |
| POST | `/api/Fornecedor` | Cria fornecedor (CNPJ único) |
| PUT | `/api/Fornecedor/{id}` | Atualiza fornecedor |
| DELETE | `/api/Fornecedor/{id}` | Exclui (bloqueado se houver produtos associados) |

```json
{
  "nome": "Distribuidora Exemplo LTDA",
  "cnpj": "11.222.333/0001-81",
  "telefone": "(11) 91234-5678",
  "email": "contato@exemplo.com",
  "endereco": "Rua Exemplo, 123 - São Paulo/SP"
}
```

### Produtos — `/api/Produtos` (Gerente, Estoquista)

| Método | Rota | Descrição |
|---|---|---|
| GET | `/api/Produtos` | Lista produtos |
| GET | `/api/Produtos/{id}` | Busca por ID |
| POST | `/api/Produtos` | Cria produto (nasce ativo e com estoque 0) |
| PUT | `/api/Produtos/{id}` | Atualiza produto |
| DELETE | `/api/Produtos/{id}` | Exclui produto |

```json
POST /api/Produtos
{
  "nomeProduto": "Refrigerante 2L",
  "descricaoProduto": "Refrigerante sabor cola 2 litros",
  "categoriaId": 1,
  "fornecedorId": 1,
  "precoProduto": 9.90
}
```

> O estoque só é alterado por movimentações de estoque e vendas.

### Movimentações de estoque — `/api/MovimentacaoEstoque` (Gerente, Estoquista)

| Método | Rota | Descrição |
|---|---|---|
| GET | `/api/MovimentacaoEstoque` | Lista movimentações |
| GET | `/api/MovimentacaoEstoque/{id}` | Busca por ID |
| GET | `/api/MovimentacaoEstoque/movimentação/produto/{produtoId}` | Movimentações de um produto (mais recentes primeiro) |
| GET | `/api/MovimentacaoEstoque/movimentação/periodo?dataInicio=&dataFim=` | Movimentações em um período |
| POST | `/api/MovimentacaoEstoque/entrada` | Registra entrada de estoque |
| POST | `/api/MovimentacaoEstoque/saida` | Registra saída de estoque (itens) |

```json
POST /api/MovimentacaoEstoque/entrada
{
  "produtoId": 1,
  "usuarioId": 1,
  "fornecedorId": 1,
  "quantidade": 50,
  "tipoMovimentacao": 1,
  "observacao": "Reposição mensal"
}
```

### Vendas — `/api/Venda`

| Método | Rota | Acesso | Descrição |
|---|---|---|---|
| GET | `/api/Venda` | Gerente | Lista vendas |
| GET | `/api/Venda/{id}` | Gerente | Busca venda por ID |
| POST | `/api/Venda` | Caixa, Gerente | Registra uma venda |

```json
POST /api/Venda
{
  "dataVenda": "2026-01-15T10:30:00",
  "usuarioId": 1,
  "formaPagamento": 4,
  "itensVenda": [
    { "produtoId": 1, "quantidade": 2, "precoUnitario": 9.90 }
  ]
}
```

O total da venda é calculado pelo servidor usando o **preço cadastrado do produto**, e o estoque é baixado automaticamente via movimentação de saída.

### Perfis — `/api/Perfil` (Gerente)

Endpoint de listagem de perfis (em ajuste, veja a seção de melhorias).

## ✅ Validações

- **CNPJ:** 14 dígitos, rejeita sequências repetidas e confere os dois dígitos verificadores. Aceita com ou sem máscara.
- **Telefone:** 10 ou 11 dígitos com DDD válido; celulares (11 dígitos) devem começar com 9.
- **Login:** apenas letras, números, ponto e underline (`.` e `_`).
- **Senha (criação de usuário):** mínimo de 6 caracteres.
- **E-mail e login de usuário:** únicos no banco.

## 🧱 Migrations

As migrations são aplicadas automaticamente ao iniciar. Para gerenciá-las manualmente:

```bash
# Criar nova migration
dotnet ef migrations add NomeDaMigration

# Aplicar no banco
dotnet ef database update
```

O projeto possui uma `IDesignTimeDbContextFactory`, que lê a connection string de `appsettings.json`, User Secrets ou variáveis de ambiente.

## 🛣️ Melhorias planejadas

- Camada de Models e DTOs: revisar validações dos DTOs de atualização (produto, usuário, categoria)
- Padronizar tratamento de erros com middleware global (hoje alguns serviços lançam `Exception` genérica)
- Envolver a criação de venda e a baixa de estoque em uma **transação**
- Obter o `id` do usuário a partir do token na troca de senha
- Revisar comportamento de exclusão em cascata para preservar o histórico de vendas
- Testes automatizados (unitários e de integração)
- Paginação nas listagens

## 📄 Licença

Defina aqui a licença do projeto (ex.: MIT).

## 👤 Autor

Desenvolvido por ** Gilmar De Azevedo Prado** — [GitHub](https://github.com/Gilmar381)
