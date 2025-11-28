namespace Unex.Modulo9.Financiamientos.CrossCutting.Helpers.CustomMapper
{
    public class PropertyMap<TSource, TDestination>
    {
        public string DestinationProperty { get; set; }
        public Func<TSource, MappingContext, object> ValueResolver { get; set; }
    }

    public class MappingContext
    {
        private readonly Dictionary<string, object> _data = new();

        public void Set<T>(string key, T value) => _data[key] = value;
        public T Get<T>(string key) => _data.TryGetValue(key, out var value) ? (T)value : default;
        public bool Has(string key) => _data.ContainsKey(key);
        public object this[string key]
        {
            get => _data.TryGetValue(key, out var value) ? value : null;
            set => _data[key] = value;
        }
    }

}
