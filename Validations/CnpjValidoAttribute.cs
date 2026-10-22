using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Controle_De_estoqueAPI.Validations
{
    public class CnpjValidoAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var cnpj = value as string;

            if (string.IsNullOrWhiteSpace(cnpj))
            {
                return ValidationResult.Success;
            }

            cnpj = Regex.Replace(cnpj, @"[^\d]", "");

            if (cnpj.Length != 14)
            {
                return new ValidationResult("CNPJ inválido: deve conter 14 dígitos.");
            }

            if (new string(cnpj[0], cnpj.Length) == cnpj)
            {
                return new ValidationResult("CNPJ inválido.");
            }

            int[] multiplicadores1 = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            string doze = cnpj.Substring(0, 12);
            int soma = 0;
            for (int i = 0; i < 12; i++)
            {
                soma += int.Parse(doze[i].ToString()) * multiplicadores1[i];
            }
            int resto = soma % 11;
            int primeiroDigito = resto < 2 ? 0 : 11 - resto;

            int[] multiplicadores2 = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            string treze = doze + primeiroDigito;
            soma = 0;
            for (int i = 0; i < 13; i++)
            {
                soma += int.Parse(treze[i].ToString()) * multiplicadores2[i];
            }
            resto = soma % 11;
            int segundoDigito = resto < 2 ? 0 : 11 - resto;

            string digitoVerificadorCalculado = $"{primeiroDigito}{segundoDigito}";
            string digitoVerificadorInformado = cnpj.Substring(12, 2);

            if (digitoVerificadorCalculado != digitoVerificadorInformado)
            {
                return new ValidationResult("CNPJ inválido.");
            }

            return ValidationResult.Success;
        }
    }
}