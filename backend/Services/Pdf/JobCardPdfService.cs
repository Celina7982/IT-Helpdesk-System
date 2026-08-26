using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using IThelpdesk.Data;
using IThelpdesk.Interfaces.Services;
using IThelpdesk.Models;

namespace IThelpdesk.Services.Pdf
{
    // ============================================================
    // 1. DI SERVICE (Implements IJobCardPdfService)
    // ============================================================
    public class JobCardPdfService : IJobCardPdfService
    {
        private readonly ApplicationDbContext _db;

        public JobCardPdfService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<byte[]> GenerateJobCardPdfAsync(int jobCardId)
        {
            var jobCard = await _db.JobCards
                .Include(j => j.Ticket)
                .Include(j => j.AssignedTechnician)
                .Include(j => j.LabourEntries)
                    .ThenInclude(l => l.Technician)
                .Include(j => j.PartsUsed)
                .FirstOrDefaultAsync(j => j.JobCardId == jobCardId);

            if (jobCard == null)
            {
                return Array.Empty<byte>();
            }

            var document = new JobCardDocument(jobCard);
            return document.GeneratePdf();
        }
    }

    // ============================================================
    // 2. QUESTPDF DOCUMENT (Layout Engine)
    // ============================================================
    public class JobCardDocument : IDocument
    {
        private readonly JobCard _jobCard;

        private const string NavyBlue = "#104A85";
        private const string TableBlue = "#155BA5";
        private const string LightBlue = "#EDF5FB";
        private const string InfoBlue = "#F3F8FC";
        private const string BorderBlue = "#D3DFE9";
        private const string LabelBlue = "#70879D";
        private const string StatusGreen = "#168A4A";
        private const string StatusBackground = "#E7F4ED";
        private const string Black = "#1F2933";
        private const string White = "#FFFFFF";

        public JobCardDocument(JobCard jobCard)
        {
            _jobCard = jobCard;
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            // PAGE 1: Summary & Details
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(32);
                page.PageColor(White);

                page.DefaultTextStyle(x => x
                    .FontFamily("Lato")
                    .FontSize(10)
                    .FontColor(Black)
                );

                page.Content().Element(ComposePageOne);
            });

            // PAGE 2: Labour & Parts
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(32);
                page.PageColor(White);

                page.DefaultTextStyle(x => x
                    .FontFamily("Lato")
                    .FontSize(10)
                    .FontColor(Black)
                );

