using RaceDayMVC.Services;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<AzureBlobService>();

builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.Name = ".RaceDay.Session";
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.IdleTimeout = TimeSpan.FromMinutes(30);
});
builder.Services.AddHttpClient<RaceDayApiClient>((sp, client) =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var baseUrl = configuration["Api:BaseUrl"];
    if (string.IsNullOrWhiteSpace(baseUrl))
        throw new InvalidOperationException("Api:BaseUrl must be configured in appsettings.json.");
    client.BaseAddress = new Uri(baseUrl);
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
