using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PCM.Api.Data;
using PCM.Api.Models;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://0.0.0.0:" + (Environment.GetEnvironmentVariable("PORT") ?? "8080"));


// ================= DATABASE =================
builder.Services.AddDbContext<AppDbContext>(options =>
	options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


// ================= IDENTITY =================
builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
	options.Password.RequireDigit = false;
	options.Password.RequireUppercase = false;
	options.Password.RequireLowercase = false;
	options.Password.RequireNonAlphanumeric = false;
	options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();


// ================= JWT AUTH =================
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
		ValidIssuer = builder.Configuration["Jwt:Issuer"],
		ValidAudience = builder.Configuration["Jwt:Audience"],
		IssuerSigningKey = new SymmetricSecurityKey(
			Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
	};
});


// ================= CORS =================
builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowAll", policy =>
		policy.AllowAnyOrigin()
			  .AllowAnyHeader()
			  .AllowAnyMethod());
});


// ================= JSON LOOP FIX =================
builder.Services.AddControllers()
	.AddJsonOptions(options =>
	{
		options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
	});


// ================= SWAGGER + JWT =================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
	options.SwaggerDoc("v1", new OpenApiInfo
	{
		Title = "PCM API",
		Version = "v1"
	});

	options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
	{
		Name = "Authorization",
		Type = SecuritySchemeType.Http,
		Scheme = "bearer",
		BearerFormat = "JWT",
		In = ParameterLocation.Header,
		Description = "Enter: Bearer {your JWT token}"
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
			Array.Empty<string>()
		}
	});
});

var app = builder.Build();


// ================= AUTO MIGRATE DATABASE =================
using (var scope = app.Services.CreateScope())
{
	var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
	db.Database.Migrate();
}


// ================= MIDDLEWARE =================
app.UseCors("AllowAll");

// Bật Swagger trên cả Production (Render)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
	c.SwaggerEndpoint("/swagger/v1/swagger.json", "PCM API V1");
	c.RoutePrefix = "swagger";
});

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


// ================= AUTO CREATE ROLES + ADMIN =================
using (var scope = app.Services.CreateScope())
{
	var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
	var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

	string[] roles = { "Admin", "Treasurer", "Referee", "Member" };

	foreach (var role in roles)
		if (!await roleManager.RoleExistsAsync(role))
			await roleManager.CreateAsync(new IdentityRole(role));

	var adminEmail = "admin@pcm.com";
	var adminUser = await userManager.FindByEmailAsync(adminEmail);

	if (adminUser == null)
	{
		var user = new AppUser
		{
			UserName = adminEmail,
			Email = adminEmail,
			FullName = "Super Admin"
		};

		await userManager.CreateAsync(user, "Admin@123");
		await userManager.AddToRoleAsync(user, "Admin");
	}
}
app.MapGet("/", () => "PCM API is running 🚀");

app.Run();
