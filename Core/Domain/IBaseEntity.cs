namespace Core.Domain
{
    public interface IBaseEntity<TKey>
    {
        TKey Id { get; set; }
    }

    public interface IBaseEntity : IBaseEntity<string> { }
}
