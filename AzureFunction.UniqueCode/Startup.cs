using AzureFunction.UniqueCode;
using AzureFunction.UniqueCode.DataAccess;
using AzureFunction.UniqueCode.Domain;
using AzureFunction.UniqueCode.Entities.Dto;
using FluentValidation;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

[assembly: FunctionsStartup(typeof(Startup))]
namespace AzureFunction.UniqueCode
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddValidatorsFromAssemblyContaining(typeof(Startup));
            builder.Services.AddAutoMapper(Assembly.GetAssembly(typeof(MappingProfile)));
            builder.Services.AddScoped<IDomain, Domain.Domain>();
            builder.Services.AddScoped<IRepository, Repository>();
            builder.Services.AddSingleton<ICommandText, CommandText>();
            builder.Services.AddSingleton(typeof(ICosmosRepository<>), typeof(CosmosRepository<>));
        }
    }
}
