using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MINIMAL_API___DIO.Domain.DTOs;
using MINIMAL_API___DIO.Domain.Entities;
using MINIMAL_API___DIO.Domain.Enums;
using MINIMAL_API___DIO.Domain.ModelViews;
using MINIMAL_API___DIO.Domain.Services;
using MINIMAL_API___DIO.Dominio.DTOs;
using MINIMAL_API___DIO.Dominio.Entidades;
using MINIMAL_API___DIO.Dominio.Servicos;
using MINIMAL_API___DIO.Infraestrutura.Db;
using MINIMAL_API___DIO.Infrastructure.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.Intrinsics.Arm;
using System.Security.Claims;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;

#region Builder
var builder = WebApplication.CreateBuilder(args);

// Acionando autenticação via token JWT
var key = builder.Configuration.GetSection("Jwt").ToString();
if (string.IsNullOrEmpty(key)) key = "123456";

builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(option =>
{
    option.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateLifetime = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddAuthorization();

// Adicionando Swagger para documentação da API
builder.Services.AddEndpointsApiExplorer();

// Configurando o Swagger para utilizar autenticação via token JWT
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insira o token JWT aqui"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
             new OpenApiSecurityScheme
             {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
             },
             new string[] {}
        }
    });
});


// Adicionando o serviço de admin no escopo da aplicação
builder.Services.AddScoped<IAdminService, AdminService>();

// Adicionando o serviço de veículo no escopo da aplicação
builder.Services.AddScoped<IVeiculoService, VeiculoService>();

// Adicionando o serviço de configuração do banco de dados
builder.Services.AddDbContext<DbContexto>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Database"));
});


var app = builder.Build();
#endregion

#region Home
// Alterando a rota padrão para acessar o Swagger
app.MapGet("/", () => Results.Json(new Home())).AllowAnonymous().WithTags("Home");
#endregion

#region Admins