                page.Content().Element(ComposePageTwo);
            });
        }

        private void ComposePageOne(IContainer container)
        {
            container.Column(column =>
            {
                column.Item().Element(ComposeHeader);

                column.Item()
                    .PaddingTop(12)
                    .Height(3)
                    .Background(TableBlue);

                column.Item()
                    .PaddingTop(17)
                    .Element(ComposeJobInformationHeading);

                column.Item()
                    .PaddingTop(11)
                    .Element(ComposeJobInformationTable);

                column.Item()
                    .PaddingTop(13)
                    .Element(c => ComposeInformationSection(c, "FAULT REPORTED", _jobCard.FaultReported));

                column.Item()
                    .PaddingTop(13)
                    .Element(c => ComposeInformationSection(c, "FAULT FOUND", _jobCard.FaultFound));

                column.Item()
                    .PaddingTop(13)
                    .Element(c => ComposeInformationSection(c, "WORK PERFORMED", _jobCard.WorkPerformed));

                column.Item()
                    .PaddingTop(13)
                    .Element(c => ComposeInformationSection(c, "COMPLETION NOTES", _jobCard.CompletionNotes));
            });
        }

        private void ComposePageTwo(IContainer container)
        {
            container.Column(column =>
            {
                column.Item().Element(ComposeHeader);

                column.Item()
                    .PaddingTop(12)
                    .Height(3)
                    .Background(TableBlue);

                column.Item()
                    .PaddingTop(17)
                    .Element(c => ComposePageSectionHeading(c, "LABOUR ENTRIES"));

                column.Item()
                    .PaddingTop(10)
                    .Element(ComposeLabourTable);

                column.Item()
                    .PaddingTop(17)
                    .Element(c => ComposePageSectionHeading(c, "PARTS USED"));

                column.Item()
                    .PaddingTop(10)
                    .Element(ComposePartsTable);
            });
        }

        private void ComposeHeader(IContainer container)
        {
            container.Row(row =>
            {
                row.RelativeItem()
                    .AlignLeft()
                    .AlignMiddle()
                    .Element(ComposeLogo); // Uses your new logo method

                row.ConstantItem(190)
                    .AlignRight()
                    .Column(column =>
                    {
                        column.Item()
                            .AlignRight()
                            .Text("JOB CARD")
                            .FontFamily("Lato")
                            .FontSize(24)
                            .Bold()
                            .FontColor(NavyBlue);

                        column.Item()
                            .PaddingTop(2)
                            .AlignRight()
                            .Text($"#{_jobCard.JobNumber}")
                            .FontFamily("Lato")
                            .FontSize(10.5f)
                            .FontColor("#3F4F5F");

                        column.Item()
                            .PaddingTop(8)
                            .AlignRight()
                            .Element(ComposeStatusBadge);
                    });
            });
        }

        private void ComposeLogo(IContainer container)
        {
            string logoPath = @"C:\Users\Celina\Documents\Celina code\IT-Helpdesk-System\backend\Assets\Lbc-Logo.png";

            if (File.Exists(logoPath))
            {
                container
                    .Width(135)
                    .Height(55)
                    .Image(logoPath, ImageScaling.FitArea);
            }
            else
            {
                // Fallback placeholder if the logo file is moved or deleted
                container
                    .Width(135)
                    .Height(55)
                    .Border(1)
                    .BorderColor(BorderBlue)
                    .AlignCenter()
                    .AlignMiddle()
                    .Text("LOGO")
                    .FontFamily("Lato")
                    .FontSize(11)
                    .Bold()
                    .FontColor(LabelBlue);
            }
        }
        

        private void ComposeStatusBadge(IContainer container)
        {
            string status = _jobCard.Status?.ToUpperInvariant() ?? string.Empty;

            container
                .Width(112)
                .Height(18)
                .Background(StatusBackground)
                .AlignCenter()
                .AlignMiddle()
                .Text(SpreadCharacters(status))
                .FontFamily("Lato")
                .FontSize(8)
                .Bold()
                .FontColor(StatusGreen);
        }

        private void ComposeJobInformationHeading(IContainer container)
        {
            container.Row(row =>
            {
                row.ConstantItem(4).Height(25).Background(TableBlue);

                row.RelativeItem()
                    .PaddingLeft(10)
                    .Column(column =>
                    {
                        column.Item()
                            .Text("JOB INFORMATION")
                            .FontFamily("Lato")
                            .FontSize(11)
                            .Bold()
                            .FontColor(NavyBlue);

                        column.Item()
                            .PaddingTop(1)
                            .Text("Service and assignment details")
                            .FontFamily("Lato")
                            .FontSize(8.5f)
                            .FontColor(LabelBlue);
                    });
            });
        }

        private void ComposePageSectionHeading(IContainer container, string title)
        {
            container.Row(row =>
            {
                row.ConstantItem(4).Height(25).Background(TableBlue);

                row.RelativeItem()
                    .PaddingLeft(10)
                    .AlignMiddle()
                    .Text(title)
                    .FontFamily("Lato")
                    .FontSize(11)
                    .Bold()
                    .FontColor(NavyBlue);
            });
        }

        private void ComposeJobInformationTable(IContainer container)
        {
            container
                .Border(1)
                .BorderColor(BorderBlue)
                .Background(InfoBlue)
                .Padding(10)
                .Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(108);
                        columns.RelativeColumn(1);
                        columns.ConstantColumn(108);
                        columns.RelativeColumn(1);
                    });

                    AddInfoCell(table, "Job Number", _jobCard.JobNumber);
                    AddInfoCell(table, "Status", _jobCard.Status, greenValue: _jobCard.Status == "Completed");
                    AddInfoCell(table, "Ticket Number", $"#{_jobCard.TicketId}");
                    AddInfoCell(table, "Date Created", FormatDateTime(_jobCard.DateCreated));
                    AddInfoCell(table, "Customer", !string.IsNullOrWhiteSpace(_jobCard.CustomerName) ? _jobCard.CustomerName : _jobCard.Ticket?.CustomerName ?? string.Empty);
                    AddInfoCell(table, "Date Completed", FormatDateTime(_jobCard.DateCompleted));
                    AddInfoCell(table, "Technician", _jobCard.AssignedTechnician?.FullName ?? "Unassigned");
                    AddInfoCell(table, "Document Type", "Service Job Card");
                });
        }

        private void AddInfoCell(TableDescriptor table, string label, string? value, bool greenValue = false)
        {
            table.Cell()
                .BorderBottom(1)
                .BorderColor(BorderBlue)
                .PaddingVertical(7)
                .PaddingLeft(6)
                .Text(label)
                .FontFamily("Lato")
                .FontSize(8)
                .FontColor(LabelBlue);

            var cell = table.Cell()
                .BorderBottom(1)
                .BorderColor(BorderBlue)
                .PaddingVertical(7)
                .PaddingLeft(6);

            if (greenValue)
            {
                cell.Text(value ?? string.Empty)
                    .FontFamily("Lato")
                    .FontSize(8.5f)
                    .Bold()
                    .FontColor(StatusGreen);
            }
            else
            {
                cell.Text(value ?? string.Empty)
                    .FontFamily("Lato")
                    .FontSize(8.5f)
                    .FontColor(Black);
            }
        }

        private void ComposeInformationSection(IContainer container, string title, string? value)
        {
            container
                .Border(1)
                .BorderColor(BorderBlue)
                .Column(column =>
                {
                    column.Item()
                        .Height(24)
                        .Background(LightBlue)
                        .PaddingLeft(10)
                        .Element(c =>
                        {
                            c.Row(row =>
                            {
                                row.ConstantItem(4).Height(16).Background(TableBlue);

                                row.RelativeItem()
                                    .PaddingLeft(10)
                                    .AlignMiddle()
                                    .Text(SpreadCharacters(title))
                                    .FontFamily("Lato")
                                    .FontSize(9)
                                    .Bold()
                                    .FontColor(NavyBlue);
                            });
                        });

                    var lines = GetLines(value);
                    float bodyHeight = lines.Count <= 1 ? 55 : 45 + (lines.Count * 17);

                    column.Item()
                        .MinHeight(bodyHeight)
                        .PaddingLeft(10)
                        .PaddingRight(10)
                        .PaddingTop(12)
                        .PaddingBottom(10)
                        .Column(body =>
                        {
                            foreach (string line in lines)
                            {
                                body.Item()
                                    .PaddingBottom(7)
                                    .Text(line)
                                    .FontFamily("Lato")
                                    .FontSize(9)
                                    .FontColor("#3F4F5F");
                            }
                        });
                });
        }

        private void ComposeLabourTable(IContainer container)
        {
            container.Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(110);
                    columns.ConstantColumn(128);
                    columns.ConstantColumn(57);
                    columns.RelativeColumn();
                });

                table.Header(header =>
                {
                    header.Cell().Background(TableBlue).Height(21).AlignCenter().AlignMiddle()
                        .Text("D A T E").FontFamily("Lato").FontSize(8).Bold().FontColor(White);

                    header.Cell().Background(TableBlue).Height(21).AlignCenter().AlignMiddle()
                        .Text("T E C H N I C I A N").FontFamily("Lato").FontSize(8).Bold().FontColor(White);

                    header.Cell().Background(TableBlue).Height(21).AlignCenter().AlignMiddle()
                        .Text("H O U R S").FontFamily("Lato").FontSize(8).Bold().FontColor(White);

                    header.Cell().Background(TableBlue).Height(21).AlignCenter().AlignMiddle()
                        .Text("W O R K").FontFamily("Lato").FontSize(8).Bold().FontColor(White);
                });

                decimal totalHours = 0;

                foreach (JobCardLabour labour in _jobCard.LabourEntries)
                {
                    totalHours += labour.HoursWorked;

                    table.Cell().Border(1).BorderColor(BorderBlue).Height(21).PaddingLeft(6).AlignMiddle()
                        .Text(FormatDateTime(labour.DateWorked)).FontFamily("Lato").FontSize(8).FontColor("#45586A");

                    table.Cell().Border(1).BorderColor(BorderBlue).PaddingLeft(6).AlignMiddle()
                        .Text(labour.Technician?.FullName ?? "Unassigned").FontFamily("Lato").FontSize(8).FontColor("#45586A");

                    table.Cell().Border(1).BorderColor(BorderBlue).AlignCenter().AlignMiddle()
                        .Text(labour.HoursWorked.ToString("0.00", CultureInfo.InvariantCulture)).FontFamily("Lato").FontSize(8).FontColor("#45586A");

                    table.Cell().Border(1).BorderColor(BorderBlue).PaddingLeft(6).AlignMiddle()
                        .Text(labour.WorkPerformed).FontFamily("Lato").FontSize(8).FontColor("#45586A");
                }

                if (!_jobCard.LabourEntries.Any())
                {
                    table.Cell().ColumnSpan(4).Border(1).BorderColor(BorderBlue).Height(25).PaddingLeft(6).AlignMiddle()
                        .Text("No labour entries.").FontFamily("Lato").FontSize(8).FontColor("#45586A");
                }

                table.Cell().ColumnSpan(2).Background(LightBlue).Border(1).BorderColor(BorderBlue).Height(21).PaddingRight(8).AlignRight().AlignMiddle()
                    .Text("TOTAL HOURS").FontFamily("Lato").FontSize(8).Bold().FontColor(NavyBlue);

                table.Cell().Background(LightBlue).Border(1).BorderColor(BorderBlue).AlignCenter().AlignMiddle()
                    .Text(totalHours.ToString("0.00", CultureInfo.InvariantCulture)).FontFamily("Lato").FontSize(8).Bold().FontColor(NavyBlue);

                table.Cell().Background(LightBlue).Border(1).BorderColor(BorderBlue);
            });
        }

        private void ComposePartsTable(IContainer container)
        {
            container.Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(110);
                    columns.RelativeColumn();
                    columns.ConstantColumn(57);
                });

                table.Header(header =>
                {
                    header.Cell().Background(TableBlue).Height(21).AlignCenter().AlignMiddle()
                        .Text("D A T E  A D D E D").FontFamily("Lato").FontSize(8).Bold().FontColor(White);

                    header.Cell().Background(TableBlue).Height(21).AlignCenter().AlignMiddle()
                        .Text("P A R T  N A M E").FontFamily("Lato").FontSize(8).Bold().FontColor(White);

                    header.Cell().Background(TableBlue).Height(21).AlignCenter().AlignMiddle()
                        .Text("Q T Y").FontFamily("Lato").FontSize(8).Bold().FontColor(White);
                });

                int totalQty = 0;

                foreach (JobCardPart part in _jobCard.PartsUsed)
                {
                    totalQty += part.Quantity;

                    table.Cell().Border(1).BorderColor(BorderBlue).Height(21).PaddingLeft(6).AlignMiddle()
                        .Text(FormatDateTime(part.DateAdded)).FontFamily("Lato").FontSize(8).FontColor("#45586A");

                    table.Cell().Border(1).BorderColor(BorderBlue).PaddingLeft(6).AlignMiddle()
                        .Text(part.PartName).FontFamily("Lato").FontSize(8).FontColor("#45586A");

                    table.Cell().Border(1).BorderColor(BorderBlue).AlignCenter().AlignMiddle()
                        .Text(part.Quantity.ToString(CultureInfo.InvariantCulture)).FontFamily("Lato").FontSize(8).FontColor("#45586A");
                }

                if (!_jobCard.PartsUsed.Any())
                {
                    table.Cell().ColumnSpan(3).Border(1).BorderColor(BorderBlue).Height(25).PaddingLeft(6).AlignMiddle()
                        .Text("No parts used.").FontFamily("Lato").FontSize(8).FontColor("#45586A");
                }

                table.Cell().ColumnSpan(2).Background(LightBlue).Border(1).BorderColor(BorderBlue).Height(21).PaddingRight(8).AlignRight().AlignMiddle()
                    .Text("TOTAL QUANTITY").FontFamily("Lato").FontSize(8).Bold().FontColor(NavyBlue);

                table.Cell().Background(LightBlue).Border(1).BorderColor(BorderBlue).AlignCenter().AlignMiddle()
                    .Text(totalQty.ToString(CultureInfo.InvariantCulture)).FontFamily("Lato").FontSize(8).Bold().FontColor(NavyBlue);
            });
        }

        private static string SpreadCharacters(string input) =>
            string.IsNullOrEmpty(input) ? string.Empty : string.Join(" ", input.ToCharArray());

        private static string FormatDateTime(DateTime? dateTime) =>
            dateTime?.ToString("yyyy/MM/dd HH:mm", CultureInfo.InvariantCulture) ?? string.Empty;

        private static System.Collections.Generic.List<string> GetLines(string? text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return new System.Collections.Generic.List<string> { string.Empty };

            return text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).ToList();
        }
    }
}