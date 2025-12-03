using SQLite;

namespace MyszkaMauii
{
    public class Mouse1
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Model { get; set; }
        public string Firma { get; set; }
        public string Sensor { get; set; }
        public double Waga { get; set; }
        public string Link { get; set; }
    }
}
