#     WebApplication - Arquitetura Limpa com .NET 8

Este projeto demonstra a implementa��o de uma **API RESTful** utilizando **Arquitetura Limpa**, **Princ�pios SOLID** e **Design Patterns** em .NET 8.

##    �ndice

- [   Objetivo](#-objetivo)
- [    Arquitetura](#-arquitetura)
- [   Estrutura de Pastas](#-estrutura-de-pastas)
- [    Tecnologias](#-tecnologias)
- [   Como Executar](#-como-executar)
- [   Conceitos Explicados](#-conceitos-explicados)
- [   Autentica��o JWT](#-autentica��o-jwt)
- [   Banco de Dados](#-banco-de-dados)
- [   API Documentation](#-api-documentation)
- [   Testando a API](#-testando-a-api)

##    Objetivo

Criar uma **API de autentica��o** robusta, escal�vel e de f�cil manuten��o, seguindo as melhores pr�ticas de desenvolvimento, servindo como **exemplo educacional** para desenvolvedores que desejam aprender sobre arquitetura de software.

##     Arquitetura

Este projeto implementa a **Arquitetura Limpa (Clean Architecture)** com **separa��o de responsabilidades** em camadas bem definidas:

```
                                                                 
    Controllers             Services             Repositories    
   (Presentation)          (Business)             (Data Access)  
                                                                 
                                                          
                                                          
                                                                 
      Models               Interfaces              Database      
    (DTOs/Data)           (Contracts)             (Entities)     
                                                                 
```

### Fluxo de Dados:
1. **Controller** recebe requisi��o HTTP
2. **Controller** chama o **Service** correspondente
3. **Service** executa l�gica de neg�cio e chama **Repository**
4. **Repository** acessa o banco de dados via **Entity Framework**
5. Dados retornam pela mesma cadeia at� o **Controller**
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
        Authentication/      # DTOs de autentica��o
            LoginRequestDTO.cs
            LoginResponseDTO.cs
            RegisterRequestDTO.cs
            RegisterResponseDTO.cs
       Repositories/         #     Acesso a dados
        IRepositories/       # Interfaces dos repositories
            IAuthenticationRepository.cs
        AuthenticationRepository.cs
       Services/            #    L�gica de neg�cio
        IServices/          # Interfaces dos services
            IAuthenticationService.cs
        AuthenticationService.cs
        JwtService.cs
       Program.cs           #   Configura��o da aplica��o
       appsettings.json     #    Configura��es gerais
       appsettings.Development.json #    Configura��es de desenvolvimento
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

### Pr�-requisitos
- .NET 8 SDK
- SQL Server (LocalDB ou inst�ncia completa)
- Visual Studio 2022 ou VS Code

### Passos para execu��o

1. **Clone o reposit�rio**
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

4. **Execute a aplica��o**
```bash
dotnet run
```

5. **Acesse o Swagger**
   - Desenvolvimento: `https://localhost:7xxx/swagger`

##    Conceitos Explicados

###    Princ�pios SOLID

#### **S** - Single Responsibility Principle
Cada classe tem uma �nica responsabilidade:
- `AuthenticationController`: Apenas gerencia endpoints de autentica��o
- `AuthenticationService`: Apenas l�gica de neg�cio de autentica��o
- `AuthenticationRepository`: Apenas acesso a dados de usu�rios

#### **O** - Open/Closed Principle
Classes abertas para extens�o, fechadas para modifica��o:
- Novas funcionalidades via interfaces
- Extensibilidade sem alterar c�digo existente

#### **L** - Liskov Substitution Principle
Implementa��es podem ser substitu�das por suas interfaces:
```csharp
IAuthenticationService service = new AuthenticationService();
// Pode ser substitu�do por qualquer implementa��o da interface
```

#### **I** - Interface Segregation Principle
Interfaces espec�ficas e coesas:
- `IAuthenticationRepository`: Apenas m�todos relacionados � autentica��o
- `IAuthenticationService`: Apenas opera��es de neg�cio de autentica��o

#### **D** - Dependency Inversion Principle
Depend�ncia de abstra��es, n�o implementa��es:
```csharp
public class AuthenticationService : IAuthenticationService
{
    private readonly IAuthenticationRepository _repository; // Interface, n�o implementa��o
    
    public AuthenticationService(IAuthenticationRepository repository)
    {
        _repository = repository; // Inje��o de depend�ncia
    }
}
```

###     Design Patterns Implementados

#### Repository Pattern
Abstra��o do acesso a dados:
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
Separa��o entre entidades de dom�nio e dados de transporte:
- `TbUser` (Entidade) vs `LoginRequestDTO` (Transporte)

###    Entity Framework - Configura��o Interna

Implementamos um pattern personalizado onde cada entidade configura a si mesma:

```csharp
public static void ConfigureModelBuilder(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<TbUser>(entity =>
    {
        entity.HasKey(e => e.Id).HasName("PK_TbUser");
        entity.ToTable("TbUser");
        // ... mais configura��es
    });
}
```

**Benef�cios:**
-   AppDbContext mais limpo
-   Configura��o co-localizada com a entidade
-   Melhor performance do IntelliSense
-   Facilita manuten��o

##    Autentica��o JWT

### Como Funciona
1. **Login**: Usu�rio envia email/senha
2. **Valida��o**: Sistema valida credenciais
3. **Token**: Sistema gera JWT com claims do usu�rio
4. **Uso**: Cliente inclui token no header `Authorization: Bearer <token>`
5. **Valida��o**: Sistema valida token em endpoints protegidos

### Configura��o JWT
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
[Authorize] // Requer JWT v�lido
public async Task<ActionResult> DeleteUser()
{
    var userIdClaim = User.FindFirst("userId") .Value; // Extrai dados do token
    // ... l�gica
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
- **Soft Delete**: Campo `IsActive` para exclus�o l�gica
- **Auditing**: Campos `CreatedAt` e `UpdatedAt` atualizados automaticamente
- **Email �nico**: �ndice �nico no campo Email
- **Password Hash**: Senhas criptografadas com BCrypt

##    API Documentation

### Endpoints Dispon�veis

#### POST `/api/Authentication/login`
Autentica um usu�rio existente.

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
  "name": "Nome do Usu�rio",
  "expiresAt": "2024-01-15T10:30:00Z"
}
```

#### POST `/api/Authentication/register`
Registra um novo usu�rio.

**Request Body:**
```json
{
  "name": "Nome do Usu�rio",
  "email": "user@example.com",
  "password": "123456",
  "confirmPassword": "123456"
}
```

#### GET `/api/Authentication/me`   
Retorna dados do usu�rio autenticado (requer JWT).

#### DELETE `/api/Authentication/user`   
Exclui (soft delete) o usu�rio autenticado (requer JWT).

##    Testando a API

### Usando cURL

**1. Registrar usu�rio:**
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
1. Execute a aplica��o
2. Acesse `https://localhost:7xxx/swagger`
3. Use o bot�o "Authorize" para configurar o JWT
4. Teste os endpoints diretamente na interface

##    Pr�ximos Passos (Para Aprendizado)

Para expandir este projeto e aprender mais conceitos:

1. **Logging**: Implementar Serilog para logs estruturados
2. **Validation**: FluentValidation para valida��es mais robustas
3. **Caching**: Redis para cache de dados
4. **Testing**: Unit tests com xUnit e Moq
5. **Health Checks**: Monitoramento de sa�de da aplica��o
6. **Rate Limiting**: Controle de taxa de requisi��es
7. **CORS**: Configura��o para aplica��es frontend
8. **Docker**: Containeriza��o da aplica��o
9. **CI/CD**: Pipeline de deploy automatizado

##    Contribui��o

Este � um projeto educacional! Contribui��es s�o bem-vindas:

1. Fork o projeto
2. Crie uma branch para sua feature (`git checkout -b feature/AmazingFeature`)
3. Commit suas mudan�as (`git commit -m 'Add some AmazingFeature'`)
4. Push para a branch (`git push origin feature/AmazingFeature`)
5. Abra um Pull Request

##    Licen�a

Este projeto est� sob a licen�a MIT. Veja o arquivo `LICENSE` para mais detalhes.

---

**   Desenvolvido para fins educacionais - Demonstrando Arquitetura Limpa em .NET 8**