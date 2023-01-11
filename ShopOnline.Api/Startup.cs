using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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

            services.AddDbContextPool<ShopOnlineDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("ShopOnlineConnection"))
            );

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

            services.AddAuthorization(options =>
            {
                options.AddPolicy("CustomerOnly", policy => policy.RequireRole("Customer", "Admin"));
            });

            services.AddScoped<IProductsRepository, ProductRepository>();
            services.AddScoped<IShoppingCartRepository, ShoppingCartRepository>();
        }


        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors(police =>
                police.WithOrigins("http://localhost:7058", "https://localhost:7058")
                .AllowAnyMethod()
                .AllowAnyHeader()
                //.WithHeaders(HeaderNames.ContentType)  // т.к. web client в header присылает JWToken, поэтому используем AllowAnyHeader()
            );

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {;
                endpoints.MapControllers();
            });




            // HttpRequest. Получение данных запроса   https://metanit.com/sharp/aspnet6/2.5.php
            app.Run(async (context) =>
            {
                context.Response.ContentType = "text/html; charset=utf-8";
                var stringBuilder = new System.Text.StringBuilder("<table>");

                foreach (var header in context.Request.Headers)
                {
                    stringBuilder.Append($"<tr><td>{header.Key}</td><td>{header.Value}</td></tr>");
                }
                stringBuilder.Append("</table>");
                await context.Response.WriteAsync(stringBuilder.ToString());
            });

        }
    }
}