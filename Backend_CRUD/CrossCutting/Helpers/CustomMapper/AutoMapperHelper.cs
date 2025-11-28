using System.Reflection;
using Unex.Modulo9.Financiamientos.CrossCutting.Helpers.CustomMapper.Interfaces;

namespace Unex.Modulo9.Financiamientos.CrossCutting.Helpers.CustomMapper
{
    public class AutoMapperHelper : ICustomMapper
    {
        private readonly Dictionary<(Type, Type), object> _configs = new();
        private readonly Dictionary<object, object> _cache = new();

        public MapConfig<TSource, TDestination> CreateMap<TSource, TDestination>()
        {
            var config = new MapConfig<TSource, TDestination>();
            _configs[(typeof(TSource), typeof(TDestination))] = config;
            return config;
        }

        private bool IsCollectionType(Type type)
        {
            return type != typeof(string) && typeof(System.Collections.IEnumerable).IsAssignableFrom(type);
        }

        private Type GetCollectionElementType(Type type)
        {
            if (type.IsArray) return type.GetElementType();
            if (type.IsGenericType) return type.GetGenericArguments().First();
            return typeof(object);
        }

        private bool IsPrimitiveOrAssignable(Type type)
        {
            var underlyingType = Nullable.GetUnderlyingType(type) ?? type;

            return underlyingType.IsPrimitive
                || underlyingType.IsEnum
                || underlyingType == typeof(string)
                || underlyingType == typeof(decimal)
                || underlyingType == typeof(DateOnly)
                || underlyingType == typeof(DateTime)
                || underlyingType == typeof(Guid)
                || underlyingType == typeof(long)
                || underlyingType == typeof(int)
                || underlyingType == typeof(bool)
                || underlyingType == typeof(double);
        }

        public ICollection<TDestination> MapCollection<TSource, TDestination>(ICollection<TSource> source, MappingContext context)
        {
            return source == null
                ? new List<TDestination>()
                : source
                    .Select(item => Map<TSource, TDestination>(item, context))
                    .Where(mapped => mapped != null)
                    .ToList();
        }

        public TDestination Map<TSource, TDestination>(TSource source, MappingContext context = null)
        {
            if (source == null) return default;

            //if (_cache.TryGetValue(source, out var existing))
            //    return default;

            var config = _configs.GetValueOrDefault((typeof(TSource), typeof(TDestination))) as MapConfig<TSource, TDestination>;
            var destination = Activator.CreateInstance<TDestination>();
            //_cache[source] = destination!;

            var sourceProps = typeof(TSource).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var destProps = typeof(TDestination).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            context ??= new MappingContext();

            foreach (var destProp in destProps)
            {
                var customMap = config?.PropertyMaps.FirstOrDefault(p => p.DestinationProperty == destProp.Name);
                object value = null;

                if (customMap != null)
                {
                    value = customMap.ValueResolver(source, context);

                    if (value != null && IsCollectionType(value.GetType()) && IsCollectionType(destProp.PropertyType))
                    {
                        var srcElemType = GetCollectionElementType(value.GetType());
                        var destElemType = GetCollectionElementType(destProp.PropertyType);

                        if (_configs.ContainsKey((srcElemType, destElemType)))
                        {
                            var mapCollectionMethod = typeof(AutoMapperHelper)
                                .GetMethod(nameof(MapCollection))
                                .MakeGenericMethod(srcElemType, destElemType);

                            value = mapCollectionMethod.Invoke(this, new[] { value, context });
                        }
                    }

                    if (value != null && value.GetType() != destProp.PropertyType &&
                        _configs.ContainsKey((value.GetType(), destProp.PropertyType)))
                    {
                        var mapMethod = typeof(AutoMapperHelper)
                            .GetMethod(nameof(Map))
                            .MakeGenericMethod(value.GetType(), destProp.PropertyType);

                        value = mapMethod.Invoke(this, new object[] { value, context });
                    }

                    if (value != null && destProp.CanWrite)
                    {
                        var destType = destProp.PropertyType;

                        // Si es colección y el valor no es del tipo esperado, intentamos convertir a List<T>
                        if (IsCollectionType(destType) && !destType.IsAssignableFrom(value.GetType()))
                        {
                            var destElementType = GetCollectionElementType(destType);
                            var castMethod = typeof(Enumerable).GetMethod(nameof(Enumerable.ToList))?
                                .MakeGenericMethod(destElementType);

                            if (castMethod != null)
                            {
                                value = castMethod.Invoke(null, new[] { value });
                            }
                        }

                        var actualDestType = Nullable.GetUnderlyingType(destType) ?? destType;
                        var valueType = value.GetType();
                        if (actualDestType.IsAssignableFrom(valueType))
                        {
                            destProp.SetValue(destination, value);
                        }
                    }


                    continue;
                }

                var sourceProp = sourceProps.FirstOrDefault(p => p.Name.Equals(destProp.Name, StringComparison.OrdinalIgnoreCase));
                if (sourceProp != null && destProp.CanWrite)
                {
                    value = sourceProp.GetValue(source);

                    if (value != null)
                    {
                        if (IsCollectionType(sourceProp.PropertyType) && IsCollectionType(destProp.PropertyType))
                        {
                            var srcElemType = GetCollectionElementType(sourceProp.PropertyType);
                            var destElemType = GetCollectionElementType(destProp.PropertyType);

                            if (_configs.ContainsKey((srcElemType, destElemType)))
                            {
                                var mapCollectionMethod = typeof(AutoMapperHelper)
                                    .GetMethod(nameof(MapCollection))
                                    .MakeGenericMethod(srcElemType, destElemType);

                                value = mapCollectionMethod.Invoke(this, new[] { value, context });
                            }
                        }

                        else if (_configs.ContainsKey((sourceProp.PropertyType, destProp.PropertyType)))
                        {
                            var mapMethod = typeof(AutoMapperHelper)
                                .GetMethod(nameof(Map))
                                .MakeGenericMethod(sourceProp.PropertyType, destProp.PropertyType);

                            value = mapMethod.Invoke(this, new object[] { value, context });
                        }
                    }

                    if (IsPrimitiveOrAssignable(destProp.PropertyType) ||
                        (value != null && destProp.PropertyType.IsAssignableFrom(value.GetType())))
                    {
                        destProp.SetValue(destination, value);
                    }
                }
            }

            config?.AfterMapAction?.Invoke(source, destination);
            return destination;
        }
    }

}