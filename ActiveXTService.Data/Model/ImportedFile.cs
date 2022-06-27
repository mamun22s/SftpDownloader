using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace ActiveXTService.Data.Model
{
    public class ImportedFile
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [StringLength(200)]
        public string FileName { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime FileSaveDate { get; set; } = DateTime.Now;
        public byte[] FileContent { get; set; }
        [StringLength(200)]
        public string FilePath { get; set; }
        [StringLength(200)]
        public string Status { get; set; }
    }
}
