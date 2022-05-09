namespace WebApiPIA.Entidades
{
    public class Premio
    {
        public int Id { get; set; }

        public string NombrePremio{ get; set; }

        public List<Rifa> Rifa { get; set; }
    }
}
