using System.ComponentModel.DataAnnotations;

namespace WebApiPIA.Entidades
{
    public class Rifa
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El campo {0} es requierido")]
        [Range(1,100, ErrorMessage = "El numero de la rifa debe de ser un valor entre 1 y 100")]
        public int NumeroRifa { get; set; }

    }
}
