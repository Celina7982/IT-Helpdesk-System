using System;
using System.Security.Claims;
using System.Threading.Tasks;
using IThelpdesk.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IThelpdesk.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SlaTicketController : ControllerBase
    {
        private readonly ISlaTicketService _slaTicketService;
        private readonly ISlaTicketPdfService _slaTicketPdfService;


        //---------------------------------------------------------
        // CONSTRUCTOR
        //---------------------------------------------------------

        public SlaTicketController(
            ISlaTicketService slaTicketService,
            ISlaTicketPdfService slaTicketPdfService)
        {
            _slaTicketService = slaTicketService;
            _slaTicketPdfService = slaTicketPdfService;
        }


        //---------------------------------------------------------
        // GET ALL SLA REPORTS
        //---------------------------------------------------------

        [Authorize(Roles = "Admin,Technician")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var slaTickets =
                await _slaTicketService.GetAllAsync();

            return Ok(slaTickets);
        }


        //---------------------------------------------------------
        // GET SLA REPORT BY ID
        //---------------------------------------------------------

        [Authorize(Roles = "Admin,Technician")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var slaTicket =
                await _slaTicketService.GetByIdAsync(id);

            if (slaTicket == null)
            {
                return NotFound(new
                {
                    message = "SLA Report not found."
                });
            }

            return Ok(slaTicket);
        }


        //---------------------------------------------------------
        // GET SLA REPORT BY TICKET ID
        //---------------------------------------------------------

        [Authorize(Roles = "Admin,Technician")]
        [HttpGet("ticket/{ticketId}")]
        public async Task<IActionResult> GetByTicketId(
            int ticketId)
        {
            var slaTicket =
                await _slaTicketService
                    .GetByTicketIdAsync(ticketId);

            if (slaTicket == null)
            {
                return NotFound(new
                {
                    message =
                        "No SLA Report exists for this ticket."
                });
            }

            return Ok(slaTicket);
        }


        //---------------------------------------------------------
        // CREATE SLA REPORT FROM RESOLVED TICKET
        //---------------------------------------------------------

        [Authorize(Roles = "Admin,Technician")]
        [HttpPost("create-from-ticket/{ticketId}")]
        public async Task<IActionResult> CreateFromTicket(
            int ticketId)
        {
            //--------------------------------------------------
            // Get logged-in user ID from JWT
            //--------------------------------------------------

            var userIdClaim =
                User.FindFirst(
                    ClaimTypes.NameIdentifier
                )?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized();
            }

            if (!int.TryParse(
                    userIdClaim,
                    out int userId))
            {
                return Unauthorized();
            }

            try
            {
                var slaTicket =
                    await _slaTicketService
                        .CreateFromTicketAsync(
                            ticketId,
                            userId
                        );

                //--------------------------------------------------
                // Return only what frontend needs
                //--------------------------------------------------

                return Ok(new
                {
                    slaTicketId =
                        slaTicket.SlaTicketId,

                    slaNumber =
                        slaTicket.SlaNumber,

                    ticketId =
                        slaTicket.TicketId,

                    technicianId =
                        slaTicket.TechnicianId,

                    status =
                        slaTicket.Status
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }


        //---------------------------------------------------------
        // UPDATE SLA REPORT
        //---------------------------------------------------------

        [Authorize(Roles = "Admin,Technician")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] UpdateSlaTicketRequest request)
        {
            //--------------------------------------------------
            // Get logged-in user ID from JWT
            //--------------------------------------------------

            var userIdClaim =
                User.FindFirst(
                    ClaimTypes.NameIdentifier
                )?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized();
            }

            if (!int.TryParse(
                    userIdClaim,
                    out int userId))
            {
                return Unauthorized();
            }

            try
            {
                var slaTicket =
                    await _slaTicketService.UpdateAsync(
                        id,
                        request.WorkPerformed,
                        request.ResolutionNotes,
                        request.Status,
                        userId
                    );

                return Ok(slaTicket);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }


        //---------------------------------------------------------
        // DOWNLOAD SLA REPORT PDF
        //---------------------------------------------------------

        [Authorize(Roles = "Admin,Technician")]
        [HttpGet("{id}/pdf")]
        public async Task<IActionResult> DownloadPdf(
            int id)
        {
            //--------------------------------------------------
            // Find SLA Report
            //--------------------------------------------------

            var slaTicket =
                await _slaTicketService.GetByIdAsync(id);

            if (slaTicket == null)
            {
                return NotFound(new
                {
                    message = "SLA Report not found."
                });
            }


            //--------------------------------------------------
            // Generate PDF
            //--------------------------------------------------

            var pdf =
                await _slaTicketPdfService
                    .GenerateSlaTicketPdfAsync(id);

            if (pdf == null || pdf.Length == 0)
            {
                return NotFound(new
                {
                    message =
                        "Unable to generate SLA Report PDF."
                });
            }


            //--------------------------------------------------
            // Create PDF File Name
            //--------------------------------------------------

            var fileName =
                $"{slaTicket.SlaNumber}.pdf";


            //--------------------------------------------------
            // Return PDF
            //--------------------------------------------------

            return File(
                pdf,
                "application/pdf",
                fileName
            );
        }


        //---------------------------------------------------------
        // DELETE SLA REPORT
        //---------------------------------------------------------

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(
            int id)
        {
            //--------------------------------------------------
            // Get logged-in user ID from JWT
            //--------------------------------------------------

            var userIdClaim =
                User.FindFirst(
                    ClaimTypes.NameIdentifier
                )?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized();
            }

            if (!int.TryParse(
                    userIdClaim,
                    out int userId))
            {
                return Unauthorized();
            }

            try
            {
                await _slaTicketService.DeleteAsync(
                    id,
                    userId
                );

                return Ok(new
                {
                    message =
                        "SLA Report deleted successfully."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }
    }


    //=============================================================
    // UPDATE SLA REQUEST DTO
    //=============================================================

    public class UpdateSlaTicketRequest
    {
        public string WorkPerformed { get; set; }
            = string.Empty;

        public string ResolutionNotes { get; set; }
            = string.Empty;

        public string Status { get; set; }
            = string.Empty;
    }
}