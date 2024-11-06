// TRONG ỨNG DỤNG NÀY CHÚNG TA SẼ TÌM HIỂU VỀ MINIMAL APIS
// Đây là nơi chính thức chạy của ứng dụng nơi tập trung như gọi là sảnh chính của ứng dụng
using Microsoft.OpenApi.Models;
using PizzaStore.DB;
// builder => Dòng này tạo một WebApplicationBuilder, đối tượng này chịu trách nhiệm cấu hình tất cả các dịch vụ cần thiết cho ứng dụng của bạn.
var builder = WebApplication.CreateBuilder(args);

// Sử dụng dịch vụ CORS 
builder.Services.AddCors(options => {});

// Sử dụng và cấu hình dịch vụ swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
  c.SwaggerDoc("v1", new OpenApiInfo { Title = "Todo API", Description = "Keep track of your tasks", Version = "v1" });
});
// app => Dòng này sử dụng builder để tạo ra một đối tượng WebApplication, đây chính là ứng dụng thực tế bạn sẽ chạy.
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.UseCors("some unique string");

// Thực hiện tạo các route cho CRUD Mininal APIs
app.MapGet("/pizzas/{id}", (int id) => PizzaDB.GetPizza(id));
app.MapGet("/pizzas", () => PizzaDB.GetPizzas());
app.MapPost("/pizzas", (Pizza pizza) => PizzaDB.CreatePizza(pizza));
app.MapPut("/pizzas", (Pizza pizza) => PizzaDB.UpdatePizza(pizza));
app.MapDelete("/pizzas/{id}", (int id) => PizzaDB.RemovePizza(id));
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
