using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Exceptions;
using LibraryManagementSystem.Domain.Interfaces;
using LibraryManagementSystem.Infrastructure.Enums.Filters;
using LibraryManagementSystem.Infrastructure.ValueObjects;

namespace LibraryManagementSystem.Application.Repositories.InMemory;

public class InMemoryTranslatorRepository : ITranslatorRepository
{
	private readonly List<Translator> _translators;
	public InMemoryTranslatorRepository() { _translators = CreateSeedData(); }


	private static List<Translator> CreateSeedData()
	{
		return
		[
			new Translator
			{
				Id = Guid.CreateVersion7(),
				FirstName = "Najaf",
				LastName = "Daryabandari",
				NationalCode = "1234567890",
				Email = Email.Create("najaf.daryabandari@example.com"),
				PhoneNumber = PhoneNumber.Create("09123456789"),
				BirthDate = new DateOnly(1921, 6, 12),
				Biography = "Good Iranian translator."
			},
			new Translator
			{
				Id = Guid.CreateVersion7(),
				FirstName = "Ahmad",
				LastName = "Golshiri",
				NationalCode = "0987654321",
				Email = Email.Create("ahmad.golshiri@example.com"),
				PhoneNumber = PhoneNumber.Create("09127654321"),
				BirthDate = new DateOnly(1940, 3, 25),
				Biography = "Good Iranian translator."
			},
			new Translator
			{
				Id = Guid.CreateVersion7(),
				FirstName = "Mansoureh",
				LastName = "Pirnia",
				NationalCode = "1122334455",
				Email = Email.Create("mansoureh.pirnia@example.com"),
				PhoneNumber = PhoneNumber.Create("09129876543"),
				BirthDate = new DateOnly(1955, 11, 8),
				Biography = "Good Iranian translator."
			},
			new Translator
			{
				Id = Guid.CreateVersion7(),
				FirstName = "Reza",
				LastName = "SeyedHosseini",
				NationalCode = "6677889900",
				Email = Email.Create("reza.seyedhosseini@example.com"),
				PhoneNumber = PhoneNumber.Create("09121122334"),
				BirthDate = new DateOnly(1968, 9, 17),
				Biography = "Good Iranian translator."
			}
		];
	}


	public void Add(Translator translator)
	{
		ArgumentNullException.ThrowIfNull(translator);

		translator.Id = Guid.CreateVersion7();
		translator.CreatedAt = DateTime.UtcNow;
		translator.IsRemoved = false;
		_translators.Add(translator);
	}


	public Translator? GetById(Guid id, EntityFilter filter = EntityFilter.Active)
	{
		return ApplyFilter(_translators, filter).FirstOrDefault(t => t.Id == id);
	}


	public IReadOnlyList<Translator> GetAll(EntityFilter filter = EntityFilter.Active)
	{
		return [.. ApplyFilter(_translators, filter)];
	}


	public bool Exists<T>(T value, Func<Translator, T> selector, Guid? excludeId)
	{
		return _translators.Any(t =>
			!t.IsRemoved &&
			(excludeId is null || t.Id != excludeId) &&
			EqualityComparer<T>.Default.Equals(selector(t), value));
	}


	public void Remove(Translator translator)
	{
		ArgumentNullException.ThrowIfNull(translator);
		if (translator.IsRemoved) return;
		translator.IsRemoved = true;
		translator.UpdatedAt = DateTime.UtcNow;
	}


	public void Update(Translator translator)
	{
		ArgumentNullException.ThrowIfNull(translator);

		var tracked = _translators.FirstOrDefault(a => a.Id == translator.Id) ??
		              throw new TranslatorNotFoundException(translator.Id);

		tracked.FirstName = translator.FirstName;
		tracked.LastName = translator.LastName;
		tracked.NationalCode = translator.NationalCode;
		tracked.Email = translator.Email;
		tracked.PhoneNumber = translator.PhoneNumber;
		tracked.BirthDate = translator.BirthDate;
		tracked.Biography = translator.Biography;
		tracked.UpdatedAt = DateTime.UtcNow;
		tracked.UpdatedByUserId = translator.UpdatedByUserId;
	}


	// ---------- Private helper ----------
	private static IEnumerable<Translator> ApplyFilter(IEnumerable<Translator> source, EntityFilter filter)
	{
		return filter switch
		{
			EntityFilter.Active => source.Where(a => !a.IsRemoved),
			EntityFilter.Removed => source.Where(a => a.IsRemoved),
			EntityFilter.All => source,
			_ => throw new ArgumentOutOfRangeException(nameof(filter), filter, null)
		};
	}
}