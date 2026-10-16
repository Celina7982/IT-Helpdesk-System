import { useEffect, useState } from "react";
import ticketService from "../services/ticketService";
import TicketDetailsModal from "../components/tickets/TicketDetailsModal";

//--------------------------------------------------
// Admin Archived Tickets
//--------------------------------------------------

function AdminArchivedTickets() {
    const [tickets, setTickets] = useState([]);
    const [loading, setLoading] = useState(true);

    const [selectedTicketId, setSelectedTicketId] = useState(null);
    const [showDetails, setShowDetails] = useState(false);

    // Pagination states
    const [pageNumber, setPageNumber] = useState(1);
    const [pageSize, setPageSize] = useState(10);

    //--------------------------------------------------
    // Load Archived Tickets
    //--------------------------------------------------

    const loadArchivedTickets = async () => {
        try {
            setLoading(true);
            const data = await ticketService.getArchivedTickets(pageNumber, pageSize);
            setTickets(data || []);
        } catch (error) {
            console.error(
                "Failed to load archived tickets:",
                error
            );
            alert(
                error.response?.data?.message ||
                error.response?.data ||
                "Unable to load archived tickets."
            );
        } finally {
            setLoading(false);
        }
    };

    //--------------------------------------------------
    // Initial Load & Page Change Effect
    //--------------------------------------------------

    useEffect(() => {
        loadArchivedTickets();
    }, [pageNumber, pageSize]);

    //--------------------------------------------------
    // View Ticket
    //--------------------------------------------------

    const handleView = (ticketId) => {
        console.log("Archived ticket selected:", ticketId);
        setSelectedTicketId(ticketId);
        setShowDetails(true);
    };

    //--------------------------------------------------
    // Close Details
    //--------------------------------------------------

    const handleCloseDetails = () => {
        setShowDetails(false);
        setSelectedTicketId(null);
    };

    //--------------------------------------------------
    // Status Badge
    //--------------------------------------------------

    const getStatusClass = (status) => {
        switch (status) {
            case "Resolved":
                return "badge bg-success";
            case "Escalated":
                return "badge bg-danger";
            case "In Progress":
                return "badge bg-info";
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
    // Format Date
    //--------------------------------------------------

    const formatDate = (date) => {
        if (!date)
            return "—";
        return new Date(date).toLocaleString();
    };

    //--------------------------------------------------
    // Return
    //--------------------------------------------------

    return (
        <>
            <div className="card shadow mt-4">
                {/* Header */}
                <div className="card-header bg-secondary text-white">
                    <h5 className="mb-0">
                        Archived Tickets
                    </h5>
                </div>

                <div className="card-body">
                    {/* Loading */}
                    {loading ? (
                        <div className="text-center py-4">
                            <div className="spinner-border text-primary"></div>
                            <p className="mt-2 mb-0">
                                Loading archived tickets...
                            </p>
                        </div>
                    ) : tickets.length === 0 ? (
                        /* No Archived Tickets */
                        <div className="text-center py-4">
                            <p className="mb-0">
                                There are no archived tickets.
                            </p>
                        </div>
                    ) : (
                        /* Archived Tickets Table */
                        <>
                            <div className="table-responsive">
                                <table className="table table-striped table-hover">
                                    <thead>
                                        <tr>
                                            <th>ID</th>
                                            <th>Subject</th>
                                            <th>Customer</th>
                                            <th>Status</th>
                                            <th>Priority</th>
                                            <th>Created</th>
                                            <th>Archived</th>
                                            <th>Actions</th>
                                        </tr>
                                    </thead>

                                    <tbody>
                                        {tickets.map(ticket => (
                                            <tr key={ticket.ticketId}>
                                                <td>{ticket.ticketId}</td>
                                                <td>{ticket.subject}</td>
                                                <td>{ticket.customerName}</td>
                                                <td>
                                                    <span className={getStatusClass(ticket.status)}>
                                                        {ticket.status}
                                                    </span>
                                                </td>
                                                <td>
                                                    <span className={getPriorityClass(ticket.priority)}>
                                                        {ticket.priority}
                                                    </span>
                                                </td>
                                                <td>{formatDate(ticket.createdDate)}</td>
                                                <td>{formatDate(ticket.archivedDate)}</td>
                                                <td>
                                                    <button
                                                        type="button"
                                                        className="btn btn-outline-primary btn-sm"
                                                        onClick={() => handleView(ticket.ticketId)}
                                                    >
                                                        View
                                                    </button>
                                                </td>
                                            </tr>
                                        ))}
                                    </tbody>
                                </table>
                            </div>

                            {/* Pagination Controls */}
                            <div className="d-flex justify-content-between align-items-center mt-3">
                                <button
                                    className="btn btn-outline-secondary btn-sm"
                                    onClick={() => setPageNumber(prev => Math.max(prev - 1, 1))}
                                    disabled={pageNumber === 1}
                                >
                                    Previous
                                </button>
                                <span>Page {pageNumber}</span>
                                <button
                                    className="btn btn-outline-secondary btn-sm"
                                    onClick={() => setPageNumber(prev => prev + 1)}
                                    disabled={tickets.length < pageSize}
                                >
                                    Next
                                </button>
                            </div>
                        </>
                    )}
                </div>
            </div>

            {/* Ticket Details Modal */}
            {showDetails && selectedTicketId && (
                <TicketDetailsModal
                    show={showDetails}
                    ticketId={selectedTicketId}
                    onClose={handleCloseDetails}
                />
            )}
        </>
    );
}

export default AdminArchivedTickets;