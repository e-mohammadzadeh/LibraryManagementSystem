namespace LibraryManagementSystem.Domain.Exceptions;

public class TranslatorNotFoundException : DomainException
{
	public TranslatorNotFoundException(Guid translatorId) : base($"Translator with id '{translatorId} was not found.")
	{
	}
}