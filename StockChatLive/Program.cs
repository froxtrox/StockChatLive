using StockChatLive.Hubs;
using StockChatLive.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddSignalR(options =>
{
    // Maximum size of incoming messages the server will buffer
    options.MaximumReceiveMessageSize = 64 * 1024; // 64 KB
});

builder.Services.AddSingleton<IRealTimeStockProvider,FakeRealTimeStockProvider>();
builder.Services.AddHostedService<StockPriceHostedService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();


app.MapHub<StockListingHub>("/stocklisting", options =>
{
    options.ApplicationMaxBufferSize = 64 * 1024; // 64 KB
    options.TransportMaxBufferSize = 64 * 1024;   // 64 KB
});

app.MapHub<LiveChatHub>("/livechat", options =>
{
    options.ApplicationMaxBufferSize = 64 * 1024; // 64 KB
    options.TransportMaxBufferSize = 64 * 1024;   // 64 KB
});
app.Run();
 