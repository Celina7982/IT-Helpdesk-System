import jobCardService from "../../services/jobCardService";
import ticketService from "../../services/ticketService";
import { useNavigate } from "react-router-dom";

//--------------------------------------------------
// Admin My Tickets
//--------------------------------------------------

function AdminTicket({
    tickets = [],
    loading,
    onView,
    onResolve,
    onEscalate,
    onAssign,
    onTicketDeleted
    
}) {
    const navigate = useNavigate();

    //--------------------------------------------------
    // Create Job Card
    //--------------------------------------------------

    const createJobCard = async (ticketId) => {
        try {
            const jobCard =
                await jobCardService.createFromTicket(ticketId);

            if (!jobCard || !jobCard.jobCardId) {
                alert("Job Card was created but its ID was not returned.");
                return;
            }

            navigate(`/admin/jobcards/${jobCard.jobCardId}`);

        } catch (error) {

            console.error(
                "Failed to create Job Card:",
                error
            );

            alert(
                error.response?.data?.message ||
                error.response?.data ||
                "Unable to create Job Card."
            );
        }
    };

    //--------------------------------------------------
    // View Job Card
    //--------------------------------------------------

    const viewJobCard = (jobCardId) => {

        if (!jobCardId) {
            alert("Job Card ID was not found.");
            return;
        }

        navigate(`/admin/jobcards/${jobCardId}`);
    };

    //--------------------------------------------------
    // Delete Ticket
    //--------------------------------------------------

    const deleteTicket = async (ticketId) => {

        const confirmed = window.confirm(
            "Are you sure you want to delete this ticket?\n\n" +
            "This action cannot be undone."
        );

        if (!confirmed)
            return;

        try {

            await ticketService.deleteTicket(ticketId);

            alert("Ticket deleted successfully.");

            if (onTicketDeleted) {
                onTicketDeleted();
            }

        } catch (error) {

            console.error(
                "Failed to delete ticket:",
                error
            );

            alert(
                error.response?.data?.message ||
                error.response?.data ||
                "Unable to delete ticket."
            );
        }
    };

    //--------------------------------------------------
// Archive Ticket
//--------------------------------------------------

const archiveTicket = async (ticketId) => {

    const confirmed = window.confirm(
        "Are you sure you want to archive this ticket?"
    );

    if (!confirmed)
        return;

    try {

        await ticketService.archiveTicket(ticketId);

        alert("Ticket archived successfully.");

        // Refresh the ticket list
        if (onTicketDeleted) {
            await onTicketDeleted();
        }

    } catch (error) {

        console.error(
            "Failed to archive ticket:",
            error
        );

        alert(
            error.response?.data?.message ||
            error.response?.data ||
            "Unable to archive ticket."
        );
    }
};

    //--------------------------------------------------
    // Status Badge
    //--------------------------------------------------

    const getStatusClass = (status) => {

        switch (status) {

            case "Resolved":
                return "badge bg-success";

            case "In Progress":
                return "badge bg-info";

            case "Escalated":
                return "badge bg-danger";

            case "Open":
                return "badge bg-warning text-dark";

            default:
                return "badge bg-secondary";
        }
    };

    //--------------------------------------------------
    // Priority Badge
    //--------------------------------------------------

    const getPriorityClass = (priority) => {

        switch (priority) {

            case "High":
                return "badge bg-danger";

            case "Medium":
                return "badge bg-warning text-dark";

            case "Low":
                return "badge bg-success";

            default:
                return "badge bg-secondary";
        }
    };

    //--------------------------------------------------
    // Return
    //--------------------------------------------------

    return (

        <div className="card shadow mt-4">

            {/* Header */}

            <div className="card-header bg-primary text-white">
                My Tickets
            </div>


            <div className="card-body">

                {/* Loading */}

                {loading ? (

                    <div className="text-center">

                        <div className="spinner-border text-primary"></div>

                        <p className="mt-2">
                            Loading your tickets...
                        </p>

                    </div>

                ) : (

                    <>

                        {/* No tickets */}

                        {tickets.length === 0 ? (

                            <div className="text-center py-4">

                                <p className="mb-0">
                                    You have no assigned tickets.
                                </p>

                            </div>

                        ) : (

                            <div className="table-responsive">

                                <table className="table table-striped table-hover">

                                    <thead>

                                        <tr>

                                            <th>ID</th>

                                            <th>Subject</th>

                                            <th>Status</th>

                                            <th>Priority</th>

                                            <th>Actions</th>

                                        </tr>

                                    </thead>


                                    <tbody>

                                        {tickets.map(ticket => (

                                            <tr
                                                key={ticket.ticketId}
                                            >

                                                {/* ID */}

                                                <td>
                                                    {ticket.ticketId}
                                                </td>


                                                {/* Subject */}

                                                <td>
                                                    {ticket.subject}
                                                </td>


                                                {/* Status */}

                                                <td>

                                                    <span
                                                        className={getStatusClass(
                                                            ticket.status
                                                        )}
                                                    >
                                                        {ticket.status}
                                                    </span>

                                                </td>


                                                {/* Priority */}

                                                <td>

                                                    <span
                                                        className={getPriorityClass(
                                                            ticket.priority
                                                        )}
                                                    >
                                                        {ticket.priority}
                                                    </span>

                                                </td>


                                                {/* Actions */}

                                                <td>

                                                    {/* -------------------------------- */}
                                                    {/* VIEW TICKET */}
                                                    {/* -------------------------------- */}

                                                    <button
                                                        type="button"
                                                        className="btn btn-outline-primary btn-sm me-2"
                                                        onClick={() =>
                                                            onView(
                                                                ticket.ticketId
                                                            )
                                                        }
                                                    >
                                                        View
                                                    </button>


                                                    {/* -------------------------------- */}
                                                    {/* OPEN / IN PROGRESS */}
                                                    {/* -------------------------------- */}

                                                    {(ticket.status === "Open" ||
                                                        ticket.status === "In Progress") && (

                                                        <>

                                                            {/* Resolve */}

                                                            <button
                                                                type="button"
                                                                className="btn btn-success btn-sm me-2"
                                                                onClick={() =>
                                                                    onResolve(
                                                                        ticket.ticketId
                                                                    )
                                                                }
                                                            >
                                                                Resolve
                                                            </button>


                                                            {/* Assign */}

                                                            {onAssign && (

                                                                <button
                                                                    type="button"
                                                                    className="btn btn-primary btn-sm me-2"
                                                                    onClick={() =>
                                                                        onAssign(
                                                                            ticket.ticketId
                                                                        )
                                                                    }
                                                                >
                                                                    Assign
                                                                </button>

                                                            )}


                                                            {/* Escalate */}

                                                            <button
                                                                type="button"
                                                                className="btn btn-warning btn-sm me-2"
                                                                onClick={() =>
                                                                    onEscalate(
                                                                        ticket.ticketId
                                                                    )
                                                                }
                                                            >
                                                                Escalate
                                                            </button>

                                                        </>

                                                    )}


                                                    {/* -------------------------------- */}
                                                    {/* ESCALATED */}
                                                    {/* -------------------------------- */}

                                                    {ticket.status === "Escalated" && (

                                                        <span className="badge bg-danger me-2">
                                                            Escalated
                                                        </span>

                                                    )}


                                                    {/* -------------------------------- */}
                                                    {/* RESOLVED + NO JOB CARD */}
                                                    {/* -------------------------------- */}

                                                    {ticket.status === "Resolved" &&
                                                        !ticket.hasJobCard &&
                                                        !ticket.jobCardId && (

                                                            <button
                                                                type="button"
                                                                className="btn btn-primary btn-sm me-2"
                                                                onClick={() =>
                                                                    createJobCard(
                                                                        ticket.ticketId
                                                                    )
                                                                }
                                                            >
                                                                Create Job Card
                                                            </button>

                                                        )}


                                                    {/* -------------------------------- */}
                                                    {/* RESOLVED + JOB CARD EXISTS */}
                                                    {/* -------------------------------- */}

                                                    {ticket.status === "Resolved" &&
                                                        (ticket.hasJobCard ||
                                                            ticket.jobCardId) && (

                                                            <button
                                                                type="button"
                                                                className="btn btn-outline-secondary btn-sm me-2"
                                                                onClick={() =>
                                                                    viewJobCard(
                                                                        ticket.jobCardId
                                                                    )
                                                                }
                                                            >
                                                                View Job Card
                                                            </button>

                                                        )}

 {/* -------------------------------- */}
{/* DELETE TICKET */}
{/* -------------------------------- */}

<button
    type="button"
    className="btn btn-outline-danger btn-sm me-2"
    onClick={() =>
        deleteTicket(ticket.ticketId)
    }
>
    Delete
</button>




                                                    {/* -------------------------------- */}
                                                    {/* ARCHIVE */}
                                                    {/* -------------------------------- */}
<button
    type="button"
    className="btn btn-outline-secondary btn-sm"
    onClick={() =>
        archiveTicket(ticket.ticketId)
    }
>
    Archive
</button>
                                                    
                                                </td>

                                            </tr>

                                        ))}

                                    </tbody>

                                </table>

                            </div>

                        )}

                    </>

                )}

            </div>

        </div>

    );
}

export default AdminTicket;