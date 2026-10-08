using System;
using System.Collections.Generic;
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
    // 1. DI SERVICE
    // ============================================================

    public class SlaTicketPdfService : ISlaTicketPdfService
    {
        private readonly ApplicationDbContext _db;

        public SlaTicketPdfService(ApplicationDbContext db)
        {
            _db = db;
        }

        //--------------------------------------------------
        // Generate SLA PDF
        //--------------------------------------------------

        public async Task<byte[]> GenerateSlaTicketPdfAsync(
            int slaTicketId)
        {
            var slaTicket = await _db.SlaTickets
                .Include(s => s.Ticket)
                .Include(s => s.Technician)
                .FirstOrDefaultAsync(
                    s => s.SlaTicketId == slaTicketId
                );

            //--------------------------------------------------
            // SLA Report Not Found
            //--------------------------------------------------

            if (slaTicket == null)
            {
                return Array.Empty<byte>();
            }

            //--------------------------------------------------
            // Generate PDF
            //--------------------------------------------------

            var document =
                new SlaTicketDocument(slaTicket);

            return document.GeneratePdf();
        }
    }


    // ============================================================
    // 2. QUESTPDF DOCUMENT
    // ============================================================

    public class SlaTicketDocument : IDocument
    {
        private readonly SlaTicket _slaTicket;

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


        //--------------------------------------------------
        // Constructor
        //--------------------------------------------------

        public SlaTicketDocument(
            SlaTicket slaTicket)
        {
            _slaTicket = slaTicket;
        }


        //--------------------------------------------------
        // Metadata
        //--------------------------------------------------

        public DocumentMetadata GetMetadata()
            => DocumentMetadata.Default;


        //--------------------------------------------------
        // Document
        //--------------------------------------------------

        public void Compose(
            IDocumentContainer container)
        {
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

                page.Content()
                    .Element(ComposePage);
            });
        }


        //--------------------------------------------------
        // Main Page
        //--------------------------------------------------

        private void ComposePage(
            IContainer container)
        {
            container.Column(column =>
            {
                //--------------------------------------------------
                // Header
                //--------------------------------------------------

                column.Item()
                    .Element(ComposeHeader);


                //--------------------------------------------------
                // Blue Divider
                //--------------------------------------------------

                column.Item()
                    .PaddingTop(12)
                    .Height(3)
                    .Background(TableBlue);


                //--------------------------------------------------
                // Report Information
                //--------------------------------------------------

                column.Item()
                    .PaddingTop(17)
                    .Element(
                        ComposeReportInformationHeading
                    );

                column.Item()
                    .PaddingTop(11)
                    .Element(
                        ComposeReportInformationTable
                    );


                //--------------------------------------------------
                // Issue
                //--------------------------------------------------

                column.Item()
                    .PaddingTop(13)
                    .Element(c =>
                        ComposeInformationSection(
                            c,
                            "ISSUE",
                            _slaTicket.Issue
                        )
                    );


                //--------------------------------------------------
                // Resolution Notes
                //--------------------------------------------------

                column.Item()
                    .PaddingTop(13)
                    .Element(c =>
                        ComposeInformationSection(
                            c,
                            "RESOLUTION NOTES",
                            _slaTicket.ResolutionNotes
                        )
                    );
            });
        }


        //--------------------------------------------------
        // Header
        //--------------------------------------------------

        private void ComposeHeader(
            IContainer container)
        {
            container.Row(row =>
            {
                //--------------------------------------------------
                // Logo
                //--------------------------------------------------

                row.RelativeItem()
                    .AlignLeft()
                    .AlignMiddle()
                    .Element(ComposeLogo);


                //--------------------------------------------------
                // SLA Report Information
                //--------------------------------------------------

                row.ConstantItem(190)
                    .AlignRight()
                    .Column(column =>
                    {
                        column.Item()
                            .AlignRight()
                            .Text("SLA REPORT")
                            .FontFamily("Lato")
                            .FontSize(24)
                            .Bold()
                            .FontColor(NavyBlue);

                        column.Item()
                            .PaddingTop(2)
                            .AlignRight()
                            .Text(
                                $"#{_slaTicket.SlaNumber}"
                            )
                            .FontFamily("Lato")
                            .FontSize(10.5f)
                            .FontColor("#3F4F5F");

                        column.Item()
                            .PaddingTop(8)
                            .AlignRight()
                            .Element(
                                ComposeStatusBadge
                            );
                    });
            });
        }


        //--------------------------------------------------
        // Logo
        //--------------------------------------------------

        private void ComposeLogo(
            IContainer container)
        {
            string logoPath =
                @"C:\Users\Celina\Documents\Celina code\IT-Helpdesk-System\backend\Assets\Lbc-Logo.png";

            if (File.Exists(logoPath))
            {
                container
                    .Width(135)
                    .Height(55)
                    .Image(logoPath)
                    .FitArea();
            }
            else
            {
                //--------------------------------------------------
                // Fallback if logo cannot be found
                //--------------------------------------------------

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


        //--------------------------------------------------
        // Status Badge
        //--------------------------------------------------

        private void ComposeStatusBadge(
            IContainer container)
        {
            string status =
                _slaTicket.Status
                    ?.ToUpperInvariant()
                ?? string.Empty;

            container
                .Width(112)
                .Height(18)
                .Background(StatusBackground)
                .AlignCenter()
                .AlignMiddle()
                .Text(
                    SpreadCharacters(status)
                )
                .FontFamily("Lato")
                .FontSize(8)
                .Bold()
                .FontColor(StatusGreen);
        }


        //--------------------------------------------------
        // Report Information Heading
        //--------------------------------------------------

        private void ComposeReportInformationHeading(
            IContainer container)
        {
            container.Row(row =>
            {
                row.ConstantItem(4)
                    .Height(25)
                    .Background(TableBlue);

                row.RelativeItem()
                    .PaddingLeft(10)
                    .Column(column =>
                    {
                        column.Item()
                            .Text(
                                "REPORT INFORMATION"
                            )
                            .FontFamily("Lato")
                            .FontSize(11)
                            .Bold()
                            .FontColor(NavyBlue);

                        column.Item()
                            .PaddingTop(1)
                            .Text(
                                "SLA support report details"
                            )
                            .FontFamily("Lato")
                            .FontSize(8.5f)
                            .FontColor(LabelBlue);
                    });
            });
        }


        //--------------------------------------------------
        // Report Information Table
        //--------------------------------------------------

        private void ComposeReportInformationTable(
            IContainer container)
        {
            container
                .Border(1)
                .BorderColor(BorderBlue)
                .Background(InfoBlue)
                .Padding(10)
                .Table(table =>
                {
                    //--------------------------------------------------
                    // Columns
                    //--------------------------------------------------

                    table.ColumnsDefinition(
                        columns =>
                        {
                            columns.ConstantColumn(108);
                            columns.RelativeColumn(1);

                            columns.ConstantColumn(108);
                            columns.RelativeColumn(1);
                        }
                    );


                    //--------------------------------------------------
                    // Row 1
                    //--------------------------------------------------

                    AddInfoCell(
                        table,
                        "SLA Number",
                        _slaTicket.SlaNumber
                    );

                    AddInfoCell(
                        table,
                        "Status",
                        _slaTicket.Status,
                        greenValue:
                            string.Equals(
                                _slaTicket.Status,
                                "Completed",
                                StringComparison
                                    .OrdinalIgnoreCase
                            )
                    );


                    //--------------------------------------------------
                    // Row 2
                    //--------------------------------------------------

                    AddInfoCell(
                        table,
                        "Ticket Number",
                        $"#{_slaTicket.TicketId}"
                    );

                    AddInfoCell(
                        table,
                        "Date Created",
                        FormatDateTime(
                            _slaTicket.DateCreated
                        )
                    );


                    //--------------------------------------------------
                    // Row 3
                    //--------------------------------------------------

                    AddInfoCell(
                        table,
                        "Company",
                        _slaTicket.CompanyName
                    );

                    AddInfoCell(
                        table,
                        "Date Completed",
                        FormatDateTime(
                            _slaTicket.DateCompleted
                        )
                    );


                    //--------------------------------------------------
                    // Row 4
                    //--------------------------------------------------

                    AddInfoCell(
                        table,
                        "Customer",
                        _slaTicket.CustomerName
                    );

                    AddInfoCell(
                        table,
                        "Technician",
                        _slaTicket.Technician
                            ?.FullName
                        ?? "Unassigned"
                    );
                });
        }


        //--------------------------------------------------
        // Information Cell
        //--------------------------------------------------

        private void AddInfoCell(
            TableDescriptor table,
            string label,
            string? value,
            bool greenValue = false)
        {
            //--------------------------------------------------
            // Label
            //--------------------------------------------------

            table.Cell()
                .BorderBottom(1)
                .BorderColor(BorderBlue)
                .PaddingVertical(7)
                .PaddingLeft(6)
                .Text(label)
                .FontFamily("Lato")
                .FontSize(8)
                .FontColor(LabelBlue);


            //--------------------------------------------------
            // Value
            //--------------------------------------------------

            var cell = table.Cell()
                .BorderBottom(1)
                .BorderColor(BorderBlue)
                .PaddingVertical(7)
                .PaddingLeft(6);

            if (greenValue)
            {
                cell.Text(
                        value ?? string.Empty
                    )
                    .FontFamily("Lato")
                    .FontSize(8.5f)
                    .Bold()
                    .FontColor(StatusGreen);
            }
            else
            {
                cell.Text(
                        value ?? string.Empty
                    )
                    .FontFamily("Lato")
                    .FontSize(8.5f)
                    .FontColor(Black);
            }
        }


        //--------------------------------------------------
        // Issue / Resolution Notes Section
        //--------------------------------------------------

        private void ComposeInformationSection(
            IContainer container,
            string title,
            string? value)
        {
            container
                .Border(1)
                .BorderColor(BorderBlue)
                .Column(column =>
                {
                    //--------------------------------------------------
                    // Section Heading
                    //--------------------------------------------------

                    column.Item()
                        .Height(24)
                        .Background(LightBlue)
                        .PaddingLeft(10)
                        .Element(c =>
                        {
                            c.Row(row =>
                            {
                                row.ConstantItem(4)
                                    .Height(16)
                                    .Background(
                                        TableBlue
                                    );

                                row.RelativeItem()
                                    .PaddingLeft(10)
                                    .AlignMiddle()
                                    .Text(
                                        SpreadCharacters(
                                            title
                                        )
                                    )
                                    .FontFamily("Lato")
                                    .FontSize(9)
                                    .Bold()
                                    .FontColor(
                                        NavyBlue
                                    );
                            });
                        });


                    //--------------------------------------------------
                    // Section Content
                    //--------------------------------------------------

                    var lines =
                        GetLines(value);

                    float bodyHeight =
                        lines.Count <= 1
                            ? 55
                            : 45 +
                              (lines.Count * 17);

                    column.Item()
                        .MinHeight(bodyHeight)
                        .PaddingLeft(10)
                        .PaddingRight(10)
                        .PaddingTop(12)
                        .PaddingBottom(10)
                        .Column(body =>
                        {
                            foreach (
                                string line
                                in lines)
                            {
                                body.Item()
                                    .PaddingBottom(7)
                                    .Text(line)
                                    .FontFamily("Lato")
                                    .FontSize(9)
                                    .FontColor(
                                        "#3F4F5F"
                                    );
                            }
                        });
                });
        }


        //--------------------------------------------------
        // Spread Characters
        //--------------------------------------------------

        private static string SpreadCharacters(
            string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            return string.Join(
                " ",
                input.ToCharArray()
            );
        }


        //--------------------------------------------------
        // Format Date
        //--------------------------------------------------

        private static string FormatDateTime(
            DateTime? dateTime)
        {
            return dateTime?.ToString(
                "yyyy/MM/dd HH:mm",
                CultureInfo.InvariantCulture
            ) ?? string.Empty;
        }


        //--------------------------------------------------
        // Get Text Lines
        //--------------------------------------------------

        private static List<string> GetLines(
            string? text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return new List<string>
                {
                    string.Empty
                };
            }

            return text
                .Split(
                    new[]
                    {
                        "\r\n",
                        "\r",
                        "\n"
                    },
                    StringSplitOptions.None
                )
                .ToList();
        }
    }
}