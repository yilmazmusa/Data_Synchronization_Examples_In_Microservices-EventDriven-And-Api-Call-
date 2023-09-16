using MassTransit;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using ServiceA.Models.Entities;
using ServiceA.Services;
using Shared.Events;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<MongoDBService>();
builder.Services.AddMassTransit(configurator =>
{
    configurator.UsingRabbitMq((context, _configurator) =>
    {
        _configurator.Host(builder.Configuration["RabbitMQ"]);
    });
});





#region MongoDB'ye Seed Data Ekleme
using IServiceScope scope = builder.Services.BuildServiceProvider().CreateScope();
MongoDBService mongoDBService = scope.ServiceProvider.GetService<MongoDBService>();
var collection = mongoDBService.GetCollection<Person>();
if (!collection.FindSync(s => true).Any())
{
    await collection.InsertOneAsync(new() { Name = "Gençay" });
    await collection.InsertOneAsync(new() { Name = "Hilmi" });
    await collection.InsertOneAsync(new() { Name = "Þuayip" });
    await collection.InsertOneAsync(new() { Name = "Rakýf" });
    await collection.InsertOneAsync(new() { Name = "Rýfký" });
    await collection.InsertOneAsync(new() { Name = "Muiddin" });
}
#endregion

var app = builder.Build();




// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}





//ServiceA dan ServiceB ya istek atıyoruz.Yani önce ServiceA da değişiklik yapıyoruz o değişikliği ServiceB ye de haber veriyoruz Veri Senkronizasyonu olsun veri tutarsızlığı olmasın diye.
app.MapGet("updateName/{id}/{newName}", async (
[FromRoute] string id,
[FromRoute] string newName,
MongoDBService mongoDBService,
IPublishEndpoint publishEndpoint) =>
{
    var persons = mongoDBService.GetCollection<Person>();
    Person person = await (await persons.FindAsync(p => p.Id == ObjectId.Parse(id))).FirstOrDefaultAsync();
    person.Name = newName;
    await persons.FindOneAndReplaceAsync(p => p.Id == ObjectId.Parse(id), person);

    UpdatedPersonNameEvent updatePesonNameEvent = new()
    {
        PersonId = id,
        NewName = newName,
    };

    await publishEndpoint.Publish(updatePesonNameEvent);
});






app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