// Método para gerar token JWT  
string GenerateTokenJwt(Admin admin)
{
    Console.WriteLine(key);
    if (string.IsNullOrEmpty(key)) return string.Empty;

    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

    // Criptografia do token
    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

    var claims = new List<Claim>()
    {
        new Claim("Email", admin.Email),
        new Claim("Perfil", admin.Perfil),
        new Claim(ClaimTypes.Role, admin.Perfil)
    };


    var token = new JwtSecurityToken(
        claims: claims,
        expires: DateTime.Now.AddDays(1),
        signingCredentials: credentials      
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
}

// Endpoint para realizar o login do administrador utilizando o DTO
app.MapPost("/admins/login", ([FromBody] LoginDTO loginDTO, IAdminService adminService) =>
{
    // Implementando a autenticação do administrador
    var admin = adminService.Login(loginDTO);

    if (admin != null)
    {
        string token = GenerateTokenJwt(admin);

        return Results.Ok(new AdminLogged
        {
            Email = admin.Email,
            Perfil = admin.Perfil,
            Token = token
        });
    }
    else
    {
        return Results.Unauthorized();
    }
    // Deixando a rota autorizada
}).AllowAnonymous().WithTags("Admins");

// CREATE
app.MapPost("/admin", ([FromBody] AdminDTO adminDTO, IAdminService adminService) =>
{
    // Realizando a validação dos dados do veículo
    var validation = new ValidationErrors
    {
        Messages = new List<string>()
    };

    if (string.IsNullOrEmpty(adminDTO.Email))
    {
        validation.Messages.Add("O Email não pode ser vazio!"); 
    }

    if (string.IsNullOrEmpty(adminDTO.Senha))
    {
        validation.Messages.Add("a senha não pode ser vazia!");
    }

    if (adminDTO.Perfil == null)
    {
        validation.Messages.Add("O Perfil não pode ser vazio!");
    }

    if (validation.Messages.Count > 0)
    {
        return Results.BadRequest(validation);
    }

    var admin = new Admin
    {
        Email = adminDTO.Email,
        Senha = adminDTO.Senha,
        Perfil = adminDTO.Perfil.ToString() ?? Perfil.Editor.ToString()
    };

    adminService.Create(admin);

    return Results.Created($"/admin/{admin.Id}", new AdminModelView
    {
        Id = admin.Id,
        Email = admin.Email,
        Perfil = Enum.Parse<Perfil>(admin.Perfil)
    });

}).RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" }).WithTags("Admins");

// GET ALL
app.MapGet("/admins", ([FromQuery] int? page, IAdminService adminService) =>
{
    var adminsList = new List<AdminModelView>();
    var admins = adminService.GetAll(page);

    foreach (var adm in admins)
    {
        adminsList.Add(new AdminModelView
        {
            Id = adm.Id,
            Email = adm.Email,
            Perfil = Enum.Parse<Perfil>(adm.Perfil)
        });
    }
    return Results.Ok(admins);
}).RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" }).WithTags("Admins");

// GET BY ID
app.MapGet("/admin/{id}", ([FromRoute] int id, IAdminService adminService) =>
{
    var admin = adminService.GetById(id);
    if (admin == null) return Results.NotFound();
    return Results.Ok(new AdminModelView
    {
        Id = admin.Id,
        Email = admin.Email,
        Perfil = Enum.Parse<Perfil>(admin.Perfil)
    });
}).RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" }).WithTags("Admins");
#endregion

#region Veiculos

// Método para validar os dados do veículo
ValidationErrors validationDTO(VeiculoDTO veiculoDTO)
{
    var validation = new ValidationErrors
    {
        Messages = new List<string>()
    }; 

    if (string.IsNullOrEmpty(veiculoDTO.Nome))
    {
        validation.Messages.Add("O Nome não pode ser vazio!");
    }

    if (string.IsNullOrEmpty(veiculoDTO.Marca))
    {
        validation.Messages.Add("A Marca não pode ficar em branco!");
    }

    if (veiculoDTO.Ano < 1950)
    {
        validation.Messages.Add("Veículo muito antigo, aceito somente anos superiores a 1950!");
    }

    return validation;
}


// Post
app.MapPost("/veiculos", ([FromBody] VeiculoDTO veiculoDTO, IVeiculoService veiculoService) => {

    // Realizando a validação dos dados do veículo
    var validation = validationDTO(veiculoDTO);

    if (validation.Messages.Count > 0)
    {
        return Results.BadRequest(validation);
    }

    var veiculo = new Veiculo
    {
        Nome = veiculoDTO.Nome,
        Marca = veiculoDTO.Marca,
        Ano = veiculoDTO.Ano
    };

    veiculoService.Create(veiculo);

    return Results.Created($"/veiculo/{veiculo.Id}", veiculo);
})
.RequireAuthorization(new AuthorizeAttribute { Roles = "Adm, Editor" }).WithTags("Veículo");

// GET
app.MapGet("/veiculos", ([FromQuery] int? page, IVeiculoService veiculoService) => {
    var veiculos = veiculoService.GetAll(page);
    return Results.Ok(veiculos);
}).RequireAuthorization().WithTags("Veículo");

// GET BY ID
app.MapGet("/veiculos/{id}", ([FromRoute] int id, IVeiculoService veiculoService) => {
    var veiculo = veiculoService.GetById(id);
    if (veiculo == null) return Results.NotFound();
    return Results.Ok(veiculo);
}).RequireAuthorization(new AuthorizeAttribute { Roles = "Adm, Editor" }).WithTags("Veículo");

// Update
app.MapPut("/veiculos/{id}", ([FromRoute] int id, VeiculoDTO veiculoDTO, IVeiculoService veiculoService) => {
    var veiculo = veiculoService.GetById(id);

    if (veiculo == null) return Results.NotFound();

    var validation = validationDTO(veiculoDTO);

    if (validation.Messages.Count > 0)
    {
        return Results.BadRequest(validation);
    }

    veiculo.Nome = veiculoDTO.Nome;
    veiculo.Marca = veiculoDTO.Marca;
    veiculo.Ano = veiculoDTO.Ano;

    veiculoService.Update(veiculo);
    return Results.Ok(veiculo);
}).RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" }).WithTags("Veículo");

// Delete
app.MapDelete("/veiculos/{id}", ([FromRoute] int id, IVeiculoService veiculoService) => {
    var veiculo = veiculoService.GetById(id);

    if (veiculo == null) return Results.NotFound();

    veiculoService.Delete(veiculo);
    return Results.NoContent();
}).RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" }).WithTags("Veículo");

#endregion

#region App
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.Run();
#endregion