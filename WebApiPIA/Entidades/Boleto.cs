namespace WebApiPIA.Entidades
{
    public class Boleto
    {
        public int Id { get; set; }

        public int NumeroBoleto { get; set; }

        public int RifaID  { get; set; }

        public int ClienteID { get; set; }

        public Rifa Rifa { get; set; }

        public Cliente Cliente { get; set; }
    }
}
