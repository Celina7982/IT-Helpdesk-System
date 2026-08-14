import { useState } from "react";
import ticketService from "../../services/ticketService";
import TicketDetailsModal from "./TicketDetailsModal";
import StatusBadge from "../StatusBadge";
import PriorityBadge from "../PriorityBadge";

/// ALL TICKETS TABLE FOR ADMIN

function AdminTicketsTable({
    tickets,
    loading,
    refreshTickets,
    onArchive,
      onDelete
}) {

    const [selectedTicketId, setSelectedTicketId] = useState(null);
    const [showDetails, setShowDetails] = useState(false);

    //-------------------------------------------------------
    // Delete Ticket
    //-------------------------------------------------------

   

    //-------------------------------------------------------
    // Loading
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

    //-------------------------------------------------------
    // Table
    //-------------------------------------------------------

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

                                <th style={{ width: "220px" }}>
                                    Actions
                                </th>

                            </tr>

                        </thead>

                        <tbody>

                            {tickets.length === 0 ? (

                                <tr>

                                    <td
                                        colSpan="8"
                                        className="text-center"
                                    >

                                        No tickets found.

                                    </td>

                                </tr>

                            ) : (

                                tickets.map(ticket => (

                                    <tr key={ticket.ticketId}>

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

                                            <StatusBadge
                                                status={ticket.status}
                                            />

                                        </td>

                                        {/* Priority */}

                                        <td>

                                            <PriorityBadge
                                                priority={ticket.priority}
                                            />

                                        </td>

                                        {/* Assigned Technician */}

                                        <td>

                                            {ticket.assignedTechnician ||
                                                "Unassigned"}

                                        </td>

                                        {/* Created */}

                                        <td>

                                            {new Date(
                                                ticket.createdDate
                                            ).toLocaleDateString()}

                                        </td>

                                        {/* Actions */}

                                        <td>

                                            {/* View */}

                                            <button
                                                type="button"
                                                className="btn btn-primary btn-sm me-2"
                                                onClick={() => {

                                                    setSelectedTicketId(
                                                        ticket.ticketId
                                                    );

                                                    setShowDetails(true);

                                                }}
                                            >
                                                View
                                            </button>

                                            {/* Archive */}

                                            <button
    type="button"
    className="btn btn-outline-secondary btn-sm me-2"
    onClick={() => {
        onArchive(ticket.ticketId);
    }}
>
    Archive
</button>
                                            {/* Delete */}

                                            <button
    type="button"
    className="btn btn-danger btn-sm"
    onClick={() => {
        onDelete(ticket.ticketId);
    }}
>
    Delete
</button>

                                        </td>

                                    </tr>

                                ))

                            )}

                        </tbody>

                    </table>

                </div>

            </div>

            {/* Ticket Details Modal */}

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