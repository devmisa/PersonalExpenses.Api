namespace PersonalExpenses.Security.Validators
{
    public interface IPasswordValidator
    {
        (bool IsValid, List<string> Errors) ValidatePasswordStrength(string password);
    }
}

