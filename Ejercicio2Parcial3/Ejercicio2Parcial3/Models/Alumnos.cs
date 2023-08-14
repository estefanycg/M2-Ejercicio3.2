using SQLite;
using Xamarin.Forms;

namespace Ejercicio2Parcial3.Models
{
    [Table("Alumnos")]
    public class Alumnos
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [MaxLength(50)]
        public string Nombres { get; set; }

        [MaxLength(50)]
        public string Apellidos { get; set; }

        [MaxLength(10)]
        public string Sexo { get; set; }

        [MaxLength(100)]
        public string Direccion { get; set; }

        public string Imagen { get; set; } // Base64 string

        [Ignore]
        public ImageSource Foto { get; set; } // ImageSource property
    }
}
