namespace Accounts.Domain.Entities.CommandsInterface
{
    public interface IUpdateUserCommand : ICreateUserCommand
    {
        int Id { get; set; }
    }
}
