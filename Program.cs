using System.Text;
using DevTalles.Ecommerce.WebAPI.Constants;
using DevTalles.Ecommerce.WebAPI.Data;
using DevTalles.Ecommerce.WebAPI.Repository;
using DevTalles.Ecommerce.WebAPI.Repository.IRepository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var dbConnectionString = builder.Configuration.GetConnectionString("ConnectionSql");
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(dbConnectionString));
builder.Services.AddResponseCaching(options =>
{
    options.MaximumBodySize = 1024 * 1024; // 1 MB
    options.UseCaseSensitivePaths = true;
});

builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddAutoMapper(_ => { }, typeof(Program).Assembly);
var secretKey = builder.Configuration["ApiSettings:SecretKey"] ??
                throw new InvalidOperationException("JWT secret key is not configured.");
builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ValidateIssuer = false,
        ValidateAudience = false,
    };
});
builder.Services.AddControllers(options =>
{
    options.CacheProfiles.Add(CacheProfiles.DefaultCacheProfile,
        CacheProfiles.Profiles[CacheProfiles.DefaultCacheProfile]);
    options.CacheProfiles.Add(CacheProfiles.Default20CacheProfile,
        CacheProfiles.Profiles[CacheProfiles.Default20CacheProfile]);
    options.CacheProfiles.Add(CacheProfiles.NoCacheProfile, CacheProfiles.Profiles[CacheProfiles.NoCacheProfile]);
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
    {
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "JWT Authorization header using the Bearer scheme. Example: \n\r\n\r" +
                          "\",Authorization: Bearer {token}\" \n\r\n\r" +
                          "Enter then your token in the text input below.\n\r\n\r" +
                          "Example: \"eyJhbGciOi\"",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer"
        });
        options.AddSecurityRequirement(new OpenApiSecurityRequirement()
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
                new List<string>()
            }
        });
    }
);
builder.Services.AddCors(options =>
{
    options.AddPolicy(PolicyName.AllowSpecificOrigin, policyBuilder =>
    {
        policyBuilder.WithOrigins("*")
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors(PolicyName.AllowSpecificOrigin);

app.UseResponseCaching();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();