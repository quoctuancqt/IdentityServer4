using Core.Domain;

namespace Core.Interfaces
{
    public interface IRepository<T> where T : IBaseEntity
    {
        IUnitOfWork UnitOfWork { get; }
    }
}
