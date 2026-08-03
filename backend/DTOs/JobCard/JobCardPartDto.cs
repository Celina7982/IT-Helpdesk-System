namespace IThelpdesk.DTOs.JobCard
{
    /// <summary>
    /// DTO returned when viewing Parts Used on a Job Card.
    /// </summary>
    public class JobCardPartDto
    {
        public int PartId { get; set; }

        public string PartName { get; set; } = string.Empty;

        public int Quantity { get; set; }
    }
}
