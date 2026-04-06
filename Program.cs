var builder = WebApplication.CreateBuilder(args);


builder.Services.AddOpenApi();
builder.Services.AddRazorPages();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}


app.UseStaticFiles();
app.UseHttpsRedirection();
app.MapRazorPages();


app.Run();
