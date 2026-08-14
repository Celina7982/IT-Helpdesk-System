import { useEffect, useState } from "react";
import ticketService from "../services/ticketService";
import AdminTicketsTable from "../components/tickets/AdminTicketsTable";
import CreateTicketModal from "../components/tickets/CreateTicketModal";

function Tickets() {

    const [tickets, setTickets] = useState([]);
    const [loading, setLoading] = useState(true);
    const [showCreateModal, setShowCreateModal] = useState(false);

    //-------------------------------------------------------
    // Load Tickets
    //-------------------------------------------------------

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

    //-------------------------------------------------------
    // Initial Load
    //-------------------------------------------------------

    useEffect(() => {

        loadTickets();

    }, []);

    //-------------------------------------------------------
    // Archive Ticket
    //-------------------------------------------------------

    const handleArchive = async (ticketId) => {

        const confirmed = window.confirm(
            "Are you sure you want to archive this ticket?"
        );

        if (!confirmed) {
            return;
        }

        try {

            await ticketService.archiveTicket(ticketId);

            alert("Ticket archived successfully.");

            await loadTickets();

        }
        catch (error) {

            console.error(
                "Failed to archive ticket:",
                error
            );

            alert(
                error.response?.data?.message ||
                error.response?.data ||
                "Unable to archive ticket."
            );

        }

    };

    //-------------------------------------------------------
    // Delete Ticket
    //-------------------------------------------------------

    const handleDelete = async (ticketId) => {

        const confirmed = window.confirm(
            "Are you sure you want to delete this ticket?"
        );

        if (!confirmed) {
            return;
        }

        try {

            await ticketService.deleteTicket(ticketId);

            alert("Ticket deleted successfully.");

            await loadTickets();

        }
        catch (error) {

            console.error(
                "Failed to delete ticket:",
                error
            );

            alert(
                error.response?.data?.message ||
                error.response?.data ||
                "Unable to delete ticket."
            );

        }

    };

    //-------------------------------------------------------
    // Return
    //-------------------------------------------------------

    return (

        <div className="container-fluid">

            <div className="d-flex justify-content-between align-items-center mb-4">

                <h2>Ticket Management</h2>

                <button
                    type="button"
                    className="btn btn-primary"
                    onClick={() => setShowCreateModal(true)}
                >
                    + New Ticket
                </button>

            </div>

            {/* All Tickets Table */}

            <AdminTicketsTable

                tickets={tickets}

                loading={loading}

                refreshTickets={loadTickets}

                onArchive={handleArchive}

                onDelete={handleDelete}

            />

            {/* Create Ticket Modal */}

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