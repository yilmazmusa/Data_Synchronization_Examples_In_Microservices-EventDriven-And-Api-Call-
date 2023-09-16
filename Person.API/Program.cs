using Amazon.Runtime;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using Person.API.Models.Entities;
using Person.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddSingleton<MongoDBService>();
builder.Services.AddHttpClient("Employee.API", httpClient =>
{
    httpClient.BaseAddress = new Uri("https://localhost:7288");
});


#region MongoDB ye Seed Data Ekleme

using IServiceScope scope = builder.Services.BuildServiceProvider().CreateScope();
MongoDBService mongoDBService = scope.ServiceProvider.GetService<MongoDBService>();

var collection = mongoDBService.GetCollection<Person.API.Models.Entities.Person>(); //Person tablosundaki tüm verileri collection a attýk.
if (!collection.FindSync(s => true).Any())
{
    await collection.InsertOneAsync(new() { Name = "Ahmet" });
    await collection.InsertOneAsync(new() { Name = "Nebahat" });
    await collection.InsertOneAsync(new() { Name = "Ýbrahim" });
    await collection.InsertOneAsync(new() { Name = "Samet" });
    await collection.InsertOneAsync(new() { Name = "Betül" });
    await collection.InsertOneAsync(new() { Name = "Musa" });

}

#endregion




var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/{id}/{newName}", async (
    [FromRoute] string id,
    [FromRoute] string newName,
    MongoDBService mongoDBService,
    IHttpClientFactory httpClientFactory) =>
{
    var httpClient = httpClientFactory.CreateClient("Employee.API");

    var persons = mongoDBService.GetCollection<Person.API.Models.Entities.Person>();

    Person.API.Models.Entities.Person person = await (await persons.FindAsync(p => p.Id == ObjectId.Parse(id))).FirstOrDefaultAsync();
    person.Name = newName;
    await persons.FindOneAndReplaceAsync(p => p.Id == ObjectId.Parse(id), person);

    var httpResponseMessage = await httpClient.GetAsync($"update/{person.Id}/{person.Name}");
    if (httpResponseMessage.IsSuccessStatusCode)
    {
        var content = await httpResponseMessage.Content.ReadAsStringAsync();
        await Console.Out.WriteLineAsync(content);
    }
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
