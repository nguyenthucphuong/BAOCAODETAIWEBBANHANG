using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SaleApi.Models.Entity;
using SaleApi.Models.Services;
using System.Text;
using System.Text.Json.Serialization;
using AutoMapper;
using SaleApi.Models.Users;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Cấu hình Mapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddDbContext<SaleApiContext>(
    c => c.UseSqlServer(builder.Configuration.GetConnectionString("ketnoi"))
);

// Add services JWT tạo token
builder.Services.AddScoped<JwtService>();

// Add services upload
builder.Services.AddScoped<UploadService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// khai báo để dùng Ajax
builder.Services.AddCors(o => o.AddPolicy("CorsPolicy", builder =>
{
    builder.WithOrigins("http://localhost:5068")
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
}));

builder.Services.AddControllers();

// JWT
var jwtKey = builder.Configuration["Jwt:Key"];
if (jwtKey == null)
{
    throw new Exception("Jwt:Key is not configured");
}
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(p =>
    {
        p.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
			ClockSkew = TimeSpan.Zero
		};
    });

// Khai báo xác thực không cho phép Role là Default truy cập
builder.Services.AddAuthorization(options =>
{
	options.AddPolicy("NonDefault", policy => policy.RequireClaim(ClaimTypes.Role, "Admin", "Manager", "User", "Customer"));
});


builder.Services.AddMvc();

// Khai báo xác thực Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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

builder.Services.AddRouting();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
// Khai báo xác thực Swagger
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
});


app.UseRouting();
// khai báo để dùng Ajax
app.UseCors("CorsPolicy");
//khai báo hiển thị hình ảnh
app.UseStaticFiles();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
// hiển thị lỗi ở http://localhost:5260/swagger/v1/swagger.json
app.UseDeveloperExceptionPage();
app.Run();

