using System.Reflection;
using Unex.Modulo9.Financiamientos.CrossCutting.Helpers.CustomMapper.Interfaces;

namespace Unex.Modulo9.Financiamientos.Application.Mapping
{
    public class CustomMapperConfigurations
    {
        public static void RegisterMappings(ICustomMapper mapper)
        {
            var mappingTypes = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => typeof(IMappingConfiguration).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

            foreach (var type in mappingTypes)
            {
                var config = (IMappingConfiguration)Activator.CreateInstance(type);
                config.Configure(mapper);
            }
        }
    }
}
