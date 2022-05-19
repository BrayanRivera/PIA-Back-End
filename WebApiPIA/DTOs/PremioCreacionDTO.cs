using System.ComponentModel.DataAnnotations;

namespace WebApiPIA.DTOs
{
    public class PremioCreacionDTO : IValidatableObject
    {
        [Required(ErrorMessage = "El campo {0} es requierido")]
        [StringLength(maximumLength: 40, ErrorMessage = "El campo {0} solo puede tener hasta 40 caracteres")]
        public string NombrePremio { get; set; }

        [Required(ErrorMessage = "El campo {0} es requierido")]
        public int RifaId { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!string.IsNullOrEmpty(NombrePremio))
            {
                var enMayuscula = NombrePremio.ToString();

                if (enMayuscula != enMayuscula.ToUpper())
                {
                    yield return new ValidationResult("El premio debe ser ingresado en mayusculas",
                        new String[] { nameof(NombrePremio) });
                }
            }
        }
    }
}
