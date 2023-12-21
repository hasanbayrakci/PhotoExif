using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PhotoExif.Models
{
    public class Photo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FileSize { get; set; }
        public DateTime DateTimeOriginal { get; set; }
        public string FilePath { get; set; }
        public DateTime Tarih { get; set; } = DateTime.Now;
    }
}
