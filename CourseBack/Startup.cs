using CourseBack.Models;
using CourseBack.Repository;
using CourseBack.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Formatters.Json;
using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.Tokens;

namespace CourseBack
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var connection = Configuration.GetConnectionString("CourseWorkDatabase");

            services.AddDbContext<CourseWorkDBContext>(options => options.UseSqlServer(connection));
            services.AddControllers();
            services.AddMvc(options => options.EnableEndpointRouting = false);
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "CourseWorkAPI", Version = "v1" });
            });

            services.AddScoped<IUsersService, UsersService>();
            services.AddScoped<ISavedItemsService, SavedItemsService>();
            services.AddScoped<ISecurityService, SecurityService>();

            services.AddScoped<IRecognizedItemsRepository, RecognizedItemsRepository>();
            services.AddScoped<IUsersRepository, UsersRepository>();


            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                          // ��������, ����� �� �������������� �������� ��� ��������� ������
                          ValidateIssuer = true,
                          // ������, �������������� ��������
                          ValidIssuer = AuthOptions.ISSUER,

                          // ����� �� �������������� ����������� ������
                          ValidateAudience = true,
                          // ��������� ����������� ������
                          ValidAudience = AuthOptions.AUDIENCE,
                          // ����� �� �������������� ����� �������������
                          ValidateLifetime = true,

                          // ��������� ����� ������������
                          IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
                          // ��������� ����� ������������
                          ValidateIssuerSigningKey = true,
                    };
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Course work API");
                c.RoutePrefix = "swagger";
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
