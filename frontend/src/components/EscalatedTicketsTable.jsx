import { useEffect, useState } from "react";
import ticketService from "../services/ticketService";
import userService from "../services/userService";

function EscalatedTicketsTable() {
    const [tickets, setTickets] = useState([]);
    const [technicians, setTechnicians] = useState([]);
    const [selectedTechnicians, setSelectedTechnicians] = useState({});

    const loadData = async () => {
        try {
            const escalated = await ticketService.getEscalatedTickets();
            const techs = await userService.getTechnicians();

            setTickets(escalated);
            setTechnicians(techs);
        } catch (error) {
            console.error("Failed to load escalated tickets:", error);
        }
    };

    useEffect(() => {
        const fetchData = async () => {
            await loadData();
        };

        fetchData();
    }, []);

    const handleSelection = (ticketId, technicianId) => {
        setSelectedTechnicians(prev => ({
            ...prev,
            [ticketId]: technicianId
        }));
    };

    const assignTicket = async (ticketId) => {
        const technicianId = selectedTechnicians[ticketId];

        if (!technicianId) {
            alert("Please select a technician.");
            return;
        }

        try {
            await ticketService.assignTicket(ticketId, technicianId);

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
                                            value={selectedTechnicians[ticket.ticketId] || ""}
                                            onChange={(e) =>
                                                handleSelection(
                                                    ticket.ticketId,
                                                    Number(e.target.value)
                                                )
                                            }
                                        >
                                            <option value="">
                                                Select Technician
                                            </option>

                                            {technicians.map(tech => (
                                                <option
                                                    key={tech.userId}
                                                    value={tech.userId}
                                                >
                                                    {tech.firstName} {tech.lastName}
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
                )}
            </div>
        </div>
    );
}

export default EscalatedTicketsTable;