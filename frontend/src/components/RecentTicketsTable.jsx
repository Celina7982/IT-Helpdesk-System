import { useEffect, useState } from "react";
import dashboardService from "../services/dashboardService";
import ticketService from "../services/ticketService";
import StatusBadge from "./StatusBadge";
import PriorityBadge from "./PriorityBadge";
import TicketDetailsModal from "./tickets/TicketDetailsModal";

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

            await loadTickets();

        }
        catch (error) {

            console.error(error);

            alert("Unable to resolve ticket.");

        }

    };


    //-------------------------------------------------------
// Claim Ticket
//-------------------------------------------------------

const claimTicket = async (ticketId) => {

    if (!window.confirm("Claim this ticket?"))
        return;

    try {

        await ticketService.claimTicket(ticketId);

        await loadTickets();

    }
    catch (error) {

        console.error(error);

        alert("Unable to claim ticket.");

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

                        <th style={{ width: "220px" }}>
                            Actions
                        </th>

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

                                    <td>{ticket.assignedTechnicianName}</td>

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

                                        {!ticket.isClaimed && ticket.status !== "Resolved" && (

                                        <button
                                            className="btn btn-warning btn-sm me-2"
                                            onClick={() => claimTicket(ticket.ticketId)}
                                        >
                                            Claim
                                        </button>

                                    )}

                                        {ticket.isClaimed && ticket.status !== "Resolved" && (
                                            <button
                                                className="btn btn-success btn-sm me-2"
                                                onClick={() => resolveTicket(ticket.ticketId)}
                                            >
                                                Resolve
                                            </button>
                                        )}

                                    

                                        

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