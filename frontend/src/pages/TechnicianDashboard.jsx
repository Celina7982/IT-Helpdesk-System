import { useEffect, useState } from "react";

import ticketService from "../services/ticketService";

import TechnicianStats from "../components/TechnicianStats";
import AvailableTicketsTable from "../components/tickets/AvailableTicketsTable";
import MyTicketsTable from "../components/tickets/MyTicketsTable";
import TicketDetailsModal from "../components/tickets/TicketDetailsModal";

function TechnicianDashboard() {

    const [availableTickets, setAvailableTickets] = useState([]);
    const [myTickets, setMyTickets] = useState([]);

    const [loading, setLoading] = useState(true);

    const [selectedTicketId, setSelectedTicketId] = useState(null);
    const [showDetailsModal, setShowDetailsModal] = useState(false);

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

    const claimTicket = async (ticketId) => {

        await ticketService.claimTicket(ticketId);

        loadDashboard();

    };

    const resolveTicket = async (ticketId) => {

        await ticketService.resolveTicket(ticketId);

        loadDashboard();

    };

    const escalateTicket = async (ticketId) => {

        const reason = prompt("Reason for escalation:");

        if (!reason) return;

        await ticketService.escalateTicket(ticketId, reason);

        loadDashboard();

    };

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
                onView={(id) => {

                    setSelectedTicketId(id);
                    setShowDetailsModal(true);

                }}
                onClaim={claimTicket}
            />

            <MyTicketsTable
                loading={loading}
                tickets={myTickets}
                onView={(id) => {

                    setSelectedTicketId(id);
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