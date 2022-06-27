using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace ActiveXTService.Data.Model
{
    public class Log
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [StringLength(100)]
        public string LogType { get; set; }
        [StringLength(100)]
        public string LogData { get; set; }
        public DateTime LogDate { get; set; }= DateTime.Now;
    }
}
