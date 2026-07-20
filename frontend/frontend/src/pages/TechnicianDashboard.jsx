import { useEffect, useState } from "react";

import ticketService from "../services/ticketService";

import TechnicianStats from "../components/TechnicianStats";
import AvailableTicketsTable from "../components/AvailableTicketsTable";
import MyTicketsTable from "../components/MyTicketsTable";
import TicketDetailsModal from "../components/TicketDetailsModal";

function TechnicianDashboard() {

    const [availableTickets, setAvailableTickets] = useState([]);
    const [myTickets, setMyTickets] = useState([]);

    const [loading, setLoading] = useState(true);

    const [selectedTicketId, setSelectedTicketId] = useState(null);
    const [showDetailsModal, setShowDetailsModal] = useState(false);

    //--------------------------------------------------------
    // Load Dashboard
    //--------------------------------------------------------

    const loadDashboard = async () => {

        try {

            setLoading(true);

            const available =
                await ticketService.getAvailableTickets();

            const mine =
                await ticketService.getMyTickets();

            setAvailableTickets(available);
            setMyTickets(mine);

        }
        catch (error) {

            console.error(error);

            alert("Unable to load technician dashboard.");

        }
        finally {

            setLoading(false);

        }

    };

    //--------------------------------------------------------
    // Claim Ticket
    //--------------------------------------------------------

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

    //--------------------------------------------------------
    // Resolve Ticket
    //--------------------------------------------------------

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

    //--------------------------------------------------------
    // Escalate Ticket
    //--------------------------------------------------------

    const escalateTicket = async (ticketId) => {

        const reason =
            prompt("Reason for escalation:");

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

    //--------------------------------------------------------

    useEffect(() => {

        loadDashboard();

    }, []);

    return (

        <div className="container mt-4">

            <h2 className="mb-4">

                Technician Dashboard

            </h2>

            <TechnicianStats
                availableTickets={availableTickets}
                myTickets={myTickets}
            />

            <AvailableTicketsTable
                loading={loading}
                tickets={availableTickets}
                onView={(ticketId) => {

                    setSelectedTicketId(ticketId);
                    setShowDetailsModal(true);

                }}
                onClaim={claimTicket}
            />

            <MyTicketsTable
                loading={loading}
                tickets={myTickets}
                onView={(ticketId) => {

                    setSelectedTicketId(ticketId);
                    setShowDetailsModal(true);

                }}
                onResolve={resolveTicket}
                onEscalate={escalateTicket}
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

export default TechnicianDashboard;