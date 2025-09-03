#     WebApplication - Arquitetura Limpa com .NET 8

Este projeto demonstra a implementação de uma **API RESTful** utilizando **Arquitetura Limpa**, **Princípios SOLID** e **Design Patterns** em .NET 8.

##    Índice

- [   Objetivo](#-objetivo)
- [    Arquitetura](#-arquitetura)
- [   Estrutura de Pastas](#-estrutura-de-pastas)
- [    Tecnologias](#-tecnologias)
- [   Como Executar](#-como-executar)
- [   Conceitos Explicados](#-conceitos-explicados)
- [   Autenticação JWT](#-autenticação-jwt)
- [   Banco de Dados](#-banco-de-dados)
- [   API Documentation](#-api-documentation)
- [   Testando a API](#-testando-a-api)

##    Objetivo

Criar uma **API de autenticação** robusta, escalável e de fácil manutenção, seguindo as melhores práticas de desenvolvimento, servindo como **exemplo educacional** para desenvolvedores que desejam aprender sobre arquitetura de software.

##     Arquitetura

Este projeto implementa a **Arquitetura Limpa (Clean Architecture)** com **separação de responsabilidades** em camadas bem definidas:

```
                                                                 
    Controllers             Services             Repositories    
   (Presentation)          (Business)             (Data Access)  
                                                                 
                                                          
                                                          
                                                                 
      Models               Interfaces              Database      
    (DTOs/Data)           (Contracts)             (Entities)     
                                                                 
```

### Fluxo de Dados:
1. **Controller** recebe requisição HTTP
2. **Controller** chama o **Service** correspondente
3. **Service** executa lógica de negócio e chama **Repository**
4. **Repository** acessa o banco de dados via **Entity Framework**
5. Dados retornam pela mesma cadeia até o **Controller**
6. **Controller** retorna resposta HTTP formatada

##    Estrutura de Pastas

```
   WebApplication/
       Controllers/           #    Controladores da API
        AuthenticationController.cs
       Data/                 #     Acesso a dados
        AppDbContext.cs      # Contexto do Entity Framework
        Entities/            # Entidades do banco de dados
            TbUser.cs
       Models/               #    DTOs e contratos
        Authentication/      # DTOs de autenticação
            LoginRequestDTO.cs
            LoginResponseDTO.cs
            RegisterRequestDTO.cs
            RegisterResponseDTO.cs
       Repositories/         #     Acesso a dados
        IRepositories/       # Interfaces dos repositories
            IAuthenticationRepository.cs
        AuthenticationRepository.cs
       Services/            #    Lógica de negócio
        IServices/          # Interfaces dos services
            IAuthenticationService.cs
        AuthenticationService.cs
        JwtService.cs
       Program.cs           #   Configuração da aplicação
       appsettings.json     #    Configurações gerais
       appsettings.Development.json #    Configurações de desenvolvimento
```

##     Tecnologias

- **Framework**: .NET 8.0 (LTS)
- **ORM**: Entity Framework Core
- **Database**: SQL Server
- **Authentication**: JWT (JSON Web Tokens)
- **Password Hashing**: BCrypt.NET
- **Documentation**: Swagger/OpenAPI
- **Architecture Patterns**: Clean Architecture, Repository Pattern, Dependency Injection

##    Como Executar

### Pré-requisitos
- .NET 8 SDK
- SQL Server (LocalDB ou instância completa)
- Visual Studio 2022 ou VS Code

### Passos para execução

1. **Clone o repositório**
```bash
git clone <repository-url>
cd WebApplication
```

2. **Restore dependencies**
```bash
dotnet restore
```

3. **Configure o banco de dados**
   - Ajuste a connection string em `appsettings.Development.json`
   - Execute as migrations:
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

4. **Execute a aplicação**
```bash
dotnet run
```

5. **Acesse o Swagger**
   - Desenvolvimento: `https://localhost:7xxx/swagger`

##    Conceitos Explicados

###    Princípios SOLID

#### **S** - Single Responsibility Principle
Cada classe tem uma única responsabilidade:
- `AuthenticationController`: Apenas gerencia endpoints de autenticação
- `AuthenticationService`: Apenas lógica de negócio de autenticação
- `AuthenticationRepository`: Apenas acesso a dados de usuários

#### **O** - Open/Closed Principle
Classes abertas para extensão, fechadas para modificação:
- Novas funcionalidades via interfaces
- Extensibilidade sem alterar código existente

#### **L** - Liskov Substitution Principle
Implementações podem ser substituídas por suas interfaces:
```csharp
IAuthenticationService service = new AuthenticationService();
// Pode ser substituído por qualquer implementação da interface
```

#### **I** - Interface Segregation Principle
Interfaces específicas e coesas:
- `IAuthenticationRepository`: Apenas métodos relacionados à autenticação
- `IAuthenticationService`: Apenas operações de negócio de autenticação

#### **D** - Dependency Inversion Principle
Dependência de abstrações, não implementações:
```csharp
public class AuthenticationService : IAuthenticationService
{
    private readonly IAuthenticationRepository _repository; // Interface, não implementação
    
    public AuthenticationService(IAuthenticationRepository repository)
    {
        _repository = repository; // Injeção de dependência
    }
}
```

###     Design Patterns Implementados

#### Repository Pattern
Abstração do acesso a dados:
```csharp
public interface IAuthenticationRepository
{
    Task<TbUser > GetUserByEmailAsync(string email);
    Task<TbUser> CreateUserAsync(TbUser user);
    Task<bool> EmailExistsAsync(string email);
}
```

#### Dependency Injection
Configurado no `Program.cs`:
```csharp
builder.Services.AddScoped<IAuthenticationRepository, AuthenticationRepository>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
```

#### DTO Pattern
Separação entre entidades de domínio e dados de transporte:
- `TbUser` (Entidade) vs `LoginRequestDTO` (Transporte)

###    Entity Framework - Configuração Interna

Implementamos um pattern personalizado onde cada entidade configura a si mesma:

```csharp
public static void ConfigureModelBuilder(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<TbUser>(entity =>
    {
        entity.HasKey(e => e.Id).HasName("PK_TbUser");
        entity.ToTable("TbUser");
        // ... mais configurações
    });
}
```

**Benefícios:**
-   AppDbContext mais limpo
-   Configuração co-localizada com a entidade
-   Melhor performance do IntelliSense
-   Facilita manutenção

##    Autenticação JWT

### Como Funciona
1. **Login**: Usuário envia email/senha
2. **Validação**: Sistema valida credenciais
3. **Token**: Sistema gera JWT com claims do usuário
4. **Uso**: Cliente inclui token no header `Authorization: Bearer <token>`
5. **Validação**: Sistema valida token em endpoints protegidos

### Configuração JWT
```json
{
  "JwtSettings": {
    "SecretKey": "minha-chave-super-secreta-jwt-com-mais-de-32-caracteres-para-seguranca",
    "Issuer": "WebApplication",
    "Audience": "WebApplication-Users",
    "ExpiryInHours": 24
  }
}
```

### Endpoints Protegidos
```csharp
[HttpDelete("user")]
[Authorize] // Requer JWT válido
public async Task<ActionResult> DeleteUser()
{
    var userIdClaim = User.FindFirst("userId") .Value; // Extrai dados do token
    // ... lógica
}
```

##    Banco de Dados

### Estrutura da Tabela TbUser
```sql
CREATE TABLE TbUser (
    Id int IDENTITY(1,1) PRIMARY KEY,
    Name nvarchar(100) NOT NULL,
    Email nvarchar(255) NOT NULL,
    PasswordHash nvarchar(255) NOT NULL,
    CreatedAt datetime2 NOT NULL,
    UpdatedAt datetime2 NOT NULL,
    IsActive bit NOT NULL DEFAULT 1
);

CREATE UNIQUE INDEX IX_TbUser_Email ON TbUser (Email);
```

### Features Implementadas
- **Soft Delete**: Campo `IsActive` para exclusão lógica
- **Auditing**: Campos `CreatedAt` e `UpdatedAt` atualizados automaticamente
- **Email Único**: Índice único no campo Email
- **Password Hash**: Senhas criptografadas com BCrypt

##    API Documentation

### Endpoints Disponíveis

#### POST `/api/Authentication/login`
Autentica um usuário existente.

**Request Body:**
```json
{
  "email": "user@example.com",
  "password": "123456"
}
```

**Response (Success):**
```json
{
  "success": true,
  "message": "Login realizado com sucesso",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "userId": 1,
  "email": "user@example.com",
  "name": "Nome do Usuário",
  "expiresAt": "2024-01-15T10:30:00Z"
}
```

#### POST `/api/Authentication/register`
Registra um novo usuário.

**Request Body:**
```json
{
  "name": "Nome do Usuário",
  "email": "user@example.com",
  "password": "123456",
  "confirmPassword": "123456"
}
```

#### GET `/api/Authentication/me`   
Retorna dados do usuário autenticado (requer JWT).

#### DELETE `/api/Authentication/user`   
Exclui (soft delete) o usuário autenticado (requer JWT).

##    Testando a API

### Usando cURL

**1. Registrar usuário:**
```bash
curl -X POST "https://localhost:7xxx/api/Authentication/register" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Teste Usuario",
    "email": "teste@email.com",
    "password": "123456",
    "confirmPassword": "123456"
  }'
```

**2. Fazer login:**
```bash
curl -X POST "https://localhost:7xxx/api/Authentication/login" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "teste@email.com",
    "password": "123456"
  }'
```

**3. Acessar endpoint protegido:**
```bash
curl -X GET "https://localhost:7xxx/api/Authentication/me" \
  -H "Authorization: Bearer SEU_TOKEN_AQUI"
```

### Usando Swagger
1. Execute a aplicação
2. Acesse `https://localhost:7xxx/swagger`
3. Use o botão "Authorize" para configurar o JWT
4. Teste os endpoints diretamente na interface

##    Próximos Passos (Para Aprendizado)

Para expandir este projeto e aprender mais conceitos:

1. **Logging**: Implementar Serilog para logs estruturados
2. **Validation**: FluentValidation para validações mais robustas
3. **Caching**: Redis para cache de dados
4. **Testing**: Unit tests com xUnit e Moq
5. **Health Checks**: Monitoramento de saúde da aplicação
6. **Rate Limiting**: Controle de taxa de requisições
7. **CORS**: Configuração para aplicações frontend
8. **Docker**: Containerização da aplicação
9. **CI/CD**: Pipeline de deploy automatizado

##    Contribuição

Este é um projeto educacional! Contribuições são bem-vindas:

1. Fork o projeto
2. Crie uma branch para sua feature (`git checkout -b feature/AmazingFeature`)
3. Commit suas mudanças (`git commit -m 'Add some AmazingFeature'`)
4. Push para a branch (`git push origin feature/AmazingFeature`)
5. Abra um Pull Request

##    Licença

Este projeto está sob a licença MIT. Veja o arquivo `LICENSE` para mais detalhes.

---

**   Desenvolvido para fins educacionais - Demonstrando Arquitetura Limpa em .NET 8**