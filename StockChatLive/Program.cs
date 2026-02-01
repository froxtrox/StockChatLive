using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using StockChatLive.Hubs;
using StockChatLive.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddControllers(); 

const int MaxBufferSize = 64 * 1024; // 64 KB
const int MaxParallelInvocations = 1;

builder.Services.AddSignalR(options =>
{
    // Maximum size of incoming messages the server will buffer
    options.MaximumReceiveMessageSize = MaxBufferSize;
    options.MaximumParallelInvocationsPerClient = MaxParallelInvocations;
    options.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
    options.KeepAliveInterval = TimeSpan.FromSeconds(15);
});
builder.Services.AddSingleton<IRealTimeStockProvider,SimulatedStockProvider>();
builder.Services.AddHostedService<StockPriceHostedService>();

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
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) && 
                (path.StartsWithSegments("/livechat") || path.StartsWithSegments("/stocklisting")))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapRazorPages();


app.MapHub<StockListingHub>("/stocklisting", options =>
{
    options.ApplicationMaxBufferSize = MaxBufferSize; 
    options.TransportMaxBufferSize = MaxBufferSize;   
});

app.MapHub<LiveChatHub>("/livechat", options =>
{
    options.ApplicationMaxBufferSize = MaxBufferSize;  
    options.TransportMaxBufferSize = MaxBufferSize;  
});

app.Run();

 