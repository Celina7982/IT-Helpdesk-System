using IThelpdesk.DTOs.JobCard;
using IThelpdesk.Enums;
using IThelpdesk.Interfaces.Repositories;
using IThelpdesk.Interfaces.Services;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace IThelpdesk.Services.Pdf
{
    public class JobCardPdfService : IJobCardPdfService
    {
        private readonly IJobCardRepository _jobCardRepository;

        private readonly IJobCardAuditService _auditService;
        public JobCardPdfService(
         IJobCardRepository jobCardRepository,
         IJobCardAuditService auditService)
        {
            _jobCardRepository = jobCardRepository;
            _auditService = auditService;
        }

        public async Task<byte[]> GenerateJobCardPdfAsync(int jobCardId)
        {
            var jobCard = await _jobCardRepository.GetDetailsAsync(jobCardId);

            if (jobCard == null)
                throw new Exception("Job Card not found.");

            await _auditService.LogAsync(
            jobCard.JobCardId,
            jobCard.AssignedTechnicianId ?? 0,
            JobCardAuditAction.PdfGenerated,
            $"PDF generated for Job Card {jobCard.JobNumber}");

            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);

                    page.Header()
                        .Text("JOB CARD")
                        .FontSize(24)
                        .Bold();

                    page.Content()
                        .Column(column =>
                        {
                            column.Spacing(10);

                            column.Item().Text($"Job Number: {jobCard.JobNumber}");
                            column.Item().Text($"Customer: {jobCard.CustomerName}");
                            column.Item().Text($"Technician: {jobCard.AssignedTechnician}");
                            column.Item().Text($"Status: {jobCard.Status}");
                            column.Item().Text($"Date Created: {jobCard.DateCreated:d}");

                            column.Item().LineHorizontal(1);

                            column.Item().Text("Fault Reported").Bold();
                            column.Item().Text(jobCard.FaultReported);

                            column.Item().Text("Fault Found").Bold();
                            column.Item().Text(jobCard.FaultFound);

                            column.Item().Text("Work Performed").Bold();
                            column.Item().Text(jobCard.WorkPerformed);

                            column.Item().Text("Completion Notes").Bold();
                            column.Item().Text(jobCard.CompletionNotes);
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text("IT Helpdesk System");
                });
            }).GeneratePdf();
        }
    }
}