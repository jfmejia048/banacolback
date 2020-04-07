namespace PerfilacionDeCalidad.Backend.Data.Entities
{
    using System.Collections.Generic;
    public class CreatePomas
    {
        public int ID { get; set; }
        public int Codigo { get; set; }
        public int Numero { get; set; }
        public string Placa { get; set; }
        public bool Estado { get; set; }
        public Fincas Finca { get; set; }
        public Puertos Puerto { get; set; }
        public List<DetailPoma> DetailPoma { get; set; }
    }
}
