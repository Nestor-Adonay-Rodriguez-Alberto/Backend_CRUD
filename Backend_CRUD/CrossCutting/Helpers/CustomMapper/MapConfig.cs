using System.Linq.Expressions;

namespace Unex.Modulo9.Financiamientos.CrossCutting.Helpers.CustomMapper
{
    public class MapConfig<TSource, TDestination>
    {
        public List<PropertyMap<TSource, TDestination>> PropertyMaps { get; } = new();
        public Action<TSource, TDestination> AfterMapAction { get; private set; }

        public MapConfig<TSource, TDestination> ForMember<TMember>(
            Expression<Func<TDestination, TMember>> destSelector,
            Func<TSource, MappingContext, object> mapFrom)
        {
            var memberName = ((MemberExpression)destSelector.Body).Member.Name;
            PropertyMaps.Add(new PropertyMap<TSource, TDestination>
            {
                DestinationProperty = memberName,
                ValueResolver = mapFrom
            });
            return this;
        }

        public MapConfig<TSource, TDestination> AfterMap(Action<TSource, TDestination> afterMap)
        {
            AfterMapAction = afterMap;
            return this;
        }
    }
}
