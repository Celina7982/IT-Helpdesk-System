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
            var jobCard = await _jobCardService.GetByIdAsync(id);

            if (jobCard == null)
            {
                return NotFound();
            }

            var pdf = await _jobCardPdfService.GenerateJobCardPdfAsync(id);

            int userId = jobCard.AssignedTechnicianId ?? 0;

            await _auditService.LogAsync(
                jobCard.JobCardId,
                userId,
                JobCardAuditAction.PdfGenerated,
                $"PDF generated for Job Card {jobCard.JobNumber}");

            return File(
                pdf,
                "application/pdf",
                $"JobCard-{id}.pdf");
        }


        //---------------------------------------------------------
        // CREATE JOB CARD
        //---------------------------------------------------------

        [Authorize(Roles = "Admin,Technician")]
        [HttpPost("create-from-ticket")]
        public async Task<IActionResult> CreateFromTicket([FromBody] CreateJobCardDto request)
        {
            var jobCard =
                await _jobCardService.CreateFromTicketAsync(request.TicketId);

            return CreatedAtAction(
            nameof(GetDetails),
            new { id = jobCard.JobCardId },
            jobCard);
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
            await _jobCardService.AddLabourEntryAsync(id, dto);

            return Ok();
        }

        //---------------------------------------------------------
        // GET LABOUR
        //---------------------------------------------------------

        [Authorize(Roles = "Admin,Technician")]
        [HttpGet("{id}/labour")]
        public async Task<IActionResult> GetLabour(int id)
        {
            var labour = await _jobCardService.GetLabourEntriesAsync(id);

            return Ok(labour);
        }

        //---------------------------------------------------------
        // ADD PART
        //---------------------------------------------------------

        [Authorize(Roles = "Admin,Technician")]
        [HttpPost("{id}/parts")]
        public async Task<IActionResult> AddPart(
            int id,
            [FromBody] AddPartDto dto)
        {
            await _jobCardService.AddPartAsync(id, dto);

            return Ok();
        }


        //---------------------------------------------------------
        // DELETE PART
        //---------------------------------------------------------

        [Authorize(Roles = "Admin,Technician")]
        [HttpDelete("parts/{partId}")]
        public async Task<IActionResult> DeletePart(int partId)
        {
            await _jobCardService.DeletePartAsync(partId);

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
