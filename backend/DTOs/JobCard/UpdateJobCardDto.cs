namespace IThelpdesk.DTOs.JobCard
{
    public class UpdateJobCardDto
    {
        public string Status { get; set; } = string.Empty;

        public string FaultFound { get; set; } = string.Empty;

        public string WorkPerformed { get; set; } = string.Empty;

        public string CompletionNotes { get; set; } = string.Empty;

        public string CustomerSignature { get; set; } = string.Empty;
    }
}
