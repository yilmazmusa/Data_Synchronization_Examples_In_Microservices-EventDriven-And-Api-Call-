using MassTransit;
using ServiceB.Services;
using Shared.Events;
using ServiceB.Consumers;
using Shared;
using ServiceB.Models.Entities;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



//ServiceA den gelen istek ServiceB de burda karþýlanýyor.
builder.Services.AddSingleton<MongoDBService>();
builder.Services.AddMassTransit(configurator =>
{
    configurator.AddConsumer<UpdatePersonNameEventConsumer>();

    configurator.UsingRabbitMq((context, _configurator) => 
    {
        _configurator.Host(builder.Configuration["RabbitMQ"]);

        _configurator.ReceiveEndpoint(RabbitMQSettings.ServiceB_UpdatePersonNameEvetQueue, e => e.ConfigureConsumer<UpdatePersonNameEventConsumer>(context));
    });
});

#region MongoDB'ye Seed Data Ekleme
using IServiceScope scope = builder.Services.BuildServiceProvider().CreateScope();
MongoDBService mongoDBService = scope.ServiceProvider.GetService<MongoDBService>();
var collection = mongoDBService.GetCollection<Employee>();
if (!collection.FindSync(s => true).Any())
{
    await collection.InsertOneAsync(new() { PersonId = "65063a25db6f2873fdfbfc6f", Name = "GenCay", Department = "Yazilim" });
    await collection.InsertOneAsync(new() { PersonId = "65063a25db6f2873fdfbfc70", Name = "Hilmi", Department = "Memur" });
    await collection.InsertOneAsync(new() { PersonId = "65063a25db6f2873fdfbfc71", Name = "Suayip", Department = "Tamirci" });
    await collection.InsertOneAsync(new() { PersonId = "65063a25db6f2873fdfbfc72", Name = "Rifat", Department = "Muhabbet Sohbet" });
    await collection.InsertOneAsync(new() { PersonId = "65063a25db6f2873fdfbfc73", Name = "Muhsin", Department = "Taksici" });
    await collection.InsertOneAsync(new() { PersonId = "65063a25db6f2873fdfbfc74", Name = "Muiddin", Department = "Muhasebe" });
}
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}







app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
