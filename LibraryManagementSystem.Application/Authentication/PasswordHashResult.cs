namespace LibraryManagementSystem.Application.Authentication;

public record PasswordHashResult(byte[] Hash, byte[] Salt);