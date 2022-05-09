namespace WebApiPIA.Entidades
{
    public class Rifa
    {
        public int Id { get; set; }

        public int NumeroRifa { get; set; }

        public Premio Premio { get; set; }

        public List<Boleto> Boleto { get; set; }
    }
}
