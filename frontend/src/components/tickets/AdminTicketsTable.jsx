
import { useState } from "react";
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
    const [customerSearch, setCustomerSearch] = useState("");
    const [technicianSearch, setTechnicianSearch] = useState("");

    const filteredTickets = tickets.filter((ticket) => {
        const customerName = (ticket.customerName || "").toLowerCase();
        const assignedTechnician = (ticket.assignedTechnician || "Unassigned").toLowerCase();

        const matchesCustomer = customerName.includes(customerSearch.toLowerCase());
        const matchesTechnician = assignedTechnician.includes(technicianSearch.toLowerCase());

        return matchesCustomer && matchesTechnician;
    });

    if (loading) {
        return (
            <div className="text-center py-5">
                <div className="spinner-border text-primary"></div>
                <p className="mt-3">Loading tickets...</p>
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
                    <div className="row g-3 mb-3">
                        <div className="col-md-6">
                            <label className="form-label fw-semibold">Search by Customer</label>
                            <input
                                type="text"
                                className="form-control"
                                placeholder="Customer name..."
                                value={customerSearch}
                                onChange={(e) => setCustomerSearch(e.target.value)}
                            />
                        </div>

                        <div className="col-md-6">
                            <label className="form-label fw-semibold">Search by Assigned Technician</label>
                            <input
                                type="text"
                                className="form-control"
                                placeholder="Technician name..."
                                value={technicianSearch}
                                onChange={(e) => setTechnicianSearch(e.target.value)}
                            />
                        </div>
                    </div>

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
                                <th style={{ width: "220px" }}>Actions</th>
                            </tr>
                        </thead>

                        <tbody>
                            {filteredTickets.length === 0 ? (
                                <tr>
                                    <td colSpan="8" className="text-center">
                                        No tickets found.
                                    </td>
                                </tr>
                            ) : (
                                filteredTickets.map((ticket) => (
                                    <tr key={ticket.ticketId}>
                                        <td>{ticket.ticketId}</td>
                                        <td>{ticket.subject}</td>
                                        <td>{ticket.customerName}</td>
                                        <td>
                                            <StatusBadge status={ticket.status} />
                                        </td>
                                        <td>
                                            <PriorityBadge priority={ticket.priority} />
                                        </td>
                                        <td>{ticket.assignedTechnician || "Unassigned"}</td>
                                        <td>
                                            {new Date(ticket.createdDate).toLocaleDateString()}
                                        </td>
                                        <td>
                                            <button
                                                type="button"
                                                className="btn btn-primary btn-sm me-2"
                                                onClick={() => {
                                                    setSelectedTicketId(ticket.ticketId);
                                                    setShowDetails(true);
                                                }}
                                            >
                                                View
                                            </button>

                                            <button
                                                type="button"
                                                className="btn btn-outline-secondary btn-sm me-2"
                                                onClick={() => {
                                                    onArchive(ticket.ticketId);
                                                }}
                                            >
                                                Archive
                                            </button>

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