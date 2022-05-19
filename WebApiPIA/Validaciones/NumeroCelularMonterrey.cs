using System.ComponentModel.DataAnnotations;

namespace WebApiPIA.Validaciones
{
    public class NumeroCelularMonterrey : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()))
            {
                return ValidationResult.Success;
            }

            if (value.ToString().Length != 10)
            {
                return new ValidationResult("El numero de celular debe ser de 10 digitos");
            }

            var primerDigito = value.ToString()[0].ToString();

            if (primerDigito != "8")
            {
                return new ValidationResult("Ingrese un celular de Monterrey (los primeros 2 digitos deben ser 81).");

            }

            var segundoDigito = value.ToString()[1].ToString();

            if (segundoDigito != "1")
            {
                return new ValidationResult("Ingrese un celular de Monterrey (los primeros 2 digitos deben ser 81).");

            }
            return ValidationResult.Success;
        }
    }
}
