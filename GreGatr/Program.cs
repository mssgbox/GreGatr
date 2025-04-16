using Microsoft.AspNetCore.Hosting;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();


// Subscribe to the unobserved task exception event globally
TaskScheduler.UnobservedTaskException += (sender, e) =>
{
    // Get the logger from DI container
    var logger = app.Services.GetRequiredService<ILogger<Program>>();

    // Log the exception
    logger.LogError(e.Exception, "An unobserved task exception occurred.");

    // Mark the exception as observed to prevent termination
    e.SetObserved();
};

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // Show detailed error pages in dev
}
else
{
    app.UseExceptionHandler("/Home/Error"); // Handle exceptions and redirect in production
    app.UseHsts();  // Apply HTTP Strict Transport Security (HSTS) in production
}
// Configure the HTTP request pipeline.


app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
