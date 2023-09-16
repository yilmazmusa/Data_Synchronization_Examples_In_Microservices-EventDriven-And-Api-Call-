using Employee.API.Models.Entities;
using Employee.API.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



builder.Services.AddSingleton<MongoDBService>();

#region MongoDB'ye Seed Data Ekleme
using IServiceScope scope = builder.Services.BuildServiceProvider().CreateScope();
MongoDBService mongoDBService = scope.ServiceProvider.GetService<MongoDBService>();
var collection = mongoDBService.GetCollection<Employee.API.Models.Entities.Employee>();

if (!collection.FindSync(s => true).Any())
{
    await collection.InsertOneAsync(new() { PersonId = "6505f6876b0fdd9892aa75b1", Name = "Ahmet", Department = "Çiftçi" });
    await collection.InsertOneAsync(new() { PersonId = "6505f6876b0fdd9892aa75b2", Name = "Nebahat", Department = "Ev Hanýmý" });
    await collection.InsertOneAsync(new() { PersonId = "6505f6876b0fdd9892aa75b3", Name = "Ýbrahim", Department = "Patron" });
    await collection.InsertOneAsync(new() { PersonId = "6505f68b6b0fdd9892aa75b4", Name = "Samet", Department = "Tüccar" });
    await collection.InsertOneAsync(new() { PersonId = "6505f68b6b0fdd9892aa75b5", Name = "Betül", Department = "Öðretmen" });
    await collection.InsertOneAsync(new() { PersonId = "6505f68c6b0fdd9892aa75b6", Name = "Musa", Department = "Yazýlým" });
}
#endregion



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("update/{personId}/{newName}", async (
    [FromRoute] string personId,
    [FromRoute] string newName,
    MongoDBService mongoDBService) =>
{
    var employees = mongoDBService.GetCollection<Employee.API.Models.Entities.Employee>();
    Employee.API.Models.Entities.Employee employee = await (await employees.FindAsync(e => e.PersonId == personId)).FirstOrDefaultAsync();
    employee.Name = newName;
    await employees.FindOneAndReplaceAsync(p => p.Id == employee.Id, employee);
    return true;
});


//Employee.API.Models.Entities.Employee

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
