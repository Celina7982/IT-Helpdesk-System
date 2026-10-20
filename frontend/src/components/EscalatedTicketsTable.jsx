import { useEffect, useState } from "react";
import ticketService from "../services/ticketService";
import userService from "../services/userService";

function EscalatedTicketsTable() {
    const [tickets, setTickets] = useState([]);
   const [assignableUsers, setAssignableUsers] = useState([]);
const [selectedUsers, setSelectedUsers] = useState({});
    
    // Pagination states
    const [pageNumber, setPageNumber] = useState(1);
    const [pageSize, setPageSize] = useState(10);

  const loadData = async () => {
    try {

        const escalated =
            await ticketService.getEscalatedTickets(
                pageNumber,
                pageSize
            );

        const users =
            await userService.getAssignableUsers();

        setTickets(escalated);
        setAssignableUsers(users);

    } catch (error) {

        console.error(
            "Failed to load escalated tickets:",
            error
        );

    }
};

    useEffect(() => {
        loadData();
    }, [pageNumber, pageSize]);

  const handleSelection = (ticketId, userId) => {
    setSelectedUsers(prev => ({
        ...prev,
        [ticketId]: userId
    }));
};;

    const assignTicket = async (ticketId) => {
       const userId = selectedUsers[ticketId];

if (!userId) {
    alert("Please select an Admin or Technician.");
    return;
}

        try {
            await ticketService.assignTicket(ticketId, userId);
            alert("Ticket assigned successfully.");
            await loadData();
        } catch (error) {
            console.error(error);
            alert("Unable to assign ticket.");
        }
    };

    const resolveTicket = async (ticketId) => {
        const confirmed = window.confirm(
            "Are you sure you want to resolve this ticket?"
        );

        if (!confirmed) return;

        try {
            await ticketService.resolveTicket(ticketId);
            alert("Ticket resolved successfully.");
            setTickets(prev =>
                prev.filter(ticket => ticket.ticketId !== ticketId)
            );
        } catch (error) {
            console.error(error);
            alert("Unable to resolve ticket.");
        }
    };

    return (
        <div className="card shadow mt-4">
            <div className="card-header bg-danger text-white">
                Escalated Tickets
            </div>

            <div className="card-body">
                {tickets.length === 0 ? (
                    <p className="text-muted">No escalated tickets.</p>
                ) : (
                    <>
                        <table className="table table-striped table-hover">
                            <thead>
                                <tr>
                                    <th>ID</th>
                                    <th>Subject</th>
                                    <th>Priority</th>
                                    <th>Reason</th>
                                    <th>Assign To</th>
                                    <th>Assign</th>
                                    <th>Resolve</th>
                                </tr>
                            </thead>

                            <tbody>
                                {tickets.map(ticket => (
                                    <tr key={ticket.ticketId}>
                                        <td>{ticket.ticketId}</td>
                                        <td>{ticket.subject}</td>
                                        <td>
                                            <span className="badge bg-warning text-dark">
                                                {ticket.priority}
                                            </span>
                                        </td>
                                        <td>{ticket.escalationReason}</td>
                                        <td>
                                           <select
    className="form-select"
    value={selectedUsers[ticket.ticketId] || ""}
    onChange={(e) =>
        handleSelection(
            ticket.ticketId,
            Number(e.target.value)
        )
    }
>
    <option value="">
        Select Admin or Technician
    </option>

   {assignableUsers.map(user => (
    <option
        key={user.userId}
        value={user.userId}
    >
        {user.fullName} ({user.role})
    </option>
))} 
</select>


                                        </td>
                                        <td>
                                            <button
                                                className="btn btn-success"
                                                onClick={() =>
                                                    assignTicket(ticket.ticketId)
                                                }
                                            >
                                                Assign
                                            </button>
                                        </td>
                                        <td>
                                            <button
                                                className="btn btn-danger"
                                                onClick={() =>
                                                    resolveTicket(ticket.ticketId)
                                                }
                                            >
                                                Resolve
                                            </button>
                                        </td>
                                    </tr>
                                ))}
                            </tbody>
                        </table>

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
    );
}

export default EscalatedTicketsTable;