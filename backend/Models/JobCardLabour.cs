using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IThelpdesk.Models
{
    public class JobCardLabour
    {
        [Key]
        public int LabourId { get; set; }

        public int JobCardId { get; set; }

        public int CreatedByUserId { get; set; }

        public decimal HoursWorked { get; set; }

        public string WorkPerformed { get; set; } = string.Empty;

        public DateTime DateWorked { get; set; }

        //---------------------------------------------------
        // Navigation
        //---------------------------------------------------

        public JobCard? JobCard { get; set; }

        public User? CreatedByUser { get; set; }
    }
}