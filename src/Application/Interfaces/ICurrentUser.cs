namespace Application.Interfaces
{
    public interface ICurrentUser
    {
        string UserId { get; }

        string DisplayName { get; }

        string Email { get; }
    }
}
