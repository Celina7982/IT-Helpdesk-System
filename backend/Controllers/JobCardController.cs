using IThelpdesk.DTOs.JobCard;
using IThelpdesk.Enums;
using IThelpdesk.Interfaces.Services;
using IThelpdesk.Models;
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
        // Helper: Get Current User ID
        //---------------------------------------------------------
        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
                throw new UnauthorizedAccessException("Authenticated user ID could not be determined.");
            return userId;
        }

        //---------------------------------------------------------
        // Helper: Get Current User Role
        //---------------------------------------------------------
        private string GetCurrentUserRole()
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            if (string.IsNullOrWhiteSpace(role))
                throw new UnauthorizedAccessException("Authenticated user role could not be determined.");
            return role;
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
            var userId = GetCurrentUserId();
            var role = GetCurrentUserRole();
            var jobCards = await _jobCardService.GetJobCardListAsync(
                userId, role, mine, status, assignedTo, search, sortBy, sortDirection, pageNumber, pageSize);
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
            if (jobCard == null) return NotFound();
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
            if (jobCard == null) return NotFound();
            return Ok(jobCard);
        }

        //---------------------------------------------------------
        // EXPORT JOB CARD PDF
        //---------------------------------------------------------
        [Authorize(Roles = "Admin,Technician")]
        [HttpGet("{id}/pdf")]
        public async Task<IActionResult> ExportPdf(int id)
        {
            var pdf = await _jobCardPdfService.GenerateJobCardPdfAsync(id);
            var jobCard = await _jobCardService.GetByIdAsync(id);

            if (jobCard != null)
            {
                var userId = GetCurrentUserId();
                await _auditService.LogAsync(
                    jobCard.JobCardId,
                    userId,
                    JobCardAuditAction.PdfGenerated,
                    $"PDF generated for Job Card {jobCard.JobNumber}");
            }

            return File(pdf, "application/pdf", $"JobCard-{id}.pdf");
        }

        //---------------------------------------------------------
        // CREATE JOB CARD FROM TICKET
        //---------------------------------------------------------
        [Authorize(Roles = "Admin,Technician")]
        [HttpPost("create-from-ticket")]
        public async Task<IActionResult> CreateFromTicket([FromBody] CreateJobCardDto request)
        {
            if (request == null) return BadRequest(new { message = "Request is required." });

            var userId = GetCurrentUserId();
            var jobCard = await _jobCardService.CreateFromTicketAsync(request.TicketId, userId);

            return Ok(new
            {
                jobCardId = jobCard.JobCardId,
                jobNumber = jobCard.JobNumber,
                ticketId = jobCard.TicketId,
                assignedTechnicianId = jobCard.AssignedTechnicianId,
                status = jobCard.Status
            });
        }

        //=========================================================
        // LABOUR
        //=========================================================
        //---------------------------------------------------------
        // ADD LABOUR
        //---------------------------------------------------------

        [Authorize(Roles = "Admin,Technician")]
        [HttpPost("{id}/labour")]
        public async Task<IActionResult> AddLabourEntry(
            int id,
            [FromBody] AddLabourEntryDto dto)
        {
            if (dto == null)
            {
                return BadRequest(new
                {
                    message = "Request is required."
                });
            }

            try
            {
                var userId = GetCurrentUserId();

                await _jobCardService.AddLabourEntryAsync(
                    id,
                    dto,
                    userId);
                return Ok(new
                {
                    message = "Labour entry added successfully."
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new
                {
                    message = ex.Message
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
        // GET LABOUR
        //---------------------------------------------------------
        [Authorize(Roles = "Admin,Technician")]
        [HttpGet("{id}/labour")]
        public async Task<IActionResult> GetLabourEntries(int id)
        {
            var labour = await _jobCardService.GetLabourEntriesAsync(id);
            return Ok(labour);
        }

        //---------------------------------------------------------
        // UPDATE LABOUR
        //---------------------------------------------------------
        [Authorize(Roles = "Admin,Technician")]
        [HttpPut("{id}/labour/{labourId}")]
        public async Task<IActionResult> UpdateLabourEntry(int id, int labourId, [FromBody] UpdateLabourEntryDto dto)
        {
            if (dto == null) return BadRequest(new { message = "Request is required." });

            var userId = GetCurrentUserId();
            var role = GetCurrentUserRole();

            await _jobCardService.UpdateLabourEntryAsync(id, labourId, dto, userId, role);
            return NoContent();
        }

        //---------------------------------------------------------
        // DELETE LABOUR
        //---------------------------------------------------------
        [Authorize(Roles = "Admin,Technician")]
        [HttpDelete("{id}/labour/{labourId}")]
        public async Task<IActionResult> DeleteLabourEntry(int id, int labourId)
        {
            var userId = GetCurrentUserId();
            var role = GetCurrentUserRole();

            await _jobCardService.DeleteLabourEntryAsync(id, labourId, userId, role);
            return NoContent();
        }

        //=========================================================
        // PARTS
        //=========================================================
        //---------------------------------------------------------
        // ADD PART
        //---------------------------------------------------------
        [Authorize(Roles = "Admin,Technician")]
        [HttpPost("{id}/parts")]
        public async Task<IActionResult> AddPart(int id, [FromBody] AddPartDto dto)
        {
            if (dto == null) return BadRequest(new { message = "Request is required." });

            var userId = GetCurrentUserId();
            var role = GetCurrentUserRole();
            await _jobCardService.AddPartAsync(id, dto, userId, role);
            return Ok(new { message = "Part added successfully." });
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
        // UPDATE PART
        //---------------------------------------------------------
        [Authorize(Roles = "Admin,Technician")]
        [HttpPut("{id}/parts/{partId}")]
        public async Task<IActionResult> UpdatePart(int id, int partId, [FromBody] UpdatePartDto dto)
        {
            if (dto == null) return BadRequest(new { message = "Request is required." });

            var userId = GetCurrentUserId();
            var role = GetCurrentUserRole();

            await _jobCardService.UpdatePartAsync(id, partId, dto, userId, role);
            return NoContent();
        }

        //---------------------------------------------------------
        // DELETE PART
        //---------------------------------------------------------
        [Authorize(Roles = "Admin,Technician")]
        [HttpDelete("parts/{partId}")]
        public async Task<IActionResult> DeletePart(int partId)
        {
            var userId = GetCurrentUserId();
            var role = GetCurrentUserRole();

            await _jobCardService.DeletePartAsync(
                partId,
                userId,
                role);
            return NoContent();
        }

        //=========================================================
        // JOB CARD UPDATE
        //=========================================================
        [Authorize(Roles = "Admin,Technician")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
    int id,
    [FromBody] UpdateJobCardDto dto)
        {
            if (dto == null)
            {
                return BadRequest(new
                {
                    message = "Request is required."
                });
            }

            try
            {
                var userId = GetCurrentUserId();
                var role = GetCurrentUserRole();

                await _jobCardService.UpdateJobCardAsync(
                    id,
                    dto,
                    userId,
                    role);

                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new
                {
                    message = ex.Message
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
        // COMPLETE JOB CARD
        //---------------------------------------------------------

        [Authorize(Roles = "Admin,Technician")]
        [HttpPut("{id}/complete")]
        public async Task<IActionResult> Complete(int id)
        {
            try
            {
                var userId = GetCurrentUserId();

                await _jobCardService.CompleteJobCardAsync(
                    id,
                    userId);

                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new
                {
                    message = ex.Message
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
        // DELETE JOB CARD
        //---------------------------------------------------------

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var userId = GetCurrentUserId();

                await _jobCardService.DeleteAsync(
                    id,
                    userId);

                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new
                {
                    message = ex.Message
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

        //=========================================================
        // AUDIT
        //=========================================================

        //---------------------------------------------------------
        // GET AUDIT HISTORY
        //---------------------------------------------------------

        [Authorize(Roles = "Admin,Technician")]
        [HttpGet("{id}/audit")]
        public async Task<IActionResult> GetAuditHistory(int id)
        {
            var history =
                await _auditService.GetAuditHistoryDtoAsync(id);

            return Ok(history);
        }
    }
}