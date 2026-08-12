import { useEffect, useState } from "react";
import ticketService from "../services/ticketService";
import AdminTicketsTable from "../components/tickets/AdminTicketsTable";
import CreateTicketModal from "../components/tickets/CreateTicketModal";

function Tickets() {
    const [tickets, setTickets] = useState([]);
    const [loading, setLoading] = useState(true);
    const [showCreateModal, setShowCreateModal] = useState(false);

    const loadTickets = async () => {
        try {
            setLoading(true);

            const data = await ticketService.getAllTickets();

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

    return (
        <div className="container-fluid">

            <div className="d-flex justify-content-between align-items-center mb-4">

                <h2>Ticket Management</h2>

                <button
                    className="btn btn-primary"
                    onClick={() => setShowCreateModal(true)}
                >
                    + New Ticket
                </button>

            </div>

            <AdminTicketsTable
                tickets={tickets}
                loading={loading}
                refreshTickets={loadTickets}
            />

            <CreateTicketModal
                show={showCreateModal}
                onClose={() => setShowCreateModal(false)}
                onTicketCreated={() => {
                    setShowCreateModal(false);
                    loadTickets();
                }}
            />

        </div>
    );
}

export default Tickets;