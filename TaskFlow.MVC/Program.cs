using Microsoft.AspNetCore.HttpOverrides;
using TaskFlow.MVC.Services;

var builder = WebApplication.CreateBuilder(args);
var useHttpsRedirection = builder.Configuration.GetValue("UseHttpsRedirection", false);

builder.Services.AddControllersWithViews();
builder.Services.AddHealthChecks();
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

builder.Services.AddHttpClient("TaskFlowAPI", client =>
{
    client.BaseAddress = new Uri(
        builder.Configuration["ApiBaseUrl"]!
    );
});

builder.Services.AddSession();
builder.Services.AddHttpContextAccessor(); 
builder.Services.AddScoped<ApiClient>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<TaskService>();



var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseForwardedHeaders();

if (useHttpsRedirection)
{
    app.UseHttpsRedirection();
}

app.UseStaticFiles();

app.UseRouting();

app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapHealthChecks("/healthz");
app.MapGet("/readyz", () => Results.Ok(new { status = "ready" }));

app.Run();
