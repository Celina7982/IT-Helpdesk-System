import { useState } from "react";
import jobCardService from "../../services/jobCardService";
import { useNavigate, useLocation } from "react-router-dom";
import TicketDetailsModal from "./TicketDetailsModal";

function MyTicketsTable({
    tickets,
    loading,
    onResolve,
    onEscalate
}) {
    const navigate = useNavigate();
    const location = useLocation();

    const [selectedTicketId, setSelectedTicketId] = useState(null);
    const [showDetails, setShowDetails] = useState(false);

    // Determine base path dynamically (admin vs technician)
    const basePath = location.pathname.startsWith("/admin")
        ? "/admin"
        : "/technician";

    //--------------------------------------------------
    // View Ticket
    //--------------------------------------------------
    const viewTicket = (ticketId) => {
        setSelectedTicketId(ticketId);
        setShowDetails(true);
    };

    //--------------------------------------------------
    // Close Ticket Details
    //--------------------------------------------------
    const closeTicketDetails = () => {
        setShowDetails(false);
        setSelectedTicketId(null);
    };

    //--------------------------------------------------
    // Create Job Card
    //--------------------------------------------------
    const createJobCard = async (ticketId) => {
        try {
            const jobCard = await jobCardService.createFromTicket(ticketId);

            navigate(`${basePath}/jobcards/${jobCard.jobCardId}`);
        } catch (error) {
            console.error(error);

            alert(
                error.response?.data ||
                "Unable to create Job Card."
            );
        }
    };

    return (
        <>
            <div className="card shadow mt-4">

                <div className="card-header bg-success text-white">
                    My Tickets
                </div>

                <div className="card-body">

                    {loading ? (

                        <div className="text-center">

                            <div className="spinner-border text-success"></div>

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

                                            <td>
                                                {ticket.ticketId}
                                            </td>

                                            <td>
                                                {ticket.subject}
                                            </td>

                                            <td>

                                                <span
                                                    className={
                                                        ticket.status === "Resolved"
                                                            ? "badge bg-success"
                                                            : ticket.status === "In Progress"
                                                                ? "badge bg-info"
                                                                : "badge bg-warning text-dark"
                                                    }
                                                >
                                                    {ticket.status}
                                                </span>

                                            </td>

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

                                            <td>

                                                {/* View Ticket */}
                                                <button
                                                    className="btn btn-outline-primary btn-sm me-2"
                                                    onClick={() =>
                                                        viewTicket(ticket.ticketId)
                                                    }
                                                >
                                                    View
                                                </button>

                                                {/* Open / In Progress */}
                                                {(ticket.status === "Open" ||
                                                    ticket.status === "In Progress") && (

                                                    <>

                                                        <button
                                                            className="btn btn-success btn-sm me-2"
                                                            onClick={() =>
                                                                onResolve(ticket.ticketId)
                                                            }
                                                        >
                                                            Resolve
                                                        </button>

                                                        <button
                                                            className="btn btn-warning btn-sm"
                                                            onClick={() =>
                                                                onEscalate(ticket.ticketId)
                                                            }
                                                        >
                                                            Escalate
                                                        </button>

                                                    </>

                                                )}

                                                {/* Resolved but no Job Card */}
                                                {ticket.status === "Resolved" &&
                                                    !ticket.hasJobCard && (

                                                        <button
                                                            className="btn btn-primary btn-sm me-2"
                                                            onClick={() =>
                                                                createJobCard(ticket.ticketId)
                                                            }
                                                        >
                                                            Create Job Card
                                                        </button>

                                                    )}

                                                {/* Job Card already exists */}
                                                {ticket.hasJobCard && (

                                                    <button
                                                        className="btn btn-outline-secondary btn-sm"
                                                        onClick={() =>
                                                            navigate(
                                                                `${basePath}/jobcards/${ticket.jobCardId}`
                                                            )
                                                        }
                                                    >
                                                        Open Job Card
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

            {/* Ticket Details Popup */}
            <TicketDetailsModal
                show={showDetails}
                ticketId={selectedTicketId}
                onClose={closeTicketDetails}
            />

        </>
    );
}

export default MyTicketsTable;