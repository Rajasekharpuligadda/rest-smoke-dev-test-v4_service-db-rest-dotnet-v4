using ServiceDbRestDotnetV4.Interfaces;
using System.Linq;
namespace ServiceDbRestDotnetV4.Core
{
    public class Bootstrap
    {
        private readonly IServiceCollection services;
        public Bootstrap(IServiceCollection services)
        {
            this.services = services;
        }   
        internal void RegisterDependencies()
        {
            this.RegisterTransient();
            this.RegisterSingleton();
            this.RegisterPerRequest();
        }

        private void RegisterTransient()
        {
            this.GetAssemblyTypes<ITransient>().ForEach(x =>
            {
                var interfaces = x.GetInterfaces().ToList();
                interfaces.ForEach(intfc => this.services.AddTransient(intfc, x));
            });
        }

        private void RegisterSingleton()
        {
            this.GetAssemblyTypes<ISingleton>().ForEach(x =>
            {
                var interfaces = x.GetInterfaces().ToList();
                interfaces.ForEach(intfc => this.services.AddSingleton(intfc, x));
            });
        }

        private void RegisterPerRequest()
        {
            this.GetAssemblyTypes<IScoped>().ForEach(x =>
            {
                var interfaces = x.GetInterfaces().ToList();
                interfaces.ForEach(intfc => this.services.AddScoped(intfc, x));
            });
        }

        private List<Type> GetAssemblyTypes<TDependecyType>()
        {
            return this.GetType().Assembly.GetTypes()
                .Where(x => x.IsClass)
                .Where(t => typeof(TDependecyType).IsAssignableFrom(t))
                .ToList();
        }

        internal IServiceCollection Configure()
        {
            this.RegisterDependencies();
            return this.services;
        }
    }
}
