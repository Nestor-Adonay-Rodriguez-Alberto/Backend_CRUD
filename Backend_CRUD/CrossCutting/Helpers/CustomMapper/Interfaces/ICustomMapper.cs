namespace Unex.Modulo9.Financiamientos.CrossCutting.Helpers.CustomMapper.Interfaces
{
    public interface ICustomMapper
    {
        TDestination Map<TSource, TDestination>(TSource source, MappingContext context = null);
        MapConfig<TSource, TDestination> CreateMap<TSource, TDestination>();
        ICollection<TDestination> MapCollection<TSource, TDestination>(ICollection<TSource> source, MappingContext context = null);
    }
}
