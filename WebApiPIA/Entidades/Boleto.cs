using System.ComponentModel.DataAnnotations;

namespace WebApiPIA.Entidades
{
    public class Boleto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El campo {0} es requierido")]
        [Range(100, 999, ErrorMessage = "El numero de la rifa debe de ser un valor entre 100 y 999")]
        public int NumeroBoleto { get; set; }

        [Required(ErrorMessage = "El campo {0} es requierido")]
        public int ClienteID { get; set; }

        [Required(ErrorMessage = "El campo {0} es requierido")]
        public int RifaID  { get; set; }

    }
}
