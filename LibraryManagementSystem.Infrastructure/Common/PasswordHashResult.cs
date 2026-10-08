namespace LibraryManagementSystem.Infrastructure.Common;

public record PasswordHashResult(byte[] Hash, byte[] Salt);