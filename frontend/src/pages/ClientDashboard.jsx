import { useEffect, useState } from "react";
import StatisticCard from "../components/StatisticCard";
import clientService from "../services/clientService";
import CreateTicketModal from "../components/CreateTicketModal";
import TicketDetailsModal from "../components/tickets/TicketDetailsModal";
function ClientDashboard() {
  

    const [tickets, setTickets] = useState([]);
    const [loading, setLoading] = useState(true);
    const [showModal, setShowModal] = useState(false);
    const [selectedTicketId, setSelectedTicketId] = useState(null);
    const [showDetailsModal, setShowDetailsModal] = useState(false);
   
    const loadTickets = async () => {

        setLoading(true);

        try {

            const data = await clientService.getMyTickets();

            setTickets(data);

        }
        catch (error) {

            console.error(error);

            alert("Unable to load your tickets.");

        }
        finally {

            setLoading(false);

        }

    };

    useEffect(() => {

        loadTickets();

    }, []);

    const openTickets =
        tickets.filter(ticket => ticket.status === "Open").length;

    const inProgressTickets =
        tickets.filter(ticket => ticket.status === "In Progress").length;

    const resolvedTickets =
        tickets.filter(ticket => ticket.status === "Resolved").length;

    if (loading) {

        return (

            <div className="text-center mt-5">

                <div className="spinner-border text-primary"></div>

                <p className="mt-3">
                    Loading dashboard...
                </p>

            </div>

        );

    }

    return (

        <div className="container mt-4">

            <h2 className="mb-4">
                Client Dashboard
            </h2>

            {/* Statistics */}

            <div className="row">

                <StatisticCard
                    title="Open Tickets"
                    value={openTickets}
                    color="warning"
                />

                <StatisticCard
                    title="In Progress"
                    value={inProgressTickets}
                    color="info"
                />

                <StatisticCard
                    title="Resolved"
                    value={resolvedTickets}
                    color="success"
                />

            </div>

            {/* Create Ticket Button */}

            <div className="d-flex justify-content-end mt-4 mb-3">

                <button
                    className="btn btn-primary"
                    onClick={() => setShowModal(true)}
                >
                    + Create Ticket
                </button>

            </div>

            {/* Tickets Table */}

            <div className="card shadow">

                <div className="card-header bg-primary text-white">

                    My Tickets

                </div>

                <div className="card-body">

                    <table className="table table-striped table-hover">

                        <thead>

                            <tr>

                                <th>Ticket #</th>
                                <th>Subject</th>
                                <th>Status</th>
                                <th>Priority</th>
                                <th>Created</th>
                                <th>Actions</th>

                            </tr>

                        </thead>

                        <tbody>

                            {
                                tickets.length === 0 ?

                                    <tr>

                                        <td
                                            colSpan="6"
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

                                            <td>

                                                <span
                                                    className={
                                                        ticket.status === "Resolved"
                                                            ? "badge bg-success"
                                                            : ticket.status === "In Progress"
                                                                ? "badge bg-info"
                                                                : "badge bg-warning text-dark"
                                                    }
                                                >

                                                    {ticket.status}

                                                </span>

                                            </td>

                                            <td>{ticket.priority}</td>

                                            <td>

                                                {
                                                    new Date(ticket.createdDate)
                                                        .toLocaleDateString()
                                                }

                                            </td>
                                                <td>
                                            {/* <button
                                                className="btn btn-outline-primary btn-sm"
                                                onClick={() => {
                                                    setSelectedTicketId(ticket.ticketId);
                                                    setShowDetailsModal(true);
                                                }}
                                            >
                                                View
                                            </button> */}

                                            <button
                                                className="btn btn-outline-primary btn-sm"
                                                onClick={() => {

                                                    console.log("View clicked", ticket.ticketId);

                                                    setSelectedTicketId(ticket.ticketId);

                                                    setShowDetailsModal(true);

                                                }}
                                            >
                                                View
                                            </button>



                                        </td>
                                        </tr>

                                    ))

                            }

                        </tbody>

                    </table>

                </div>

            </div>

            {/* Create Ticket Modal */}

            <CreateTicketModal
                show={showModal}
                onClose={() => setShowModal(false)}
                onTicketCreated={loadTickets}
            />

            <TicketDetailsModal
                        show={showDetailsModal}
                        ticketId={selectedTicketId}
                        onClose={() => {
                            setShowDetailsModal(false);
                            setSelectedTicketId(null);
                        }}
                    />
        </div>

    );
           
}

export default ClientDashboard;