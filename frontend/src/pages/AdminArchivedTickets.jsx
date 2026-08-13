import { useEffect, useState } from "react";
import ticketService from "../services/ticketService";
import TicketDetailsModal from "../components/tickets/TicketDetailsModal";

//--------------------------------------------------
// Admin Archived Tickets
//--------------------------------------------------

function AdminArchivedTickets() {

    const [tickets, setTickets] = useState([]);
    const [loading, setLoading] = useState(true);

    const [selectedTicketId, setSelectedTicketId] =
        useState(null);

    const [showDetails, setShowDetails] =
        useState(false);


    //--------------------------------------------------
    // Load Archived Tickets
    //--------------------------------------------------

    const loadArchivedTickets = async () => {

        try {

            setLoading(true);

            const data =
                await ticketService.getArchivedTickets();

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
    // Initial Load
    //--------------------------------------------------

    useEffect(() => {

        loadArchivedTickets();

    }, []);


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


                                            {/* Customer */}

                                            <td>
                                                {ticket.customerName}
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


                                            {/* Created */}

                                            <td>
                                                {formatDate(
                                                    ticket.createdDate
                                                )}
                                            </td>


                                            {/* Archived */}

                                            <td>
                                                {formatDate(
                                                    ticket.archivedDate
                                                )}
                                            </td>


                                            {/* Actions */}

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

                    )}

                </div>

            </div>


            {/* ---------------------------------------- */}
            {/* Ticket Details Modal */}
            {/* ---------------------------------------- */}

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