using FluentValidation;
using Interfaces.Base;
using System;
using Microsoft.EntityFrameworkCore;
using NLog.LayoutRenderers;
using Services;
using Interfaces.ViewModels.UserVM;
using Services.Validator.UserValidator;
using Microsoft.EntityFrameworkCore.Migrations.Internal;
using Interfaces.Interfaces;
using Services.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Interfaces.ViewModels.ToDoVM;
using Services.Validator.ToDoListValidator;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DataContext>(op =>
                                op.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), m =>
                                {
                                    m.MigrationsAssembly("ToDoListApp");
                                    m.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                                }));

builder.Services.AddScoped<IValidator<SaveUserViewModel>, UserValidator>();
builder.Services.AddScoped<IValidator<SaveToDoViewModel>, ToDoListValidator>();
builder.Services.AddScoped<ICoreBase, CoreBaseService>();
builder.Services.AddScoped<IUser, UserService>();
builder.Services.AddScoped<IToDo, ToDoService>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["JWT:ValidAudience"],
                    ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]))
                };
            });

builder.Services.AddCors(p => p.AddPolicy("corsapp", builder =>
{
    builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
}));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// For choose folder to show error log in
LayoutRenderer.Register("basedir", (logEvent) => app.Environment.WebRootPath);

app.UseCors("corsapp");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
