import { useEffect, useState } from "react";
import ticketService from "../services/ticketService";

function TechnicianDashboard() {

    const [availableTickets, setAvailableTickets] = useState([]);
    const [myTickets, setMyTickets] = useState([]);
    const [loading, setLoading] = useState(true);

    //-------------------------------------------------------
    // Load Dashboard
    //-------------------------------------------------------

    const loadDashboard = async () => {

        try {

            setLoading(true);

            const available = await ticketService.getAvailableTickets();
            const mine = await ticketService.getMyTickets();

            setAvailableTickets(available);
            setMyTickets(mine);

        }
        catch (error) {

            console.error(error);
            alert("Unable to load dashboard.");

        }
        finally {

            setLoading(false);

        }

    };

    //-------------------------------------------------------
    // Claim Ticket
    //-------------------------------------------------------

    const claimTicket = async (ticketId) => {

        try {

            await ticketService.claimTicket(ticketId);

            alert("Ticket claimed successfully.");

            loadDashboard();

        }
        catch (error) {

            console.error(error);
            alert("Unable to claim ticket.");

        }

    };

    //-------------------------------------------------------
    // Resolve Ticket
    //-------------------------------------------------------

    const resolveTicket = async (ticketId) => {

        try {

            await ticketService.resolveTicket(ticketId);

            alert("Ticket resolved.");

            loadDashboard();

        }
        catch (error) {

            console.error(error);
            alert("Unable to resolve ticket.");

        }

    };

    //-------------------------------------------------------
    // Escalate Ticket
    //-------------------------------------------------------

    const escalateTicket = async (ticketId) => {

        const reason = prompt("Reason for escalation:");

        if (!reason)
            return;

        try {

            await ticketService.escalateTicket(ticketId, reason);

            alert("Ticket escalated.");

            loadDashboard();

        }
        catch (error) {

            console.error(error);
            alert("Unable to escalate ticket.");

        }

    };

    //-------------------------------------------------------

    useEffect(() => {

        loadDashboard();

    }, []);

    return (

        <div className="container mt-4">

            <h2 className="mb-4">
                Technician Dashboard
            </h2>

            <div className="row">

                <div className="col-md-4">

                    <div className="card shadow text-center">

                        <div className="card-body">

                            <h5>Available Tickets</h5>

                            <h2>{availableTickets.length}</h2>

                        </div>

                    </div>

                </div>

                <div className="col-md-4">

                    <div className="card shadow text-center">

                        <div className="card-body">

                            <h5>My Tickets</h5>

                            <h2>{myTickets.length}</h2>

                        </div>

                    </div>

                </div>

                <div className="col-md-4">

                    <div className="card shadow text-center">

                        <div className="card-body">

                            <h5>Resolved Today</h5>

                            <h2>0</h2>

                        </div>

                    </div>

                </div>

            </div>

            {/* Available Tickets */}

            <div className="card shadow mt-4">

                <div className="card-header bg-primary text-white">

                    Available Tickets

                </div>

                <div className="card-body">

                    {loading ? (

                        <p>Loading...</p>

                    ) : (

                        <table className="table table-striped table-hover">

                            <thead>

                                <tr>

                                    <th>ID</th>
                                    <th>Subject</th>
                                    <th>Customer</th>
                                    <th>Priority</th>
                                    <th></th>

                                </tr>

                            </thead>

                            <tbody>

                                {availableTickets.map(ticket => (

                                    <tr key={ticket.ticketId}>

                                        <td>{ticket.ticketId}</td>

                                        <td>{ticket.subject}</td>

                                        <td>{ticket.customerName}</td>

                                        <td>{ticket.priority}</td>

                                        <td>

                                            <button
                                                className="btn btn-success btn-sm"
                                                onClick={() => claimTicket(ticket.ticketId)}
                                            >
                                                Claim
                                            </button>

                                        </td>

                                    </tr>

                                ))}

                            </tbody>

                        </table>

                    )}

                </div>

            </div>

            {/* My Tickets */}

            <div className="card shadow mt-4">

                <div className="card-header bg-success text-white">

                    My Tickets

                </div>

                <div className="card-body">

                    {loading ? (

                        <p>Loading...</p>

                    ) : (

                        <table className="table table-striped table-hover">

                            <thead>

                                <tr>

                                    <th>ID</th>
                                    <th>Subject</th>
                                    <th>Status</th>
                                    <th>Priority</th>
                                    <th></th>

                                </tr>

                            </thead>

                            <tbody>

                                {myTickets.map(ticket => (

                                    <tr key={ticket.ticketId}>

                                        <td>{ticket.ticketId}</td>

                                        <td>{ticket.subject}</td>

                                        <td>{ticket.status}</td>

                                        <td>{ticket.priority}</td>

                                        <td>

                                            <button
                                                className="btn btn-primary btn-sm me-2"
                                                onClick={() => resolveTicket(ticket.ticketId)}
                                            >
                                                Resolve
                                            </button>

                                            <button
                                                className="btn btn-warning btn-sm"
                                                onClick={() => escalateTicket(ticket.ticketId)}
                                            >
                                                Escalate
                                            </button>

                                        </td>

                                    </tr>

                                ))}

                            </tbody>

                        </table>

                    )}

                </div>

            </div>

        </div>

    );

}

export default TechnicianDashboard;