using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using ShopOnline.Api.Data;
using ShopOnline.Api.Entities;
using ShopOnline.Api.Repositories;
using ShopOnline.Api.Repositories.Contracts;
using ShopOnline.Api.Security;

namespace ShopOnline.Api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration) => Configuration = configuration;

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            services.AddSqlite<ShopOnlineDbContext>("Data Source=ShopOnline.db");
            // for SqlServer, instal package too  services.AddDbContextPool<ShopOnlineDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("ShopOnlineConnection")));

            services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<ShopOnlineDbContext>()
                .AddDefaultTokenProviders();

            TokenParameters tokenParameters = new();

            services.AddSingleton(tokenParameters);

            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        //RoleClaimType = "Role",

                        ValidIssuer = tokenParameters.Issuer,
                        ValidAudience = tokenParameters.Audience,
                        IssuerSigningKey = tokenParameters.GetSymmetricSecurityKey(),
                    };
                });

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 3;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
            });

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                            policy =>
                            {
                                policy.WithOrigins("http://localhost:7058", "https://localhost:7058")
                                      .AllowAnyHeader()
                                      .AllowAnyMethod();
                            });
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("CustomerOnly", policy => policy.RequireRole("Customer", "Admin"));
            });

            services.AddScoped<IProductsRepository, ProductRepository>();
            services.AddScoped<IShoppingCartRepository, ShoppingCartRepository>();
            services.AddScoped<UserRepository>();
        }


        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseCors();

            app.UseEndpoints(endpoints =>
            {;
                endpoints.MapControllers();
            });
        }
    }
}