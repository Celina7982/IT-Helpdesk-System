import { useState } from "react";
import ticketService from "../../services/ticketService";
import TicketDetailsModal from "./TicketDetailsModal";
import StatusBadge from "../StatusBadge";
import PriorityBadge from "../PriorityBadge";

function AdminTicketsTable({
    tickets,
    loading,
    refreshTickets
}) {

    const [selectedTicketId, setSelectedTicketId] = useState(null);
    const [showDetails, setShowDetails] = useState(false);

    //-------------------------------------------------------
    // Delete Ticket
    //-------------------------------------------------------

    const deleteTicket = async (ticketId) => {

        if (!window.confirm("Delete this ticket?"))
            return;

        try {

            await ticketService.deleteTicket(ticketId);

            await refreshTickets();

        }
        catch (error) {

            console.error(error);

            alert("Unable to delete ticket.");

        }

    };

    //-------------------------------------------------------

    if (loading) {

        return (

            <div className="text-center py-5">

                <div className="spinner-border text-primary"></div>

                <p className="mt-3">
                    Loading tickets...
                </p>

            </div>

        );

    }

    return (

        <>

            <div className="card shadow">

                <div className="card-header bg-primary text-white">

                    All Tickets

                </div>

                <div className="card-body">

                    <table className="table table-hover table-striped">

                        <thead className="table-dark">

                            <tr>

                                <th>ID</th>
                                <th>Subject</th>
                                <th>Customer</th>
                                <th>Status</th>
                                <th>Priority</th>
                                <th>Assigned Technician</th>
                                <th>Created</th>
                                <th style={{ width: "180px" }}>
                                    Actions
                                </th>

                            </tr>

                        </thead>

                        <tbody>

                            {tickets.length === 0 ?

                                <tr>

                                    <td
                                        colSpan="8"
                                        className="text-center"
                                    >

                                        No tickets found.

                                    </td>

                                </tr>

                                :

                                tickets.map(ticket => (

                                    <tr key={ticket.ticketId}>

                                        <td>{ticket.ticketId}</td>

                                        <td>{ticket.subject}</td>

                                        <td>{ticket.customerName}</td>

                                        <td>

                                            <StatusBadge
                                                status={ticket.status}
                                            />

                                        </td>

                                        <td>

                                            <PriorityBadge
                                                priority={ticket.priority}
                                            />

                                        </td>

                                        <td>

                                            {ticket.assignedTechnicianName ||
                                                "Unassigned"}

                                        </td>

                                        <td>

                                            {new Date(
                                                ticket.createdDate
                                            ).toLocaleDateString()}

                                        </td>

                                        <td>

                                            <button
                                                className="btn btn-primary btn-sm me-2"
                                                onClick={() => {

                                                    setSelectedTicketId(ticket.ticketId);

                                                    setShowDetails(true);

                                                }}
                                            >
                                                View
                                            </button>

                                            <button
                                                className="btn btn-danger btn-sm"
                                                onClick={() =>
                                                    deleteTicket(ticket.ticketId)
                                                }
                                            >
                                                Delete
                                            </button>

                                        </td>

                                    </tr>

                                ))

                            }

                        </tbody>

                    </table>

                </div>

            </div>

            <TicketDetailsModal

                show={showDetails}

                ticketId={selectedTicketId}

                onClose={() => {

                    setShowDetails(false);

                    setSelectedTicketId(null);

                }}

            />

        </>

    );

}

export default AdminTicketsTable;