using System.Text.RegularExpressions;

namespace PersonalExpenses.Security.Validators
{
    public class PasswordStrengthValidator : IPasswordValidator
    {
        private const int MinLength = 8;
        private const int MaxLength = 128;

        public (bool IsValid, List<string> Errors) ValidatePasswordStrength(string password)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(password))
            {
                errors.Add("Senha é obrigatória");
                return (false, errors);
            }

            if (password.Length < MinLength)
                errors.Add($"Senha deve ter no mínimo {MinLength} caracteres");

            if (password.Length > MaxLength)
                errors.Add($"Senha deve ter no máximo {MaxLength} caracteres");

            if (!Regex.IsMatch(password, @"[A-Z]"))
                errors.Add("Senha deve conter pelo menos uma letra maiúscula");

            if (!Regex.IsMatch(password, @"[a-z]"))
                errors.Add("Senha deve conter pelo menos uma letra minúscula");

            if (!Regex.IsMatch(password, @"[0-9]"))
                errors.Add("Senha deve conter pelo menos um número");

            if (!Regex.IsMatch(password, @"[!@#$%^&*()_+\-=\[\]{};':"",.<>?/\\|`~]"))
                errors.Add("Senha deve conter pelo menos um caractere especial (!@#$%^&*)");

            return (errors.Count == 0, errors);
        }
    }
}
