
namespace IThelpdesk.DTOs.JobCard
{
    public class UpdatePaperJobTimeDto
    {
        //--------------------------------------------------
        // Overall Paper Job Card Time
        //--------------------------------------------------

        public DateTimeOffset? TimeIn { get; set; }

        public DateTimeOffset? TimeOut { get; set; }

        //--------------------------------------------------
        // Travel and On Site
        //--------------------------------------------------

        public bool? IsTravel { get; set; }

        public bool? IsOnSite { get; set; }
    }
}
