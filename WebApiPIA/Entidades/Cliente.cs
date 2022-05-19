using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using WebApiPIA.Validaciones;

namespace WebApiPIA.Entidades
{
    public class Cliente : IValidatableObject
    { 
        public int Id { get; set; }

        [Required(ErrorMessage = "El campo {0} es requierido")]
        [StringLength(maximumLength: 15, ErrorMessage = "El campo {0} solo puede tener hasta 15 caracteres")]
        public string NombreCliente { get; set; }

        [Required(ErrorMessage = "El campo {0} es requierido")]
        [StringLength(maximumLength: 15, ErrorMessage = "El campo {0} solo puede tener hasta 15 caracteres")]
        public string ApellidoCliente { get; set; }

        [Required(ErrorMessage = "El campo {0} es requierido")]
        [NumeroCelularMonterrey]
        public string TelefonoCliente { get; set; }

        [Required(ErrorMessage = "El campo {0} es requierido")]
        [Range(100000, 999999, ErrorMessage = "El numero de la rifa debe de ser un valor entre 100000 y 999999")]
        public int NumeroCliente { get; set; }

        public string UsuarioId { get; set; }
        public IdentityUser Usuario { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!string.IsNullOrEmpty(NombreCliente))
            {
                var primeraLetra = NombreCliente[0].ToString();

                if (primeraLetra != primeraLetra.ToUpper())
                {
                    yield return new ValidationResult("La primera letra del nombre debe ser mayuscula",
                        new String[] { nameof(NombreCliente) });
                }
            }

            if (!string.IsNullOrEmpty(ApellidoCliente))
            {
                var primeraLetra = ApellidoCliente[0].ToString();

                if (primeraLetra != primeraLetra.ToUpper())
                {
                    yield return new ValidationResult("La primera letra del apellido debe ser mayuscula",
                        new String[] { nameof(ApellidoCliente) });
                }
            }
        }
    }
}
 