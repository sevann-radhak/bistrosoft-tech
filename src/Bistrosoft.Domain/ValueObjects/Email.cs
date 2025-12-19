namespace Bistrosoft.Domain.ValueObjects;

public sealed class Email
{
    private Email(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Email Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("Email cannot be null or empty.", nameof(email));
        }

        if (!IsValidEmail(email))
        {
            throw new ArgumentException("Invalid email format.", nameof(email));
        }

        return new Email(email);
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    public override string ToString() => Value;

    public override bool Equals(object? obj)
    {
        if (obj is Email other)
        {
            return Value == other.Value;
        }
        return false;
    }

    public override int GetHashCode() => Value.GetHashCode();

    public static implicit operator string(Email email) => email.Value;
}
