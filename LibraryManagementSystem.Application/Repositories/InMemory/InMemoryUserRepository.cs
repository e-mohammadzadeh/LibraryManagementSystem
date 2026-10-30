using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Exceptions;
using LibraryManagementSystem.Domain.Interfaces;
using LibraryManagementSystem.Infrastructure.Enums;
using LibraryManagementSystem.Infrastructure.Enums.Filters;
using LibraryManagementSystem.Infrastructure.Interfaces;
using LibraryManagementSystem.Infrastructure.ValueObjects;

namespace LibraryManagementSystem.Application.Repositories.InMemory;

public class InMemoryUserRepository : IUserRepository
{
	private readonly List<User> _users;


	public InMemoryUserRepository(IRoleRepository roles, IPasswordHasher hasher)
	{
		_users = CreateSeedData(roles, hasher);
	}


	private static List<User> CreateSeedData(IRoleRepository roles, IPasswordHasher hasher)
	{
		var adminRole = roles.GetAll().First(r => r.Name == LibraryUserRole.Admin);
		var memberRole = roles.GetAll().First(r => r.Name == LibraryUserRole.Member);
		var librarianRole = roles.GetAll().First(r => r.Name == LibraryUserRole.Librarian);

		var hash = hasher.CreatePasswordHash("Admin@123");
		var admin = new User
		{
			Id = Guid.CreateVersion7(),
			FirstName = "Sara",
			LastName = "Admin",
			NationalCode = "3780254901",
			Email = Email.Create("admin@library.com"),
			PhoneNumber = PhoneNumber.Create("09120000010"),
			BirthDate = new DateOnly(1985, 3, 15),
			Role = adminRole,
			MembershipStartDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-100)),
			CreatedAt = DateTime.UtcNow.AddDays(-100),
			IsActive = true,
			MembershipExpiryDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-100)).AddYears(1),
			PasswordHash = hash.Hash,
			PasswordSalt = hash.Salt,
			IsRemoved = false,
			ShouldRemove = false
		};

		hash = hasher.CreatePasswordHash("Librarian1@123");
		var librarian1 = new User
		{
			Id = Guid.CreateVersion7(),
			FirstName = "Ali",
			LastName = "Librarian",
			NationalCode = "3780254902",
			Email = Email.Create("librarian@library.com"),
			PhoneNumber = PhoneNumber.Create("09120000011"),
			BirthDate = new DateOnly(1990, 5, 20),
			Role = librarianRole,
			MembershipStartDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-70)),
			CreatedAt = DateTime.UtcNow.AddDays(-70),
			IsActive = true,
			MembershipExpiryDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-70)).AddYears(1),
			PasswordHash = hash.Hash,
			PasswordSalt = hash.Salt,
			IsRemoved = false,
			ShouldRemove = false
		};


		hash = hasher.CreatePasswordHash("Librarian2@123");
		var librarian2 = new User
		{
			Id = Guid.CreateVersion7(),
			FirstName = "Reza",
			LastName = "Karimi",
			NationalCode = "3780254903",
			Email = Email.Create("reza.karimi@library.com"),
			PhoneNumber = PhoneNumber.Create("09120000014"),
			BirthDate = new DateOnly(1988, 11, 5),
			Role = librarianRole,
			MembershipStartDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-70)),
			CreatedAt = DateTime.UtcNow.AddDays(-70),
			IsActive = true,
			MembershipExpiryDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-70)).AddYears(1),
			PasswordHash = hash.Hash,
			PasswordSalt = hash.Salt,
			IsRemoved = false,
			ShouldRemove = false
		};


		hash = hasher.CreatePasswordHash("Librarian3@123");
		var librarian3 = new User
		{
			Id = Guid.CreateVersion7(),
			FirstName = "Zahra",
			LastName = "Rahimi",
			NationalCode = "3780254904",
			Email = Email.Create("zahra.rahimi@library.com"),
			PhoneNumber = PhoneNumber.Create("09120000015"),
			BirthDate = new DateOnly(1992, 4, 18),
			Role = librarianRole,
			MembershipStartDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-70)),
			CreatedAt = DateTime.UtcNow.AddDays(-70),
			IsActive = true,
			MembershipExpiryDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-70)).AddYears(1),
			PasswordHash = hash.Hash,
			PasswordSalt = hash.Salt,
			IsRemoved = false,
			ShouldRemove = false
		};


		hash = hasher.CreatePasswordHash("Member1@123");
		var member1 = new User
		{
			Id = Guid.CreateVersion7(),
			FirstName = "Mohammad",
			LastName = "Ahmadi",
			NationalCode = "3780254905",
			Email = Email.Create("m.ahmadi@example.com"),
			PhoneNumber = PhoneNumber.Create("09120000012"),
			BirthDate = new DateOnly(1998, 1, 10),
			Role = memberRole,
			MembershipStartDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-70)),
			CreatedAt = DateTime.UtcNow.AddDays(-70),
			IsActive = true,
			MembershipExpiryDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-70)).AddYears(1),
			PasswordHash = hash.Hash,
			PasswordSalt = hash.Salt,
			IsRemoved = false,
			ShouldRemove = false
		};


		var member2 = new User
		{
			Id = Guid.CreateVersion7(),
			FirstName = "Fateme",
			LastName = "Hosseini",
			NationalCode = "3780254906",
			Email = Email.Create("f.hosseini@example.com"),
			PhoneNumber = PhoneNumber.Create("09120000013"),
			BirthDate = new DateOnly(2000, 7, 25),
			Role = memberRole,
			MembershipStartDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-70)),
			CreatedAt = DateTime.UtcNow.AddDays(-70),
			IsActive = true,
			MembershipExpiryDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-70)).AddYears(1),
			PasswordHash = hash.Hash,
			PasswordSalt = hash.Salt,
			IsRemoved = false,
			ShouldRemove = false
		};

		var member3 = new User
		{
			Id = Guid.CreateVersion7(),
			FirstName = "Hossein",
			LastName = "Moradi",
			NationalCode = "3780254907",
			Email = Email.Create("h.moradi@example.com"),
			PhoneNumber = PhoneNumber.Create("09120000016"),
			BirthDate = new DateOnly(1995, 9, 12),
			Role = memberRole,
			MembershipStartDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-70)),
			CreatedAt = DateTime.UtcNow.AddDays(-70),
			IsActive = true,
			MembershipExpiryDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-70)).AddYears(1),
			PasswordHash = hash.Hash,
			PasswordSalt = hash.Salt,
			IsRemoved = false,
			ShouldRemove = false
		};

		hash = hasher.CreatePasswordHash("Member4@123");
		var member4 = new User
		{
			Id = Guid.CreateVersion7(),
			FirstName = "Narges",
			LastName = "Salehi",
			NationalCode = "3780254908",
			Email = Email.Create("n.salehi@example.com"),
			PhoneNumber = PhoneNumber.Create("09120000017"),
			BirthDate = new DateOnly(2001, 2, 28),
			Role = memberRole,
			MembershipStartDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-70)),
			CreatedAt = DateTime.UtcNow.AddDays(-70),
			IsActive = true,
			MembershipExpiryDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-70)).AddYears(1),
			PasswordHash = hash.Hash,
			PasswordSalt = hash.Salt,
			IsRemoved = false,
			ShouldRemove = false
		};


		var member5 = new User
		{
			Id = Guid.CreateVersion7(),
			FirstName = "Ali",
			LastName = "Rezaei",
			NationalCode = "3780254909",
			Email = Email.Create("a.rezaei@example.com"),
			PhoneNumber = PhoneNumber.Create("09120000018"),
			BirthDate = new DateOnly(1997, 6, 15),
			Role = memberRole,
			MembershipStartDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-70)),
			CreatedAt = DateTime.UtcNow.AddDays(-70),
			IsActive = true,
			MembershipExpiryDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-70)).AddYears(1),
			PasswordHash = hash.Hash,
			PasswordSalt = hash.Salt,
			IsRemoved = false,
			ShouldRemove = false
		};

		var member6 = new User
		{
			Id = Guid.CreateVersion7(),
			FirstName = "Maryam",
			LastName = "Khalili",
			NationalCode = "3780254910",
			Email = Email.Create("maryam.khalili@example.com"),
			PhoneNumber = PhoneNumber.Create("09120000019"),
			BirthDate = new DateOnly(1999, 10, 3),
			Role = memberRole,
			MembershipStartDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-70)),
			CreatedAt = DateTime.UtcNow.AddDays(-70),
			IsActive = true,
			MembershipExpiryDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-70)).AddYears(1),
			PasswordHash = hash.Hash,
			PasswordSalt = hash.Salt,
			IsRemoved = false,
			ShouldRemove = false
		};

		var member7 = new User
		{
			Id = Guid.CreateVersion7(),
			FirstName = "Seyed",
			LastName = "Mousavi",
			NationalCode = "3780254911",
			Email = Email.Create("s.mousavi@example.com"),
			PhoneNumber = PhoneNumber.Create("09120000020"),
			BirthDate = new DateOnly(1987, 12, 22),
			Role = memberRole,
			MembershipStartDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-70)),
			CreatedAt = DateTime.UtcNow.AddDays(-70),
			IsActive = true,
			MembershipExpiryDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-70)).AddYears(1),
			PasswordHash = hash.Hash,
			PasswordSalt = hash.Salt,
			IsRemoved = false,
			ShouldRemove = false
		};

		var member8 = new User
		{
			Id = Guid.CreateVersion7(),
			FirstName = "Leila",
			LastName = "Pourahmadi",
			NationalCode = "3780254912",
			Email = Email.Create("leila.pourahmadi@example.com"),
			PhoneNumber = PhoneNumber.Create("09120000021"),
			BirthDate = new DateOnly(2002, 5, 7),
			Role = memberRole,
			MembershipStartDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-70)),
			CreatedAt = DateTime.UtcNow.AddDays(-70),
			IsActive = true,
			MembershipExpiryDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-70)).AddYears(1),
			PasswordHash = hash.Hash,
			PasswordSalt = hash.Salt,
			IsRemoved = false,
			ShouldRemove = false
		};

		var member9 = new User
		{
			Id = Guid.CreateVersion7(),
			FirstName = "Mehdi",
			LastName = "Hashemi",
			NationalCode = "3780254913",
			Email = Email.Create("mehdi.hashemi@example.com"),
			PhoneNumber = PhoneNumber.Create("09120000022"),
			BirthDate = new DateOnly(1996, 8, 19),
			Role = memberRole,
			MembershipStartDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-65)),
			CreatedAt = DateTime.UtcNow.AddDays(-65),
			IsActive = true,
			MembershipExpiryDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-65)).AddYears(1),
			PasswordHash = hash.Hash,
			PasswordSalt = hash.Salt,
			IsRemoved = false,
			ShouldRemove = false
		};

		var member10 = new User
		{
			Id = Guid.CreateVersion7(),
			FirstName = "Sara",
			LastName = "Nikoo",
			NationalCode = "3780254914",
			Email = Email.Create("s.nikoo@example.com"),
			PhoneNumber = PhoneNumber.Create("09120000023"),
			BirthDate = new DateOnly(2003, 3, 30),
			Role = memberRole,
			MembershipStartDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-65)),
			CreatedAt = DateTime.UtcNow.AddDays(-65),
			IsActive = true,
			MembershipExpiryDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-65)).AddYears(1),
			PasswordHash = hash.Hash,
			PasswordSalt = hash.Salt,
			IsRemoved = false,
			ShouldRemove = false
		};

		var member11 = new User
		{
			Id = Guid.CreateVersion7(),
			FirstName = "Amir",
			LastName = "Jafari",
			NationalCode = "3780254915",
			Email = Email.Create("amir.jafari@example.com"),
			PhoneNumber = PhoneNumber.Create("09120000024"),
			BirthDate = new DateOnly(1994, 7, 14),
			Role = memberRole,
			MembershipStartDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-65)),
			CreatedAt = DateTime.UtcNow.AddDays(-65),
			IsActive = true,
			MembershipExpiryDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-65)).AddYears(1),
			PasswordHash = hash.Hash,
			PasswordSalt = hash.Salt,
			IsRemoved = false,
			ShouldRemove = false
		};

		var member12 = new User
		{
			Id = Guid.CreateVersion7(),
			FirstName = "Fatemeh",
			LastName = "Ebrahimi",
			NationalCode = "3780254916",
			Email = Email.Create("f.ebrahimi@example.com"),
			PhoneNumber = PhoneNumber.Create("09120000025"),
			BirthDate = new DateOnly(1991, 11, 9),
			Role = memberRole,
			MembershipStartDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-65)),
			CreatedAt = DateTime.UtcNow.AddDays(-65),
			IsActive = true,
			MembershipExpiryDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-65)).AddYears(1),
			PasswordHash = hash.Hash,
			PasswordSalt = hash.Salt,
			IsRemoved = false,
			ShouldRemove = false
		};

		return
		[
			admin, librarian1, librarian2, librarian3, member1, member2, member3, member4, member5, member6, member7,
			member8, member9, member10, member11, member12
		];
	}


	public void Add(User user)
	{
		ArgumentNullException.ThrowIfNull(user);

		user.Id = Guid.CreateVersion7();
		user.CreatedAt = DateTime.UtcNow;
		user.IsActive = true;
		user.MembershipExpiryDate = user.MembershipStartDate.AddYears(1);
		user.IsRemoved = false;
		user.ShouldRemove = false;
		_users.Add(user);
	}


	public User? GetById(Guid id, EntityFilter filter = EntityFilter.Active)
	{
		return ApplyFilter(_users, filter).FirstOrDefault(u => u.Id == id);
	}


	public User? GetByName(string firstName, string lastName, EntityFilter filter = EntityFilter.Active)
	{
		return ApplyFilter(_users, filter).FirstOrDefault(u =>
			u.FirstName.Equals(firstName, StringComparison.OrdinalIgnoreCase) &&
			u.LastName.Equals(lastName, StringComparison.OrdinalIgnoreCase));
	}


	public User? GetByEmail(Email email, EntityFilter filter = EntityFilter.Active)
	{
		return ApplyFilter(_users, filter)
			.FirstOrDefault(u => u.Email.Equals(email));
	}


	public IReadOnlyList<User> GetAll(EntityFilter filter = EntityFilter.Active)
	{
		return [.. ApplyFilter(_users, filter)];
	}


	public bool Exists<T>(T value, Func<User, T> selector, Guid? excludeId)
	{
		return _users.Any(a =>
			!a.IsRemoved &&
			(excludeId is null || a.Id != excludeId) &&
			EqualityComparer<T>.Default.Equals(selector(a), value));
	}


	public void Remove(User user)
	{
		ArgumentNullException.ThrowIfNull(user);
		if (user.IsRemoved) return;
		user.IsRemoved = true;
		user.IsActive = false;
		user.UpdatedAt = DateTime.UtcNow;
	}


	public void Update(User user)
	{
		ArgumentNullException.ThrowIfNull(user);

		var tracked = _users.FirstOrDefault(u => u.Id == user.Id) ?? throw new UserNotFoundException(user.Id);


		tracked.FirstName = user.FirstName;
		tracked.LastName = user.LastName;
		tracked.NationalCode = user.NationalCode;
		tracked.Email = user.Email;
		tracked.PhoneNumber = user.PhoneNumber;
		tracked.BirthDate = user.BirthDate;
		tracked.Role = user.Role;
		tracked.UpdatedAt = DateTime.UtcNow;
		tracked.UpdatedByUserId = user.UpdatedByUserId;
	}


	// ---------- Private helper ---------
	private static IEnumerable<User> ApplyFilter(IEnumerable<User> source, EntityFilter filter)
	{
		return filter switch
		{
			EntityFilter.Active => source.Where(b => !b.IsRemoved),
			EntityFilter.Removed => source.Where(b => b.IsRemoved),
			EntityFilter.All => source,
			_ => throw new ArgumentOutOfRangeException(nameof(filter), filter, null)
		};
	}
}