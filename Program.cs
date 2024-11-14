// TRONG ỨNG DỤNG NÀY CHÚNG TA SẼ TÌM HIỂU VỀ MINIMAL APIS
// Đây là nơi chính thức chạy của ứng dụng nơi tập trung như gọi là sảnh chính của ứng dụng
using Microsoft.OpenApi.Models;
//using PizzaStore.DB;
using PizzaStore.Models;
using Microsoft.EntityFrameworkCore;

// builder => Dòng này tạo một WebApplicationBuilder, đối tượng này chịu trách nhiệm cấu hình tất cả các dịch vụ cần thiết cho ứng dụng của bạn.
var builder = WebApplication.CreateBuilder(args);
// Thực hiện kết nối database 
var connectionString = builder.Configuration.GetConnectionString("Pizzas") ?? "Data Source=Pizzas.db";

// Thêm cấu hình dịch vụ CORS | phần trên là cũ chưa được cấu hình CORS  
// builder.Services.AddCors(options => {});
// Phần cấu hình CORS mới
// 1) define a unique string
string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

// 2) define allowed domains, in this case "http://example.com" and "*" = all
//    domains, for testing purposes only.
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
      builder =>
      {
          // Trong phần cho phép mở CORS chúng ta cho phép http://example.com và tất cả (*) nhưng (*) khá nguy hiểm và chỉ dùng cho demo
          builder.WithOrigins(
            "http://example.com", "*");
      });
});

// Sử dụng và cấu hình dịch vụ swagger
builder.Services.AddEndpointsApiExplorer();
// Chúng ta sẽ cmt lại phần sử dụng Inmemory để trực tiếp đưa vào SQLite cho dễ demo.
// builder.Services.AddDbContext<PizzaDb>(options => options.UseInMemoryDatabase("items"));

// kết nối SQL DB 
builder.Services.AddSqlite<PizzaDb>(connectionString);

builder.Services.AddSwaggerGen(c =>
{
  c.SwaggerDoc("v1", new OpenApiInfo { Title = "Todo API", Description = "Keep track of your tasks", Version = "v1" });
});
// app => Dòng này sử dụng builder để tạo ra một đối tượng WebApplication, đây chính là ứng dụng thực tế bạn sẽ chạy.
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

// Sử dụng CORS dựa trên string CORS đã khai báo
app.UseCors(MyAllowSpecificOrigins);

// Thực hiện tạo các route cho CRUD Mininal APIs theo hướng căn bản models
//app.MapGet("/pizzas/{id}", (int id) => PizzaDB.GetPizza(id));
//app.MapGet("/pizzas", () => PizzaDB.GetPizzas());
//app.MapPost("/pizzas", (Pizza pizza) => PizzaDB.CreatePizza(pizza));
//app.MapPut("/pizzas", (Pizza pizza) => PizzaDB.UpdatePizza(pizza));
//app.MapDelete("/pizzas/{id}", (int id) => PizzaDB.RemovePizza(id));

// Thực hiện tạo các route cho Minimal APIs theo EF

// GET listing
app.MapGet("/pizzas", async (PizzaDb db) => await db.Pizzas.ToListAsync());

// GET item by id
app.MapGet("/pizzas/{id}", async (PizzaDb db, int id) => await db.Pizzas.FindAsync(id));

// POST Create item
app.MapPost("/pizzas", async (PizzaDb db, Pizza pizza) =>
{
    await db.Pizzas.AddAsync(pizza);
    await db.SaveChangesAsync();
    return Results.Created($"/pizza/{pizza.Id}", pizza);
});

// PUT Update item by id
app.MapPut("/pizzas/{id}", async (PizzaDb db, Pizza updatepizza, int id) =>
{
      var pizza = await db.Pizzas.FindAsync(id);
      if (pizza is null) return Results.NotFound();
      pizza.Name = updatepizza.Name;
      pizza.Description = updatepizza.Description;
      await db.SaveChangesAsync();
      return Results.NoContent();
});

// DELETE Delete item by id
app.MapDelete("/pizzas/{id}", async (PizzaDb db, int id) =>
{
   var pizza = await db.Pizzas.FindAsync(id);
   if (pizza is null)
   {
      return Results.NotFound();
   }
   db.Pizzas.Remove(pizza);
   await db.SaveChangesAsync();
   return Results.Ok();
});

// Thực hiện thêm swagger vào add và swagger UI 
// Lưu ý vơi trường hợp isDevelopment chúng ta phải chạy lện cmd: dotnet run để chạy mt dev mới kích hoạt được swagger
// Muốn kích hoạt với mt thường ta phải đưa ra ngoài đk if.
if (app.Environment.IsDevelopment())
{
   app.UseSwagger();
   app.UseSwaggerUI(c =>
    {
      c.SwaggerEndpoint("/swagger/v1/swagger.json", "Todo API V1");
    });
} // end of if (app.Environment.IsDevelopment()) block

// Phần này thể hiện việc app chạy
app.Run();
