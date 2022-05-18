using System.ComponentModel.DataAnnotations;

namespace WebApiPIA.Entidades
{
    public class Cliente
    { 
        public int Id { get; set; }

        public string NombreCliente { get; set; }

        public string ApellidoCliente { get; set; }

        public string TelefonoCliente { get; set; }

        public int NumeroCliente { get; set; }
    }
}
 