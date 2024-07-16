using Api.Services;
using Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddExceptionHandler<ExceptionHandler>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options => options
        .AddPolicy(name: "AllowFrontend", policyBuilder => policyBuilder
                .WithOrigins("http://localhost:4200")
                .AllowAnyMethod()
                .AllowAnyHeader()
        ));

builder.Services.AddPersistenceServices(builder.Configuration);
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<TokenService>();

var app = builder.Build();

app.UseExceptionHandler("/Error");
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowFrontend");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();