import jobCardService from "../../services/jobCardService";
import { useNavigate } from "react-router-dom";

// My tickets for admin.
// Admin can view, resolve, escalate, and create/view job cards.

function AdminTicket({
    tickets = [],
    loading,
    onView,
    onResolve,
    onEscalate,
    onAssign
}) {
    const navigate = useNavigate();

    //--------------------------------------------------
    // Create Job Card
    //--------------------------------------------------

    const createJobCard = async (ticketId) => {
        try {
            const jobCard =
                await jobCardService.createFromTicket(ticketId);

            // Open the newly created Job Card
            navigate(`/admin/jobcards/${jobCard.jobCardId}`);
        } catch (error) {
            console.error(error);

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

                            {/* No tickets */}
                            {tickets.length === 0 ? (

                                <tr>
                                    <td
                                        colSpan="5"
                                        className="text-center"
                                    >
                                        You have no assigned tickets.
                                    </td>
                                </tr>

                            ) : (

                                tickets.map(ticket => (

                                    <tr key={ticket.ticketId}>

                                        {/* -------------------------------- */}
                                        {/* ID */}
                                        {/* -------------------------------- */}

                                        <td>
                                            {ticket.ticketId}
                                        </td>


                                        {/* -------------------------------- */}
                                        {/* Subject */}
                                        {/* -------------------------------- */}

                                        <td>
                                            {ticket.subject}
                                        </td>


                                        {/* -------------------------------- */}
                                        {/* Status */}
                                        {/* -------------------------------- */}

                                        <td>

                                            <span
                                                className={
                                                    ticket.status === "Resolved"
                                                        ? "badge bg-success"
                                                        : ticket.status === "In Progress"
                                                            ? "badge bg-info"
                                                            : ticket.status === "Escalated"
                                                                ? "badge bg-danger"
                                                                : "badge bg-warning text-dark"
                                                }
                                            >
                                                {ticket.status}
                                            </span>

                                        </td>


                                        {/* -------------------------------- */}
                                        {/* Priority */}
                                        {/* -------------------------------- */}

                                        <td>

                                            <span
                                                className={
                                                    ticket.priority === "High"
                                                        ? "badge bg-danger"
                                                        : ticket.priority === "Medium"
                                                            ? "badge bg-warning text-dark"
                                                            : "badge bg-success"
                                                }
                                            >
                                                {ticket.priority}
                                            </span>

                                        </td>


                                        {/* -------------------------------- */}
                                        {/* ACTIONS */}
                                        {/* -------------------------------- */}

                                        <td>

                                            {/* ============================== */}
                                            {/* VIEW TICKET */}
                                            {/* ============================== */}

                                            <button
                                                className="btn btn-outline-primary btn-sm me-2"
                                                onClick={() =>
                                                    onView(ticket.ticketId)
                                                }
                                            >
                                                View
                                            </button>


                                            {/* ============================== */}
                                            {/* OPEN / IN PROGRESS */}
                                            {/* ============================== */}

                                            {(ticket.status === "Open" ||
                                                ticket.status === "In Progress") && (

                                                <>

                                                    {/* Resolve */}
                                                    <button
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


                                            {/* ============================== */}
                                            {/* ESCALATED */}
                                            {/* ============================== */}

                                            {ticket.status === "Escalated" && (

                                                <span className="badge bg-danger me-2">
                                                    Escalated
                                                </span>

                                            )}


                                            {/* ============================== */}
                                            {/* RESOLVED - NO JOB CARD */}
                                            {/* ============================== */}

                                            {ticket.status === "Resolved" &&
                                                !ticket.hasJobCard &&
                                                !ticket.jobCardId && (

                                                    <button
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


                                            {/* ============================== */}
                                            {/* RESOLVED - JOB CARD EXISTS */}
                                            {/* ============================== */}

                                            {ticket.status === "Resolved" &&
                                                (ticket.hasJobCard ||
                                                    ticket.jobCardId) && (

                                                    <button
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

                                        </td>

                                    </tr>

                                ))

                            )}

                        </tbody>

                    </table>

                )}

            </div>

        </div>
    );
}

export default AdminTicket;