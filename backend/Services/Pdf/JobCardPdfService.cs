using IThelpdesk.DTOs.JobCard;
using IThelpdesk.Interfaces.Repositories;
using IThelpdesk.Interfaces.Services;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace IThelpdesk.Services.Pdf
{
    public class JobCardPdfService : IJobCardPdfService
    {
        private readonly IJobCardRepository _jobCardRepository;

        //---------------------------------------------------------
        // Brand / Design System
        //
        // These colours are intentionally kept in one place.
        // This makes it easy to reuse the same design system
        // later when we build the Pastel Invoice PDF.
        //---------------------------------------------------------

        private const string PrimaryBlue = "#1455A0";
        private const string DarkBlue = "#0E3D73";
        private const string LightBlue = "#EEF5FC";
        private const string PaleBlue = "#F7FAFD";

        private const string BorderGrey = "#D9E1EA";
        private const string MediumGrey = "#8A98A8";
        private const string DarkText = "#202B36";
        private const string BodyText = "#46525F";
        private const string White = "#FFFFFF";

        private const string SuccessGreen = "#2E8B57";
        private const string SuccessBackground = "#EAF6EF";

        private const string WarningOrange = "#C98220";
        private const string WarningBackground = "#FFF5E6";

        private const string NeutralBackground = "#F0F3F6";

        //---------------------------------------------------------
        // Constructor
        //---------------------------------------------------------

        public JobCardPdfService(
            IJobCardRepository jobCardRepository)
        {
            _jobCardRepository = jobCardRepository;
        }


        //---------------------------------------------------------
        // Generate Job Card PDF
        //---------------------------------------------------------

        public async Task<byte[]> GenerateJobCardPdfAsync(int jobCardId)
        {
            //---------------------------------------------------------
            // Get Job Card Details
            //---------------------------------------------------------

            var jobCard =
                await _jobCardRepository.GetDetailsAsync(jobCardId);

            if (jobCard == null)
                throw new Exception("Job Card not found.");

            //---------------------------------------------------------
            // Get Labour Entries
            //---------------------------------------------------------

            var labourEntries =
                await _jobCardRepository.GetLabourEntriesAsync(jobCardId);

            //---------------------------------------------------------
            // Get Parts
            //---------------------------------------------------------

            var parts =
                await _jobCardRepository.GetPartsAsync(jobCardId);

            //---------------------------------------------------------
            // Load Logo
            //---------------------------------------------------------

            var logoBytes = await LoadLogoAsync();





            //---------------------------------------------------------
            // Generate PDF
            //---------------------------------------------------------

            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    //-------------------------------------------------
                    // Page Setup
                    //-------------------------------------------------

                    page.Size(PageSizes.A4);

                    page.MarginTop(28);
                    page.MarginBottom(42);
                    page.MarginLeft(32);
                    page.MarginRight(32);

                    //-------------------------------------------------
                    // Header
                    //-------------------------------------------------

                    page.Header()
                        .Element(header =>
                        {
                            BuildHeader(
                                header,
                                jobCard,
                                logoBytes);
                        });

                    //-------------------------------------------------
                    // Content
                    //-------------------------------------------------

                    page.Content()
                        .Element(content =>
                        {
                            BuildContent(
                                content,
                                jobCard,
                                labourEntries,
                                parts);
                        });

                   
                });
            })
            .GeneratePdf();
        }

        //---------------------------------------------------------
        // Load Logo
        //---------------------------------------------------------

        private static async Task<byte[]?> LoadLogoAsync()
        {
            //---------------------------------------------------------
            // First try the application's output directory.
            //---------------------------------------------------------

            var outputDirectoryPath = Path.Combine(
                AppContext.BaseDirectory,
                "Assets",
                "Lbc-Logo.png");

            if (File.Exists(outputDirectoryPath))
            {
                return await File.ReadAllBytesAsync(
                    outputDirectoryPath);
            }

            //---------------------------------------------------------
            // Fallback to the project directory.
            //
            // This is useful during development if the image has
            // not yet been copied to bin/Debug/net10.0/Assets.
            //---------------------------------------------------------

            var projectDirectoryPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Assets",
                "Lbc-Logo.png");

            if (File.Exists(projectDirectoryPath))
            {
                return await File.ReadAllBytesAsync(
                    projectDirectoryPath);
            }

            //---------------------------------------------------------
            // Logo not found.
            //
            // The PDF will still generate using the text fallback.
            //---------------------------------------------------------

            return null;
        }

        //---------------------------------------------------------
        // Header
        //---------------------------------------------------------

        private static void BuildHeader(
            IContainer container,
            JobCardDetailsDto jobCard,
            byte[]? logoBytes)
        {
            container.Column(column =>
            {
                //-----------------------------------------------------
                // Main Header Row
                //-----------------------------------------------------

                column.Item()
                    .PaddingBottom(12)
                    .Row(row =>
                    {
                        //-------------------------------------------------
                        // Company Logo
                        //-------------------------------------------------

                        row.RelativeItem(1.6f)
                            .AlignLeft()
                            .AlignMiddle()
                            .Element(logoContainer =>
                            {
                                if (logoBytes != null)
                                {
                                    logoContainer
                                        .Height(62)
                                        .Image(logoBytes)
                                        .FitHeight();
                                }
                                else
                                {
                                    logoContainer
                                        .Column(fallback =>
                                        {
                                            fallback.Item()
                                                .Text("LBC")
                                                .FontSize(25)
                                                .Bold()
                                                .FontColor(PrimaryBlue);

                                            fallback.Item()
                                                .Text("IT SOLUTIONS")
                                                .FontSize(8)
                                                .Bold()
                                                .LetterSpacing(1.2f)
                                                .FontColor(DarkBlue);
                                        });
                                }
                            });

                        //-------------------------------------------------
                        // Document Information
                        //-------------------------------------------------

                        row.RelativeItem(1)
                            .AlignRight()
                            .Column(document =>
                            {
                                document.Item()
                                    .AlignRight()
                                    .Text("JOB CARD")
                                    .FontSize(24)
                                    .Bold()
                                    .FontColor(DarkBlue);

                                document.Item()
                                    .PaddingTop(3)
                                    .AlignRight()
                                    .Text($"#{jobCard.JobNumber}")
                                    .FontSize(11)
                                    .SemiBold()
                                    .FontColor(BodyText);

                                document.Item()
                                    .PaddingTop(7)
                                    .AlignRight()
                                    .Element(status =>
                                    {
                                        BuildStatusBadge(
                                            status,
                                            jobCard.Status);
                                    });
                            });
                    });

                //-----------------------------------------------------
                // Brand Divider
                //-----------------------------------------------------

                column.Item()
                    .Height(3)
                    .Background(PrimaryBlue);

                
            });
        }

        //---------------------------------------------------------
        // Status Badge
        //---------------------------------------------------------

        private static void BuildStatusBadge(
            IContainer container,
            string? status)
        {
            var cleanStatus =
                string.IsNullOrWhiteSpace(status)
                    ? "Unknown"
                    : status.Trim();

            var isCompleted =
                string.Equals(
                    cleanStatus,
                    "Completed",
                    StringComparison.OrdinalIgnoreCase);

            var background =
                isCompleted
                    ? SuccessBackground
                    : WarningBackground;

            var textColour =
                isCompleted
                    ? SuccessGreen
                    : WarningOrange;

            container
                .Background(background)
                .PaddingHorizontal(10)
                .PaddingVertical(4)
                .AlignCenter()
                .Text(cleanStatus.ToUpperInvariant())
                .FontSize(7.5f)
                .Bold()
                .LetterSpacing(0.7f)
                .FontColor(textColour);
        }

        //---------------------------------------------------------
        // Main Content
        //---------------------------------------------------------

        private static void BuildContent(
            IContainer container,
            JobCardDetailsDto jobCard,
            IEnumerable<dynamic> labourEntries,
            IEnumerable<dynamic> parts)
        {
            container
                .PaddingTop(16)
                .Column(column =>
                {
                    column.Spacing(14);

                    //-------------------------------------------------
                    // Job Information
                    //-------------------------------------------------

                    column.Item()
                        .Element(section =>
                        {
                            BuildSectionHeader(
                                section,
                                "JOB INFORMATION",
                                "Service and assignment details");
                        });

                    column.Item()
                        .Element(info =>
                        {
                            BuildJobInformation(
                                info,
                                jobCard);
                        });

                    //-------------------------------------------------
                    // Fault Reported
                    //-------------------------------------------------

                    column.Item()
                        .Element(section =>
                        {
                            BuildTextSection(
                                section,
                                "FAULT REPORTED",
                                jobCard.FaultReported);
                        });

                    //-------------------------------------------------
                    // Fault Found
                    //-------------------------------------------------

                    column.Item()
                        .Element(section =>
                        {
                            BuildTextSection(
                                section,
                                "FAULT FOUND",
                                jobCard.FaultFound);
                        });

                    //-------------------------------------------------
                    // Work Performed
                    //-------------------------------------------------

                    column.Item()
                        .Element(section =>
                        {
                            BuildTextSection(
                                section,
                                "WORK PERFORMED",
                                jobCard.WorkPerformed);
                        });

                    //-------------------------------------------------
                    // Completion Notes
                    //-------------------------------------------------

                    column.Item()
                        .Element(section =>
                        {
                            BuildTextSection(
                                section,
                                "COMPLETION NOTES",
                                jobCard.CompletionNotes);
                        });

                    //-------------------------------------------------
                    // Labour
                    //-------------------------------------------------

                    column.Item()
                        .Element(section =>
                        {
                            BuildLabourTable(
                                section,
                                labourEntries);
                        });

                    //-------------------------------------------------
                    // Parts
                    //-------------------------------------------------

                    column.Item()
                        .Element(section =>
                        {
                            BuildPartsTable(
                                section,
                                parts);
                        });

                   

                });
        }

        //---------------------------------------------------------
        // Section Header
        //---------------------------------------------------------

        private static void BuildSectionHeader(
            IContainer container,
            string title,
            string subtitle)
        {
            container
                .Column(column =>
                {
                    column.Item()
                        .Row(row =>
                        {
                            row.AutoItem()
                                .Width(4)
                                .Height(17)
                                .Background(PrimaryBlue);

                            row.RelativeItem()
                                .PaddingLeft(8)
                                .Column(text =>
                                {
                                    text.Item()
                                        .Text(title)
                                        .FontSize(10.5f)
                                        .Bold()
                                        .FontColor(DarkBlue);

                                    text.Item()
                                        .PaddingTop(1)
                                        .Text(subtitle)
                                        .FontSize(7.5f)
                                        .FontColor(MediumGrey);
                                });
                        });

                    column.Item()
                        .PaddingTop(7)
                        .Height(1)
                        .Background(BorderGrey);
                });
        }

        //---------------------------------------------------------
        // Job Information
        //---------------------------------------------------------

        private static void BuildJobInformation(
            IContainer container,
            JobCardDetailsDto jobCard)
        {
            container
                .Border(1)
                .BorderColor(BorderGrey)
                .Background(PaleBlue)
                .Padding(10)
                .Table(table =>
                {
                    //-------------------------------------------------
                    // Four Columns
                    //-------------------------------------------------

                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(1.05f);
                        columns.RelativeColumn(1.45f);
                        columns.RelativeColumn(1.05f);
                        columns.RelativeColumn(1.45f);
                    });

                    //-------------------------------------------------
                    // Row 1
                    //-------------------------------------------------

                    AddInfoLabel(table, "Job Number");
                    AddInfoValue(table, jobCard.JobNumber);

                    AddInfoLabel(table, "Status");
                    AddInfoStatusValue(table, jobCard.Status);

                    //-------------------------------------------------
                    // Row 2
                    //-------------------------------------------------

                    AddInfoLabel(table, "Ticket Number");
                    AddInfoValue(table, $"#{jobCard.TicketId}");

                    AddInfoLabel(table, "Date Created");
                    AddInfoValue(
                        table,
                        FormatDate(jobCard.DateCreated));

                    //-------------------------------------------------
                    // Row 3
                    //-------------------------------------------------

                    AddInfoLabel(table, "Customer");
                    AddInfoValue(table, jobCard.CustomerName);

                    AddInfoLabel(table, "Date Completed");
                    AddInfoValue(
                        table,
                        jobCard.DateCompleted.HasValue
                            ? FormatDate(jobCard.DateCompleted.Value)
                            : "-");

                    //-------------------------------------------------
                    // Row 4
                    //-------------------------------------------------

                    AddInfoLabel(table, "Technician");
                    AddInfoValue(
                        table,
                        jobCard.AssignedTechnician);

                    AddInfoLabel(table, "Document Type");
                    AddInfoValue(table, "Service Job Card");
                });
        }

        //---------------------------------------------------------
        // Information Label
        //---------------------------------------------------------

        private static void AddInfoLabel(
            TableDescriptor table,
            string text)
        {
            table.Cell()
                .Background(White)
                .BorderBottom(1)
                .BorderColor(BorderGrey)
                .PaddingVertical(7)
                .PaddingHorizontal(6)
                .Text(text)
                .FontSize(7.5f)
                .Bold()
                .FontColor(MediumGrey);
        }

        //---------------------------------------------------------
        // Information Value
        //---------------------------------------------------------

        private static void AddInfoValue(
            TableDescriptor table,
            string? text)
        {
            table.Cell()
                .BorderBottom(1)
                .BorderColor(BorderGrey)
                .PaddingVertical(7)
                .PaddingHorizontal(6)
                .Text(
                    string.IsNullOrWhiteSpace(text)
                        ? "-"
                        : text)
                .FontSize(8.5f)
                .SemiBold()
                .FontColor(DarkText);
        }

        //---------------------------------------------------------
        // Information Status
        //---------------------------------------------------------

        private static void AddInfoStatusValue(
            TableDescriptor table,
            string? status)
        {
            var cleanStatus =
                string.IsNullOrWhiteSpace(status)
                    ? "Unknown"
                    : status.Trim();

            var isCompleted =
                string.Equals(
                    cleanStatus,
                    "Completed",
                    StringComparison.OrdinalIgnoreCase);

            table.Cell()
                .BorderBottom(1)
                .BorderColor(BorderGrey)
                .PaddingVertical(7)
                .PaddingHorizontal(6)
                .Text(cleanStatus)
                .FontSize(8.5f)
                .Bold()
                .FontColor(
                    isCompleted
                        ? SuccessGreen
                        : WarningOrange);
        }

        //---------------------------------------------------------
        // Text Section
        //---------------------------------------------------------

        private static void BuildTextSection(
            IContainer container,
            string title,
            string? text)
        {
            container
                .Border(1)
                .BorderColor(BorderGrey)
                .Background(White)
                .Column(column =>
                {
                    //-------------------------------------------------
                    // Section Heading
                    //-------------------------------------------------

                    column.Item()
                        .Background(LightBlue)
                        .PaddingVertical(6)
                        .PaddingHorizontal(9)
                        .Row(row =>
                        {
                            row.AutoItem()
                                .Width(3)
                                .Height(12)
                                .Background(PrimaryBlue);

                            row.RelativeItem()
                                .PaddingLeft(7)
                                .Text(title)
                                .FontSize(8.5f)
                                .Bold()
                                .LetterSpacing(0.5f)
                                .FontColor(DarkBlue);
                        });

                    //-------------------------------------------------
                    // Body
                    //-------------------------------------------------

                    column.Item()
                        .Padding(10)
                        .MinHeight(34)
                        .Text(
                            string.IsNullOrWhiteSpace(text)
                                ? "-"
                                : text.Trim())
                        .FontSize(9)
                        .LineHeight(1.3f)
                        .FontColor(BodyText);
                });
        }

        //---------------------------------------------------------
        // Labour Table
        //---------------------------------------------------------

        private static void BuildLabourTable(
            IContainer container,
            IEnumerable<dynamic> labourEntries)
        {
            var entries = labourEntries.ToList();

            // Keep the entire labour section together on a single page when it fits.
            // ShowEntire() is applied to the outer container for the labour section
            // to prevent the heading, table and total row from splitting across pages.
            container.ShowEntire().Column(column =>
            {
                //-----------------------------------------------------
                // Heading
                //-----------------------------------------------------

                column.Item()
                    .Row(row =>
                    {
                        row.AutoItem()
                            .Width(4)
                            .Height(16)
                            .Background(PrimaryBlue);

                        row.RelativeItem()
                            .PaddingLeft(8)
                            .Text("LABOUR ENTRIES")
                            .FontSize(10.5f)
                            .Bold()
                            .FontColor(DarkBlue);
                    });

                column.Item()
                    .PaddingTop(7)
                    .Table(table =>
                    {
                        //-------------------------------------------------
                        // Columns
                        //-------------------------------------------------

                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(1.15f);
                            columns.RelativeColumn(1.35f);
                            columns.RelativeColumn(0.6f);
                            columns.RelativeColumn(2.5f);
                        });

                        //-------------------------------------------------
                        // Header
                        //-------------------------------------------------

                        table.Header(header =>
                        {
                            AddTableHeader(
                                header,
                                "DATE");

                            AddTableHeader(
                                header,
                                "TECHNICIAN");

                            AddTableHeader(
                                header,
                                "HOURS");

                            AddTableHeader(
                                header,
                                "WORK");
                        });

                        //-------------------------------------------------
                        // Rows
                        //-------------------------------------------------

                        for (int i = 0; i < entries.Count; i++)
                        {
                            var labour = entries[i];

                            var background =
                                i % 2 == 0
                                    ? White
                                    : PaleBlue;

                            AddTableCell(
                                table,
                                FormatDate(
                                    labour.DateWorked),
                                false,
                                background);

                            AddTableCell(
                                table,
                                labour.CreatedByUser != null
                                    ? $"{labour.CreatedByUser.FirstName} {labour.CreatedByUser.LastName}"
                                    : "Not Assigned",
                                false,
                                background);

                            AddTableCell(
                                table,
                                labour.HoursWorked.ToString("0.00"),
                                true,
                                background);

                            AddTableCell(
                                table,
                                labour.WorkPerformed,
                                false,
                                background);
                        }

                        //-------------------------------------------------
                        // Empty State
                        //-------------------------------------------------

                        if (!entries.Any())
                        {
                            AddTableCell(
                                table,
                                "No labour entries recorded.",
                                false,
                                PaleBlue);

                            AddTableCell(
                                table,
                                "-",
                                false,
                                PaleBlue);

                            AddTableCell(
                                table,
                                "-",
                                true,
                                PaleBlue);

                            AddTableCell(
                                table,
                                "-",
                                false,
                                PaleBlue);
                        }

                        //-------------------------------------------------
                        // Total Hours
                        //-------------------------------------------------

                        var totalHours =
                            entries.Sum(x =>
                                (decimal)x.HoursWorked);

                        table.Cell()
                            .ColumnSpan(2)
                            .Background(LightBlue)
                            .Border(1)
                            .BorderColor(BorderGrey)
                            .Padding(7)
                            .AlignRight()
                            .Text("TOTAL HOURS")
                            .FontSize(8)
                            .Bold()
                            .FontColor(DarkBlue);

                        table.Cell()
                            .Background(LightBlue)
                            .Border(1)
                            .BorderColor(BorderGrey)
                            .Padding(7)
                            .AlignCenter()
                            .Text(totalHours.ToString("0.00"))
                            .FontSize(9)
                            .Bold()
                            .FontColor(DarkBlue);

                        table.Cell()
                            .Background(LightBlue)
                            .Border(1)
                            .BorderColor(BorderGrey)
                            .Padding(7)
                            .Text("");
                    });
            });
        }

        //---------------------------------------------------------
        // Parts Table
        //---------------------------------------------------------

        private static void BuildPartsTable(
            IContainer container,
            IEnumerable<dynamic> parts)
        {
            var partEntries = parts.ToList();

            // Keep the entire parts section together on a single page when it fits.
            // Apply ShowEntire() to the outer container so heading, divider and
            // the whole parts table move together.
            container.ShowEntire().Column(column =>
            {
                //-----------------------------------------------------
                // Heading
                //-----------------------------------------------------

                column.Item()
                    .Row(row =>
                    {
                        row.AutoItem()
                            .Width(4)
                            .Height(16)
                            .Background(PrimaryBlue);

                        row.RelativeItem()
                            .PaddingLeft(8)
                            .Text("PARTS USED")
                            .FontSize(10.5f)
                            .Bold()
                            .FontColor(DarkBlue);
                    });

                //-----------------------------------------------------
                // Table
                //-----------------------------------------------------

                column.Item()
                    .PaddingTop(7)
                    .Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(3.8f);
                            columns.RelativeColumn(0.9f);
                        });

                        //-------------------------------------------------
                        // Header
                        //-------------------------------------------------

                        table.Header(header =>
                        {
                            AddTableHeader(
                                header,
                                "PART");

                            AddTableHeader(
                                header,
                                "QUANTITY");
                        });

                        //-------------------------------------------------
                        // Rows
                        //-------------------------------------------------

                        for (int i = 0; i < partEntries.Count; i++)
                        {
                            var part = partEntries[i];

                            var background =
                                i % 2 == 0
                                    ? White
                                    : PaleBlue;

                            AddTableCell(
                                table,
                                part.PartName,
                                false,
                                background);

                            AddTableCell(
                                table,
                                part.Quantity.ToString(),
                                true,
                                background);
                        }

                        //-------------------------------------------------
                        // Empty State
                        //-------------------------------------------------

                        if (!partEntries.Any())
                        {
                            AddTableCell(
                                table,
                                "No parts recorded.",
                                false,
                                PaleBlue);

                            AddTableCell(
                                table,
                                "-",
                                true,
                                PaleBlue);
                        }
                    });
            });
        }

        //---------------------------------------------------------
        // Table Header
        //---------------------------------------------------------

        private static void AddTableHeader(
            TableCellDescriptor header,
            string text)
        {
            header.Cell()
                .Background(PrimaryBlue)
                .Border(1)
                .BorderColor(PrimaryBlue)
                .PaddingVertical(6)
                .PaddingHorizontal(5)
                .AlignCenter()
                .Text(text)
                .Bold()
                .FontSize(7.5f)
                .LetterSpacing(0.4f)
                .FontColor(White);
        }


        //---------------------------------------------------------
        // Table Cell
        //---------------------------------------------------------

        private static void AddTableCell(
            TableDescriptor table,
            string? text,
            bool center = false,
            string background = White)
        {
            var cell = table.Cell()
                .Background(background)
                .Border(1)
                .BorderColor(BorderGrey)
                .PaddingVertical(6)
                .PaddingHorizontal(5);

            var displayText =
                string.IsNullOrWhiteSpace(text)
                    ? "-"
                    : text.Trim();

            //-----------------------------------------------------
            // IMPORTANT:
            // AlignCenter() must be chained to the Text container.
            //-----------------------------------------------------

            if (center)
            {
                cell
                    .AlignCenter()
                    .Text(displayText)
                    .FontSize(7.8f)
                    .FontColor(BodyText);
            }
            else
            {
                cell
                    .Text(displayText)
                    .FontSize(7.8f)
                    .FontColor(BodyText);
            }
        }

        

                        

        //---------------------------------------------------------
        // Date Formatting
        //---------------------------------------------------------

        private static string FormatDate(
            DateTime date)
        {
            return date
                .ToLocalTime()
                .ToString("dd MMM yyyy hh:mm tt");
        }
    }
}