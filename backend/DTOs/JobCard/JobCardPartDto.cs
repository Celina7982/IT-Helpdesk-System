namespace IThelpdesk.DTOs.JobCard
{
    /// <summary>
    /// DTO returned when viewing Parts Used on a Job Card.
    /// </summary>
    public class JobCardPartDto
    {
        public int PartId { get; set; }

        public int JobCardId { get; set; }

        public string PartName { get; set; } = string.Empty;

        public int Quantity { get; set; }

        /// <summary>
        /// Display name of the user who added the part (e.g., "Celina M.").
        /// </summary>
        public string AddedByName { get; set; } = string.Empty;

        /// <summary>
        /// Id of the user who added the part (used for ownership/permission checks).
        /// </summary>
        public int AddedByUserId { get; set; }

        /// <summary>
        /// When the part was added (UTC).
        /// </summary>
        public DateTime DateAdded { get; set; }
    }
}
