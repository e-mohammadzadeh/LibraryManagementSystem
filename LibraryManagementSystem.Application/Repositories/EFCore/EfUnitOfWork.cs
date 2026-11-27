using LibraryManagementSystem.Domain.Interfaces;
using LibraryManagementSystem.Domain.Persistence;
using Microsoft.EntityFrameworkCore.Storage;

namespace LibraryManagementSystem.Application.Repositories.EFCore;

public sealed class EfUnitOfWork : IUnitOfWork
{
	private readonly LibraryDbContext _context;

	public EfUnitOfWork(LibraryDbContext context) => _context = context;


	public ITransactionScope Begin() =>
		_context.Database.CurrentTransaction is null
			? new Scope(_context, _context.Database.BeginTransaction())
			: new NestedScope();


	private sealed class Scope : ITransactionScope
	{
		private readonly LibraryDbContext _context;
		private readonly IDbContextTransaction _transaction;
		private bool _committed;


		public Scope(LibraryDbContext context, IDbContextTransaction transaction)
		{
			_context = context;
			_transaction = transaction;
		}


		public void Commit()
		{
			_transaction.Commit();
			_committed = true;
		}


		public void Dispose()
		{
			_transaction.Dispose();
			if (!_committed) _context.ChangeTracker.Clear();
		}
	}


	private sealed class NestedScope : ITransactionScope
	{
		public void Commit() { }
		public void Dispose() { }
	}
}