using API.Modelos;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Adicionar o DbContext para injeção de dependência
builder.Services.AddDbContext<BibliotecaDbContext>();

// 2. Registrar os serviços de Controllers
builder.Services.AddControllers();

var app = builder.Build();

// 3. Mapear os Controllers para que os endpoints fiquem disponíveis
app.MapControllers();

app.Run();