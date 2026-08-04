using IThelpdesk.DTOs.JobCard;
using IThelpdesk.Interfaces.Repositories;
using IThelpdesk.Interfaces.Services;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace IThelpdesk.Services.Pdf
{
    /// <summary>
    /// Generates professional PDF Job Cards.
    /// </summary>
    public class JobCardPdfService : IJobCardPdfService
    {
        //------------------------------------------------------------
        // Constants
        //------------------------------------------------------------

        private const string PrimaryBlue = "#1565C0";
        private const string LightGrey = "#F5F5F5";
        private const string BorderGrey = "#DADADA";

        //------------------------------------------------------------
        // Dependencies
        //------------------------------------------------------------

        private readonly IJobCardRepository _jobCardRepository;
        private readonly IJobCardAuditService _auditService;

        //------------------------------------------------------------
        // Constructor
        //------------------------------------------------------------

        public JobCardPdfService(
            IJobCardRepository jobCardRepository,
            IJobCardAuditService auditService)
        {
            _jobCardRepository = jobCardRepository;
            _auditService = auditService;
        }

        //------------------------------------------------------------
        // Generate Job Card PDF
        //------------------------------------------------------------

        public async Task<byte[]> GenerateJobCardPdfAsync(int jobCardId)
        {
            //--------------------------------------------------------
            // Load Job Card
            //--------------------------------------------------------

            JobCardDetailsDto? jobCard =
                await _jobCardRepository.GetDetailsAsync(jobCardId);

            if (jobCard == null)
                throw new Exception("Job Card not found.");

            //--------------------------------------------------------
            // Company Logo
            //--------------------------------------------------------

            byte[]? logo = null;

            var logoPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Assets",
                "LBC-Logo.png");

            if (File.Exists(logoPath))
                logo = File.ReadAllBytes(logoPath);

            //--------------------------------------------------------
            // Status Colour
            //--------------------------------------------------------

            string statusColour = jobCard.Status switch
            {
                "Completed" => "#2E7D32",
                "In Progress" => "#1976D2",
                "Open" => "#ED6C02",
                "Cancelled" => "#C62828",
                _ => "#616161"
            };

            //--------------------------------------------------------
            // Generate PDF
            //--------------------------------------------------------

            return Document.Create(document =>
            {
            document.Page(page =>
            {
            //------------------------------------------------
            // Page Setup
            //------------------------------------------------

            page.Size(PageSizes.A4);

            page.MarginTop(25);
            page.MarginBottom(25);
            page.MarginHorizontal(30);

            page.DefaultTextStyle(x =>
                x.FontSize(10)
                 .FontFamily(Fonts.Calibri));

            //------------------------------------------------
            // Header
            //------------------------------------------------

            page.Header()
                .BorderBottom(1)
                .BorderColor(Colors.Grey.Lighten2)
                .PaddingBottom(15)
                .Row(row =>
                {
                    //------------------------------------------------
                    // Logo
                    //------------------------------------------------

                    row.ConstantItem(170)
              .PaddingRight(15)
              .AlignMiddle()
              .Element(container =>
              {
                  if (logo != null)
                  {
                      container.Image(logo);
                  }
              });

                    //------------------------------------------------
                    // Right Header
                    //------------------------------------------------

                    row.RelativeItem()
                        .AlignRight()
                        .Column(column =>
                        {
                            column.Spacing(4);

                            column.Item()
                                .AlignRight()
                                .Text("JOB CARD")
                                .FontSize(26)
                                .Bold()
                                .FontColor(PrimaryBlue);

                            column.Item()
                                .AlignRight()
                                .Text(jobCard.JobNumber)
                                .SemiBold()
                                .FontSize(14);

                            column.Item()
                                .AlignRight()
                                .Text(text =>
                                {
                                    text.Span("Status: ");

                                    text.Span(jobCard.Status)
                                        .Bold()
                                        .FontColor(statusColour);
                                });
                        });
                });

            //------------------------------------------------
            // Main Content
            //------------------------------------------------

            page.Content()
                .PaddingVertical(15)
                .Column(column =>
                {
                column.Spacing(18);

                //------------------------------------------------
                // JOB INFORMATION
                //------------------------------------------------

                column.Item()
                            .Background(PrimaryBlue)
                            .Padding(7)
                            .Text("JOB INFORMATION")
                            .Bold()
                            .FontSize(13)
                            .FontColor(Colors.White);

                column.Item()
                            .Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(170);
                                columns.RelativeColumn();
                            });

                            void AddRow(string title, string value)
                            {
                                table.Cell()
                                    .Background(LightGrey)
                                    .Border(1)
                                    .BorderColor(BorderGrey)
                                    .Padding(7)
                                    .Text(title)
                                    .SemiBold();

                                table.Cell()
                                    .Border(1)
                                    .BorderColor(BorderGrey)
                                    .Padding(7)
                                    .Text(value);
                            }

                            AddRow("Job Number", jobCard.JobNumber);

                            AddRow("Customer",
                                jobCard.CustomerName);

                            AddRow("Technician",
                                jobCard.AssignedTechnician);

                            table.Cell()
                                .Background(LightGrey)
                                .Border(1)
                                .BorderColor(BorderGrey)
                                .Padding(7)
                                .Text("Status")
                                .SemiBold();

                            table.Cell()
                                .Border(1)
                                .BorderColor(BorderGrey)
                                .Padding(5)
                                .Background(statusColour)
                                .AlignCenter()
                                .Text(jobCard.Status)
                                .FontColor(Colors.White)
                                .Bold();

                            AddRow("Date Created",
                                jobCard.DateCreated
                                    .ToString("dd MMM yyyy"));

                            AddRow("Date Completed",
                                jobCard.DateCompleted?
                                    .ToString("dd MMM yyyy")
                                    ?? "-");
                        });

                //------------------------------------------------
                // FAULT REPORTED
                //------------------------------------------------

                column.Item()
                            .Background(PrimaryBlue)
                            .Padding(7)
                            .Text("FAULT REPORTED")
                            .Bold()
                            .FontSize(13)
                            .FontColor(Colors.White);

                column.Item()
                            .Border(1)
                            .BorderColor(BorderGrey)
                            .Padding(10)
                            .MinHeight(75)
                            .Text(jobCard.FaultReported);

                    //------------------------------------------------
                    // FAULT FOUND
                    //------------------------------------------------

                    column.Item()
                        .Background(PrimaryBlue)
                        .Padding(7)
                        .Text("FAULT FOUND")
                        .Bold()
                        .FontSize(13)
                        .FontColor(Colors.White);

                    column.Item()
                        .Border(1)
                        .BorderColor(BorderGrey)
                        .Padding(10)
                        .MinHeight(75)
                        .Text(string.IsNullOrWhiteSpace(jobCard.FaultFound)
                            ? "-"
                            : jobCard.FaultFound);

                    //------------------------------------------------
                    // WORK PERFORMED
                    //------------------------------------------------

                    column.Item()
                        .Background(PrimaryBlue)
                        .Padding(7)
                        .Text("WORK PERFORMED")
                        .Bold()
                        .FontSize(13)
                        .FontColor(Colors.White);

                    column.Item()
                        .Border(1)
                        .BorderColor(BorderGrey)
                        .Padding(10)
                        .MinHeight(100)
                        .Text(string.IsNullOrWhiteSpace(jobCard.WorkPerformed)
                            ? "-"
                            : jobCard.WorkPerformed);

                    //------------------------------------------------
                    // COMPLETION NOTES
                    //------------------------------------------------

                    column.Item()
                        .Background(PrimaryBlue)
                        .Padding(7)
                        .Text("COMPLETION NOTES")
                        .Bold()
                        .FontSize(13)
                        .FontColor(Colors.White);

                    column.Item()
                        .Border(1)
                        .BorderColor(BorderGrey)
                        .Padding(10)
                        .MinHeight(80)
                        .Text(string.IsNullOrWhiteSpace(jobCard.CompletionNotes)
                            ? "-"
                            : jobCard.CompletionNotes);

                    //------------------------------------------------
                    // LABOUR ENTRIES
                    //------------------------------------------------

                    column.Item()
                        .Background(PrimaryBlue)
                        .Padding(7)
                        .Text("LABOUR ENTRIES")
                        .Bold()
                        .FontSize(13)
                        .FontColor(Colors.White);

                    column.Item()
                        .Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(90);
                                columns.RelativeColumn();
                                columns.ConstantColumn(70);
                                columns.RelativeColumn();
                            });

                            //------------------------------------------------
                            // Header
                            //------------------------------------------------

                            table.Header(header =>
                            {
                                header.Cell()
                                    .Background(PrimaryBlue)
                                    .Border(1)
                                    .BorderColor(BorderGrey)
                                    .Padding(6)
                                    .Text("Date")
                                    .Bold()
                                    .FontColor(Colors.White);

                                header.Cell()
                                    .Background(PrimaryBlue)
                                    .Border(1)
                                    .BorderColor(BorderGrey)
                                    .Padding(6)
                                    .Text("Technician")
                                    .Bold()
                                    .FontColor(Colors.White);

                                header.Cell()
                                    .Background(PrimaryBlue)
                                    .Border(1)
                                    .BorderColor(BorderGrey)
                                    .Padding(6)
                                    .AlignCenter()
                                    .Text("Hours")
                                    .Bold()
                                    .FontColor(Colors.White);

                                header.Cell()
                                    .Background(PrimaryBlue)
                                    .Border(1)
                                    .BorderColor(BorderGrey)
                                    .Padding(6)
                                    .Text("Work Performed")
                                    .Bold()
                                    .FontColor(Colors.White);
                            });

                            //------------------------------------------------
                            // Rows
                            //------------------------------------------------

                            if (jobCard.LabourEntries.Any())
                            {
                                bool alternate = false;

                                foreach (var labour in jobCard.LabourEntries)
                                {
                                    string background =
                                        alternate
                                            ? Colors.Grey.Lighten5
                                            : Colors.White;

                                    alternate = !alternate;

                                    table.Cell()
                                        .Background(background)
                                        .Border(1)
                                        .BorderColor(BorderGrey)
                                        .Padding(5)
                                        .Text(labour.DateWorked.ToString("dd/MM/yyyy"));

                                    table.Cell()
                                        .Background(background)
                                        .Border(1)
                                        .BorderColor(BorderGrey)
                                        .Padding(5)
                                        .Text(labour.TechnicianName);

                                    table.Cell()
                                        .Background(background)
                                        .Border(1)
                                        .BorderColor(BorderGrey)
                                        .Padding(5)
                                        .AlignCenter()
                                        .Text(labour.HoursWorked.ToString("0.##"));

                                    table.Cell()
                                        .Background(background)
                                        .Border(1)
                                        .BorderColor(BorderGrey)
                                        .Padding(5)
                                        .Text(labour.WorkPerformed);
                                }
                            }
                            else
                            {
                                table.Cell()
                                    .ColumnSpan(4)
                                    .Border(1)
                                    .BorderColor(BorderGrey)
                                    .Padding(8)
                                    .AlignCenter()
                                    .Text("No labour entries recorded.");
                            }
                        });

                    //------------------------------------------------
                    // PARTS USED
                    //------------------------------------------------

                    column.Item()
                        .Background(PrimaryBlue)
                        .Padding(7)
                        .Text("PARTS USED")
                        .Bold()
                        .FontSize(13)
                        .FontColor(Colors.White);

                    column.Item()
                        .Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.ConstantColumn(90);
                            });

                            table.Header(header =>
                            {
                                header.Cell()
                                    .Background(PrimaryBlue)
                                    .Border(1)
                                    .BorderColor(BorderGrey)
                                    .Padding(6)
                                    .Text("Part")
                                    .Bold()
                                    .FontColor(Colors.White);

                                header.Cell()
                                    .Background(PrimaryBlue)
                                    .Border(1)
                                    .BorderColor(BorderGrey)
                                    .Padding(6)
                                    .AlignCenter()
                                    .Text("Qty")
                                    .Bold()
                                    .FontColor(Colors.White);
                            });

                            if (jobCard.PartsUsed.Any())
                            {
                                bool alternate = false;

                                foreach (var part in jobCard.PartsUsed)
                                {
                                    string background =
                                        alternate
                                            ? Colors.Grey.Lighten5
                                            : Colors.White;

                                    alternate = !alternate;

                                    table.Cell()
                                        .Background(background)
                                        .Border(1)
                                        .BorderColor(BorderGrey)
                                        .Padding(5)
                                        .Text(part.PartName);

                                    table.Cell()
                                        .Background(background)
                                        .Border(1)
                                        .BorderColor(BorderGrey)
                                        .Padding(5)
                                        .AlignCenter()
                                        .Text(part.Quantity.ToString());
                                }
                            }
                            else
                            {
                                table.Cell()
                                    .ColumnSpan(2)
                                    .Border(1)
                                    .BorderColor(BorderGrey)
                                    .Padding(8)
                                    .AlignCenter()
                                    .Text("No parts were used.");
                            }
                        });

                    //------------------------------------------------
                    // CUSTOMER ACCEPTANCE
                    //------------------------------------------------

                    column.Item()
                        .Background(PrimaryBlue)
                        .Padding(7)
                        .Text("CUSTOMER ACCEPTANCE")
                        .Bold()
                        .FontSize(13)
                        .FontColor(Colors.White);

                    column.Item()
                        .Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(180);
                                columns.RelativeColumn();
                            });

                            void AcceptanceRow(string title, string value)
                            {
                                table.Cell()
                                    .Background(LightGrey)
                                    .Border(1)
                                    .BorderColor(BorderGrey)
                                    .Padding(7)
                                    .Text(title)
                                    .SemiBold();

                                table.Cell()
                                    .Border(1)
                                    .BorderColor(BorderGrey)
                                    .Padding(7)
                                    .Text(value);
                            }

                            AcceptanceRow(
                                "Customer",
                                jobCard.CustomerName);

                            AcceptanceRow(
                                "Signature",
                                string.IsNullOrWhiteSpace(jobCard.CustomerSignature)
                                    ? "Not Captured"
                                    : jobCard.CustomerSignature);

                            AcceptanceRow(
                                "Signed Date",
                                jobCard.SignedDate?.ToString("dd MMM yyyy")
                                    ?? "-");
                        });

                });

                //--------------------------------------------------------
                // FOOTER
                //--------------------------------------------------------

                page.Footer()
                    .BorderTop(1)
                    .BorderColor(Colors.Grey.Lighten2)
                    .PaddingTop(10)
                    .Row(row =>
                    {
                        //------------------------------------------------
                        // Company Details
                        //------------------------------------------------

                        row.RelativeItem()
                            .Column(column =>
                            {
                                column.Spacing(2);

                                column.Item()
                                    .Text("LBC Evolve IT")
                                    .Bold()
                                    .FontSize(10);

                                column.Item()
                                    .Text("189 Josiah Gumede Road");

                                column.Item()
                                    .Text("Cowies Hill Park");

                                column.Item()
                                    .Text("Pinetown");

                                column.Item()
                                    .Text("3610");

                                column.Item()
                                    .Hyperlink("tel:+27317090239")
                                    .Text("DUR  +27 31 709 0239");

                                column.Item()
                                    .Hyperlink("tel:+27110238170")
                                    .Text("JHB  +27 11 023 8170");

                                column.Item()
                                    .Hyperlink("mailto:Support@lbcit.co.za")
                                    .Text("Support@lbcit.co.za")
                                    .FontColor(Colors.Blue.Medium);

                                column.Item()
                                    .Hyperlink("https://www.lbcit.co.za")
                                    .Text("https://www.lbcit.co.za")
                                    .FontColor(Colors.Blue.Medium);
                            });

                        //------------------------------------------------
                        // Generated Date
                        //------------------------------------------------

                        row.RelativeItem()
                            .AlignCenter()
                            .Column(column =>
                            {
                                column.Item()
                                    .Text("Generated")
                                    .Bold();

                                column.Item()
                                    .Text(DateTime.Now.ToString("dd MMM yyyy"));

                                column.Item()
                                    .Text(DateTime.Now.ToString("HH:mm:ss"));
                            });

                        //------------------------------------------------
                        // Page Numbers
                        //------------------------------------------------

                        row.RelativeItem()
                            .AlignRight()
                            .Column(column =>
                            {
                                column.Item()
                                    .AlignRight()
                                    .Text(text =>
                                    {
                                        text.Span("Page ");
                                        text.CurrentPageNumber();
                                        text.Span(" of ");
                                        text.TotalPages();
                                    });

                                column.Item()
                                    .AlignRight()
                                    .Text("IT Helpdesk Management System")
                                    .FontSize(9)
                                    .FontColor(Colors.Grey.Darken1);

                                column.Item()
                                    .AlignRight()
                                    .Text("© LBC Evolve IT")
                                    .FontSize(9)
                                    .FontColor(Colors.Grey.Darken1);
                            });
                    });
            });
            }).GeneratePdf();
        }
    }
}