using Microsoft.EntityFrameworkCore;
using Jewelry_auction_system.Data;
using JewelryAuction.Services.Services.Interfaces;
using JewelryAuction.Services.Services;
using JewelryAuction.Data.Repositories.Interfaces; // Make sure this is the correct namespace for your IProductRepository
using JewelryAuction.Data.Repositories; // And this for your ProductRepository
using Microsoft.OpenApi.Models;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using JewelryAuction.Services.Helpers;
using JewelryAuction.Services.Services.Background;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddConnections();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
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
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header

            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins",
        builder =>
        {
            builder.WithOrigins("http://localhost:3000") // Thay đổi URL này thành URL của front-end của bạn
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
});

builder.Services.AddScoped<IProductServices, ProductServices>();
builder.Services.AddScoped<IProductRepository, ProductsRepository>();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<IAuctionServices, AuctionsServices>();
builder.Services.AddScoped<IAuctionsRepository, AuctionsRepository>();

builder.Services.AddScoped<IImageServices, ImageService>();
builder.Services.AddScoped<IImgRepository, ImgRepository>();

builder.Services.AddScoped<IWalletRepository, WalletRepository>();
builder.Services.AddScoped<IWalletServices, WalletServices>();

builder.Services.AddScoped<IEmailRepository, EmailRepository>();
builder.Services.AddScoped<IEmailServices, EmailServices>();
builder.Services.AddScoped<EmailServices>();

builder.Services.AddScoped<ICategoriesRepository, CategoriesRepository>();
builder.Services.AddScoped<ICategoryServices, CategoriesServices>();

builder.Services.AddScoped<IProductCategoryRepository, ProductCategoryRepository>();
builder.Services.AddScoped<IProductCategoryServices, ProductCategoryServices>();

builder.Services.AddScoped<ICategoriesTypeRepository, CategoryTypeRepository>();
builder.Services.AddScoped<ICategoryTypeServices, CategoryTypeServices>();

builder.Services.AddScoped<IVnPayServices, VNPayServices>();

builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<ITransactionServices, TransactionServices>();

builder.Services.AddScoped<IBidRepository, BidRepository>();
builder.Services.AddScoped<IBidServices, BidServices>();

builder.Services.AddScoped<IFeedBackRepository, FeedBackRepository>();
builder.Services.AddScoped<IFeedBackServices, FeedBackServices>();

builder.Services.AddMemoryCache();
builder.Services.AddScoped<VerificationCodeManager>();

builder.Services.AddHostedService<AuctionCheckService>();

// Configure DbContext with SQL Server
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Configure JWT Authentication

var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key),
        RoleClaimType = "Role"  // Đảm bảo claim "Role" được nhận diện đúng

    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Member", policy => policy.RequireRole("Member"));

    options.AddPolicy("Seller", policy => policy.RequireRole("Seller"));

    options.AddPolicy("Staff", policy => policy.RequireRole("Staff"));

    options.AddPolicy("Manager", policy => policy.RequireRole("Manager"));

    options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));

    options.AddPolicy("AllRole", policy =>
        policy.RequireRole("Member", "Seller", "Staff", "Manager", "Admin"));
});

builder.Services.AddSingleton<JwtTokenHelper>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Jewelry Auction System API V1");
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowSpecificOrigins");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
