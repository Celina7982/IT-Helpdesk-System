import { useEffect, useState } from "react";
import dashboardService from "../services/dashboardService";
import ticketService from "../services/ticketService";
import StatusBadge from "./StatusBadge";
import PriorityBadge from "./PriorityBadge";
import TicketDetailsModal from "./TicketDetailsModal";

function RecentTicketsTable() {

    const [tickets, setTickets] = useState([]);
    const [loading, setLoading] = useState(true);
    const [selectedTicketId, setSelectedTicketId] = useState(null);
    const [showDetails, setShowDetails] = useState(false);

    //-------------------------------------------------------
    // Load Tickets
    //-------------------------------------------------------

    const loadTickets = async () => {

        try {

            const data = await dashboardService.getRecentTickets();

            setTickets(data);

        }
        catch (error) {

            console.error(error);

            alert("Unable to load tickets.");

        }
        finally {

            setLoading(false);

        }

    };

    useEffect(() => {

        loadTickets();

    }, []);

    //-------------------------------------------------------
    // Resolve Ticket
    //-------------------------------------------------------

    const resolveTicket = async (ticketId) => {

        if (!window.confirm("Resolve this ticket?"))
            return;

        try {

            await ticketService.resolveTicket(ticketId);

            loadTickets();

        }
        catch (error) {

            console.error(error);

            alert("Unable to resolve ticket.");

        }

    };

    //-------------------------------------------------------

    if (loading)
        return <p>Loading...</p>;

    return (

        <>

            <table className="table table-hover table-striped">

                <thead className="table-dark">

                    <tr>

                        <th>Ticket #</th>

                        <th>Subject</th>

                        <th>Customer</th>

                        <th>Assigned Technician</th>

                        <th>Status</th>

                        <th>Priority</th>

                        <th>Created</th>

                        <th width="250">Actions</th>

                    </tr>

                </thead>

                <tbody>

                    {

                        tickets.length === 0 ?

                            <tr>

                                <td colSpan="8" className="text-center">

                                    No tickets found.

                                </td>

                            </tr>

                            :

                            tickets.map(ticket => (

                                <tr key={ticket.ticketId}>

                                    <td>{ticket.ticketId}</td>

                                    <td>{ticket.subject}</td>

                                    <td>{ticket.customerName}</td>

                                    <td>{ticket.assignedTechnician}</td>

                                    <td>

                                        <StatusBadge status={ticket.status} />

                                    </td>

                                    <td>

                                        <PriorityBadge priority={ticket.priority} />

                                    </td>

                                    <td>

                                        {new Date(ticket.createdDate).toLocaleDateString()}

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
                                            className="btn btn-success btn-sm me-2"
                                            onClick={() => resolveTicket(ticket.ticketId)}
                                        >
                                            Resolve
                                        </button>

                                        <button
                                            className="btn btn-secondary btn-sm"
                                            onClick={() => {

                                                setSelectedTicketId(ticket.ticketId);

                                                setShowJobCard(true);

                                            }}
                                        >
                                            Job Card
                                        </button>

                                    </td>

                                </tr>

                            ))

                    }

                </tbody>

            </table>

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

export default RecentTicketsTable;