using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Clinical.Models
{
    [Table("Priority")]
    public class Priority
    {
        [Key]
        public int PriorityID { get; set; }

        [Required, StringLength(50)]
        public string PriorityLevel { get; set; }
    }
}
