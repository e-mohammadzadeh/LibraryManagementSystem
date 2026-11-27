namespace LibraryManagementSystem.Domain.Interfaces;

public interface ITransactionScope : IDisposable
{
	void Commit();
}


public interface IUnitOfWork
{
	ITransactionScope Begin();
}