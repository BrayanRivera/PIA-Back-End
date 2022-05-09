using System.ComponentModel.DataAnnotations;

namespace WebApiPIA.Entidades
{
    public class Cliente
    { 
        public int Id { get; set; }

        public string NombreCliente { get; set; }

        public string ApellidoCliente { get; set; }

        public int TelefonoCliente { get; set; }

        public int NumeroCliente { get; set; }

        public List<Boleto> Boleto { get; set; }
    }
}
 