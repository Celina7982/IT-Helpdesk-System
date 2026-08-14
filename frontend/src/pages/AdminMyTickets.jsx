import { useEffect, useState } from "react";

import ticketService from "../services/ticketService";

import AdminTicket from "../components/tickets/AdminTicket";

import TicketDetailsModal from "../components/tickets/TicketDetailsModal";


//--------------------------------------------------
// Admin My Tickets
//--------------------------------------------------

function AdminMyTickets() {

    const [tickets, setTickets] = useState([]);

    const [loading, setLoading] = useState(true);

    const [selectedTicketId, setSelectedTicketId] =
        useState(null);

    const [showDetails, setShowDetails] =
        useState(false);


    //--------------------------------------------------
    // Load Tickets
    //--------------------------------------------------

    const loadTickets = async () => {

        try {

            setLoading(true);

            const data =
                await ticketService.getMyTickets();

            setTickets(data || []);

        }
        catch (error) {

            console.error(
                "Failed to load admin tickets:",
                error
            );

            alert(
                "Unable to load your tickets."
            );

        }
        finally {

            setLoading(false);

        }
    };


    //--------------------------------------------------
    // Initial Load
    //--------------------------------------------------

    useEffect(() => {

        loadTickets();

    }, []);


    //--------------------------------------------------
    // View Ticket
    //--------------------------------------------------

    const handleView = (ticketId) => {

        console.log(
            "Opening ticket:",
            ticketId
        );

        setSelectedTicketId(ticketId);

        setShowDetails(true);

    };


    //--------------------------------------------------
    // Close Ticket Details
    //--------------------------------------------------

    const handleCloseDetails = () => {

        setShowDetails(false);

        setSelectedTicketId(null);

    };


    //--------------------------------------------------
    // Resolve Ticket
    //--------------------------------------------------

    const handleResolve = async (ticketId) => {

        const confirmed =
            window.confirm(
                "Are you sure you want to resolve this ticket?"
            );

        if (!confirmed)
            return;


        try {

            await ticketService.resolveTicket(
                ticketId
            );

            alert(
                "Ticket resolved successfully."
            );

            await loadTickets();

        }
        catch (error) {

            console.error(
                "Failed to resolve ticket:",
                error
            );

            alert(
                error.response?.data?.message ||
                error.response?.data ||
                "Unable to resolve ticket."
            );

        }
    };


    //--------------------------------------------------
    // Escalate Ticket
    //--------------------------------------------------

    const handleEscalate = async (ticketId) => {

        const escalationReason =
            window.prompt(
                "Please enter the reason for escalating this ticket:"
            );

        if (escalationReason === null)
            return;


        if (!escalationReason.trim()) {

            alert(
                "Please provide an escalation reason."
            );

            return;
        }


        try {

            await ticketService.escalateTicket(
                ticketId,
                escalationReason.trim()
            );

            alert(
                "Ticket escalated successfully."
            );

            await loadTickets();

        }
        catch (error) {

            console.error(
                "Failed to escalate ticket:",
                error
            );

            alert(
                error.response?.data?.message ||
                error.response?.data ||
                "Unable to escalate ticket."
            );

        }
    };


    //--------------------------------------------------
    // Delete Ticket
    //--------------------------------------------------

    const handleDelete = async (ticketId) => {

        const confirmed =
            window.confirm(
                "Are you sure you want to permanently delete this ticket?"
            );

        if (!confirmed)
            return;


        try {

            await ticketService.deleteTicket(
                ticketId
            );

            alert(
                "Ticket deleted successfully."
            );

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
//--------------------------------------------------
// Archive Ticket
//--------------------------------------------------

const handleArchive = async (ticketId) => {

    const confirmed = window.confirm(
        "Are you sure you want to archive this ticket?"
    );

    if (!confirmed)
        return;

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

    //--------------------------------------------------
    // Return
    //--------------------------------------------------

    return (

        <>
<AdminTicket
    tickets={tickets}
    loading={loading}
    onView={handleView}
    onResolve={handleResolve}
    onEscalate={handleEscalate}
    onTicketDeleted={loadTickets}
    onArchive={handleArchive}
/>
            


            {/* ---------------------------------------- */}
            {/* Ticket Details Modal */}
            {/* ---------------------------------------- */}

            {showDetails && selectedTicketId && (

                <TicketDetailsModal

                    show={showDetails}

                    ticketId={selectedTicketId}

                    onClose={handleCloseDetails}

                />

            )}

        </>

    );

}

export default AdminMyTickets;