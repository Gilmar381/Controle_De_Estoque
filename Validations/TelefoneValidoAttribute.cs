using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Controle_De_estoqueAPI.Validations
{
    public class TelefoneValidoAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var telefone = value as string;

            if (string.IsNullOrWhiteSpace(telefone))
            {
                return ValidationResult.Success;
            }
          
            telefone = Regex.Replace(telefone, @"[^\d]", "");
           
            if (telefone.Length != 10 && telefone.Length != 11)
            {
                return new ValidationResult("Telefone inválido: deve conter 10 ou 11 dígitos (com DDD).");
            }
            
            int ddd = int.Parse(telefone.Substring(0, 2));
            if (ddd < 11 || ddd > 99)
            {
                return new ValidationResult("Telefone inválido: DDD fora da faixa válida.");
            }
            
            if (telefone.Length == 11)
            {
                char terceiroDigito = telefone[2];
                if (terceiroDigito != '9')
                {
                    return new ValidationResult("Telefone inválido: celular deve iniciar com 9 após o DDD.");
                }
            }
          
            if (new string(telefone[0], telefone.Length) == telefone)
            {
                return new ValidationResult("Telefone inválido.");
            }

            return ValidationResult.Success;
        }
    }
}