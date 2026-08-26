using IThelpdesk.DTOs.JobCard;
using IThelpdesk.Enums;
using IThelpdesk.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace IThelpdesk.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class JobCardController : ControllerBase
    {
        private readonly IJobCardService _jobCardService;
        private readonly IJobCardAuditService _auditService;
        private readonly IJobCardPdfService _jobCardPdfService;

        public JobCardController(
            IJobCardService jobCardService,
            IJobCardAuditService auditService,
            IJobCardPdfService jobCardPdfService)
        {
            _jobCardService = jobCardService;
            _auditService = auditService;
            _jobCardPdfService = jobCardPdfService;
        }

        //---------------------------------------------------------
        // GET JOB CARD LIST
        //---------------------------------------------------------

        [Authorize(Roles = "Admin,Technician")]
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] bool mine = false,
            [FromQuery] string? status = null,
            [FromQuery] int? assignedTo = null,
            [FromQuery] string? search = null,
            [FromQuery] string? sortBy = null,
            [FromQuery] string? sortDirection = "desc",
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            //---------------------------------------------------------
            // Logged in User
            //---------------------------------------------------------

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) ||
                string.IsNullOrEmpty(role))
            {
                return Unauthorized();
            }

            if (!int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized();
            }

            var jobCards = await _jobCardService.GetJobCardListAsync(
                userId,
                role,
                mine,
                status,
                assignedTo,
                search,
                sortBy,
                sortDirection,
                pageNumber,
                pageSize);

            return Ok(jobCards);
        }

        //---------------------------------------------------------
        // GET JOB CARD BY TICKET ID
        //---------------------------------------------------------

        [Authorize(Roles = "Admin,Technician")]
        [HttpGet("by-ticket/{ticketId}")]
        public async Task<IActionResult> GetByTicketId(int ticketId)
        {
            var jobCard = await _jobCardService.GetByTicketIdAsync(ticketId);

            if (jobCard == null)
                return NotFound();

            return Ok(new
            {
                jobCardId = jobCard.JobCardId,
                jobNumber = jobCard.JobNumber,
                ticketId = jobCard.TicketId,
                status = jobCard.Status
            });
        }

        //---------------------------------------------------------
        // GET JOB CARD DETAILS
        //---------------------------------------------------------

        [Authorize(Roles = "Admin,Technician")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDetails(int id)
        {
            var jobCard = await _jobCardService.GetDetailsAsync(id);

            if (jobCard == null)
                return NotFound();

            return Ok(jobCard);
        }
        //---------------------------------------------------------
        // EXPORT JOB CARD PDF
        //---------------------------------------------------------
        [Authorize(Roles = "Admin,Technician")]
        [HttpGet("{id}/pdf")]
        public async Task<IActionResult> ExportPdf(int id)
        {
            try
            {
                // 1. Fetch the job card first to verify existence
                var jobCard = await _jobCardService.GetByIdAsync(id);

                if (jobCard == null)
                {
                    return NotFound($"Job Card with ID {id} was not found.");
                }

                // 2. Generate the PDF byte array
                var pdf = await _jobCardPdfService.GenerateJobCardPdfAsync(id);

                if (pdf == null || pdf.Length == 0)
                {
                    return StatusCode(500, "An error occurred while generating the PDF binary payload.");
                }

                // 3. Log the audit action safely
                int userId = jobCard.AssignedTechnicianId ?? 0;

                await _auditService.LogAsync(
                    jobCard.JobCardId,
                    userId,
                    JobCardAuditAction.PdfGenerated,
                    $"PDF generated for Job Card {jobCard.JobNumber}");

                // 4. Return the valid byte array as a downloadable PDF stream
                return File(
                    pdf,
                    "application/pdf",
                    $"JobCard-{jobCard.JobNumber ?? id.ToString()}.pdf");
            }
            catch (Exception ex)
            {
                // Print the exact error to your terminal console so you can see why it failed
                Console.WriteLine($"[PDF GENERATION ERROR]: {ex.Message}");
                Console.WriteLine($"[STACK TRACE]: {ex.StackTrace}");

                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        //---------------------------------------------------------
        // CREATE JOB CARD FROM TICKET
        //---------------------------------------------------------

        [Authorize(Roles = "Admin,Technician")]
        [HttpPost("create-from-ticket")]
        public async Task<IActionResult> CreateFromTicket(
            [FromBody] CreateJobCardDto request)
        {
            if (request == null)
                return BadRequest(new { message = "Request is required." });

            try
            {
                var jobCard =
                    await _jobCardService.CreateFromTicketAsync(request.TicketId);

                // Return ONLY the values the frontend needs.
                // Do NOT return the complete JobCard entity because
                // JobCard -> Ticket -> JobCards creates a JSON cycle.

                return Ok(new
                {
                    jobCardId = jobCard.JobCardId,
                    jobNumber = jobCard.JobNumber,
                    ticketId = jobCard.TicketId,
                    assignedTechnicianId = jobCard.AssignedTechnicianId,
                    status = jobCard.Status
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
        // ADD LABOUR
        //---------------------------------------------------------

        [Authorize(Roles = "Admin,Technician")]
        [HttpPost("{id}/labour")]
        public async Task<IActionResult> AddLabourEntry(
            int id,
            [FromBody] AddLabourEntryDto dto)
        {
            // Extract logged-in user's id from JWT claims and pass to service
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized();

            if (!int.TryParse(userIdClaim, out int userId))
                return Unauthorized();

            await _jobCardService.AddLabourEntryAsync(id, dto, userId);

            return Ok();
        }




        //---------------------------------------------------------
        // GET LABOUR ENTRIES
        //---------------------------------------------------------

        [Authorize(Roles = "Admin,Technician")]
        [HttpGet("{id}/labour")]
        public async Task<IActionResult> GetLabourEntries(int id)
        {
            var entries = await _jobCardService.GetLabourEntriesAsync(id);

            return Ok(entries);
        }

        //---------------------------------------------------------
        // UPDATE LABOUR ENTRY
        //---------------------------------------------------------

        [Authorize(Roles = "Admin,Technician")]
        [HttpPut("{id}/labour/{labourId}")]
        public async Task<IActionResult> UpdateLabourEntry(
            int id,
            int labourId,
            [FromBody] UpdateLabourEntryDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || string.IsNullOrEmpty(role))
                return Unauthorized();

            if (!int.TryParse(userIdClaim, out int userId))
                return Unauthorized();

            await _jobCardService.UpdateLabourEntryAsync(id, labourId, dto, userId, role);

            return NoContent();
        }

        //---------------------------------------------------------
        // DELETE LABOUR ENTRY
        //---------------------------------------------------------

        [Authorize(Roles = "Admin,Technician")]
        [HttpDelete("labour/{labourId}")]
        public async Task<IActionResult> DeleteLabourEntry(int labourId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || string.IsNullOrEmpty(role))
                return Unauthorized();

            if (!int.TryParse(userIdClaim, out int userId))
                return Unauthorized();

            await _jobCardService.DeleteLabourEntryAsync(labourId, userId, role);

            return NoContent();
        }

        //---------------------------------------------------------
        // ADD PART
        //---------------------------------------------------------
        // CHANGE: extract authenticated user id and role and pass them to service.
        // Reason: backend needs to record who added the part (CreatedByUserId) and enforce permissions.
        [Authorize(Roles = "Admin,Technician")]
        [HttpPost("{id}/parts")]
        public async Task<IActionResult> AddPart(
            int id,
            [FromBody] AddPartDto dto)
        {
            // Get authenticated user id and role from JWT claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) ||
                string.IsNullOrEmpty(role))
            {
                return Unauthorized();
            }

            if (!int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized();
            }

            // Pass performedByUserId and role to service so it can set CreatedByUserId and DateAdded,
            // and enforce technician/admin permissions.
            await _jobCardService.AddPartAsync(id, dto, userId, role);

            return Ok();
        }

        //---------------------------------------------------------
        // UPDATE PART
        //---------------------------------------------------------
        // CHANGE: added UpdatePart endpoint to support editing parts (Edit flow in frontend).
        // Reason: frontend needs to update part name and quantity; service enforces permissions and audits.
        [Authorize(Roles = "Admin,Technician")]
        [HttpPut("{id}/parts/{partId}")]
        public async Task<IActionResult> UpdatePart(
            int id,
            int partId,
            [FromBody] UpdatePartDto dto)
        {
            // Get authenticated user id and role from JWT claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) ||
                string.IsNullOrEmpty(role))
            {
                return Unauthorized();
            }

            if (!int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized();
            }

            // Call service with performedByUserId and role for permission checks and audit logging.
            await _jobCardService.UpdatePartAsync(id, partId, dto, userId, role);

            // Return NoContent to match typical REST update semantics.
            return NoContent();
        }

        //---------------------------------------------------------
        // DELETE PART
        //---------------------------------------------------------
        // CHANGE: extract authenticated user id and role and pass them to service.
        // Reason: service must enforce permissions and record who performed the deletion in audit.
        [Authorize(Roles = "Admin,Technician")]
        [HttpDelete("parts/{partId}")]
        public async Task<IActionResult> DeletePart(int partId)
        {
            // Get authenticated user id and role from JWT claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) ||
                string.IsNullOrEmpty(role))
            {
                return Unauthorized();
            }

            if (!int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized();
            }

            // Pass performedByUserId and role to service so it can enforce permissions and audit the deletion.
            await _jobCardService.DeletePartAsync(partId, userId, role);

            return NoContent();
        }

        //---------------------------------------------------------
        // GET PARTS
        //---------------------------------------------------------

        [Authorize(Roles = "Admin,Technician")]
        [HttpGet("{id}/parts")]
        public async Task<IActionResult> GetParts(int id)
        {
            var parts = await _jobCardService.GetPartsAsync(id);

            return Ok(parts);
        }

        //---------------------------------------------------------
        // UPDATE JOB CARD
        //---------------------------------------------------------

        [Authorize(Roles = "Admin,Technician")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] UpdateJobCardDto dto)
        {
            await _jobCardService.UpdateJobCardAsync(id, dto);

            return NoContent();
        }

        //---------------------------------------------------------
        // COMPLETE JOB CARD
        //---------------------------------------------------------

        [Authorize(Roles = "Admin,Technician")]
        [HttpPut("{id}/complete")]
        public async Task<IActionResult> Complete(int id)
        {
            await _jobCardService.CompleteJobCardAsync(id);

            return NoContent();
        }

        //---------------------------------------------------------
        // DELETE JOB CARD
        //---------------------------------------------------------

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _jobCardService.DeleteAsync(id);

            return NoContent();
        }

        //--------------------------------------------------
        // Get Audit History
        //--------------------------------------------------

        [Authorize(Roles = "Admin,Technician")]
        [HttpGet("{id}/audit")]
        public async Task<IActionResult> GetAuditHistory(int id)
        {
            var history = await _auditService.GetAuditHistoryDtoAsync(id);

            return Ok(history);
        }
    }
}
