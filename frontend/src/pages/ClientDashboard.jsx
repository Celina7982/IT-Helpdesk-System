import { useEffect, useMemo, useState } from "react";
import clientService from "../services/clientService";
import CreateTicketModal from "../components/CreateTicketModal";
import TicketDetailsModal from "../components/tickets/TicketDetailsModal";
import NotificationPanel from "../components/notifications/NotificationPanel";
import "./ClientDashboard.css";

function ClientDashboard() {

    const [tickets, setTickets] = useState([]);
    const [loading, setLoading] = useState(true);

    const [showModal, setShowModal] = useState(false);

    const [selectedTicketId, setSelectedTicketId] = useState(null);
    const [showDetailsModal, setShowDetailsModal] = useState(false);

    const [selectedFilter, setSelectedFilter] = useState("All");


    // =======================================================
    // Load Tickets
    // =======================================================

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


    // =======================================================
    // Initial Load
    // =======================================================

    useEffect(() => {

        loadTickets();

    }, []);


    // =======================================================
    // Ticket Statistics
    // =======================================================

    const openTickets =
        tickets.filter(
            ticket => ticket.status === "Open"
        ).length;

    const inProgressTickets =
        tickets.filter(
            ticket => ticket.status === "In Progress"
        ).length;

    const resolvedTickets =
        tickets.filter(
            ticket => ticket.status === "Resolved"
        ).length;


    // =======================================================
    // Filter Tickets
    // =======================================================

    const filteredTickets = useMemo(() => {

        if (selectedFilter === "All") {

            return tickets;

        }

        return tickets.filter(
            ticket => ticket.status === selectedFilter
        );

    }, [tickets, selectedFilter]);


    // =======================================================
    // Sort Newest First
    // =======================================================

    const displayedTickets = [...filteredTickets]
        .sort(
            (a, b) =>
                new Date(b.createdDate) -
                new Date(a.createdDate)
        )
        .slice(0, 6);


    // =======================================================
    // Open Ticket
    // =======================================================

    const openTicket = (ticketId) => {

        if (!ticketId) {

            console.error("No ticket ID provided.");

            return;

        }

        setSelectedTicketId(ticketId);

        setShowDetailsModal(true);

    };


    // =======================================================
    // Keyboard Support For Statistic Cards
    // =======================================================

    const handleStatisticKeyDown = (
        event,
        filter
    ) => {

        if (
            event.key === "Enter" ||
            event.key === " "
        ) {

            event.preventDefault();

            setSelectedFilter(
                selectedFilter === filter
                    ? "All"
                    : filter
            );

        }

    };


    // =======================================================
    // Status Styling
    // =======================================================

    const getStatusClass = (status) => {

        switch (status) {

            case "Resolved":
                return "client-status resolved";

            case "In Progress":
                return "client-status progress";

            case "Escalated":
                return "client-status escalated";

            default:
                return "client-status open";

        }

    };


    // =======================================================
    // Priority Styling
    // =======================================================

    const getPriorityClass = (priority) => {

        switch (priority?.toLowerCase()) {

            case "high":
                return "client-priority high";

            case "critical":
                return "client-priority critical";

            case "low":
                return "client-priority low";

            default:
                return "client-priority medium";

        }

    };


    // =======================================================
    // Loading
    // =======================================================

    if (loading) {

        return (

            <div className="client-dashboard-loading">

                <div className="spinner-border text-primary"></div>

                <p>
                    Loading your support dashboard...
                </p>

            </div>

        );

    }


    // =======================================================
    // Dashboard
    // =======================================================

    return (

        <div className="client-dashboard">


            {/* =================================================
                HEADER
            ================================================= */}

            <header className="client-dashboard-header">

    <div className="client-brand">

        <div className="client-logo-wrapper">

            <img
                src="/images/company-logo.png"
                alt="Company Logo"
                className="client-company-logo"
            />

        </div>


        <div className="client-brand-divider"></div>


        <div className="client-portal-label">
            Support Portal
        </div>

    </div>


    <div className="client-header-actions">

        <NotificationPanel
            onViewTicket={openTicket}
        />


        <div className="client-user-profile">

            <div className="client-user-avatar">
                C
            </div>


            <div className="client-user-info">

                <span className="client-user-name">
                    Customer
                </span>

                <span className="client-user-role">
                    Client
                </span>

            </div>


            <span className="client-user-chevron">
                ▾
            </span>

        </div>

    </div>

</header>
            {/* =================================================
                PAGE INTRO
            ================================================= */}

            <div className="client-page-intro">

                <div>

                    <div className="client-eyebrow">
                        CUSTOMER PORTAL
                    </div>


                    <h1>
                        Welcome back 👋
                    </h1>


                    <p>
                        Here's an overview of your support requests.
                    </p>

                </div>

            </div>


            {/* =================================================
                WELCOME / SUPPORT SUMMARY
            ================================================= */}

            <div className="client-welcome-card">

                <div>

                    <div className="client-welcome-icon">
                        🎧
                    </div>

                </div>


                <div className="client-welcome-content">

                    <h3>
                        Need help with something?
                    </h3>


                    <p>
                        Submit a support ticket and our team
                        will get back to you as soon as possible.
                    </p>

                </div>


                <button
                    type="button"
                    className="client-welcome-button"
                    onClick={() => setShowModal(true)}
                >

                    Create a ticket

                    <span>
                        →
                    </span>

                </button>

            </div>


            {/* =================================================
                SUPPORT OVERVIEW HEADING
            ================================================= */}

            <div className="client-section-heading">

                <div>

                    <h2>
                        Your support overview
                    </h2>


                    <p>
                        A quick look at your current tickets.
                    </p>

                </div>

            </div>


            {/* =================================================
                STATISTICS
            ================================================= */}

            <div className="client-statistics">


                {/* =================================================
                    OPEN TICKETS
                ================================================= */}

                <div
                    className={`client-stat-card open ${
                        selectedFilter === "Open"
                            ? "active"
                            : ""
                    }`}
                    role="button"
                    tabIndex="0"
                    onClick={() =>
                        setSelectedFilter(
                            selectedFilter === "Open"
                                ? "All"
                                : "Open"
                        )
                    }
                    onKeyDown={(event) =>
                        handleStatisticKeyDown(
                            event,
                            "Open"
                        )
                    }
                >

                    <div className="client-stat-top">

                        <div className="client-stat-icon">
                            📂
                        </div>


                        <span className="client-stat-arrow">
                            →
                        </span>

                    </div>


                    <div className="client-stat-number">
                        {openTickets}
                    </div>


                    <div className="client-stat-title">
                        Open Tickets
                    </div>


                    <div className="client-stat-description">
                        Waiting for support
                    </div>

                </div>


                {/* =================================================
                    IN PROGRESS
                ================================================= */}

                <div
                    className={`client-stat-card progress ${
                        selectedFilter === "In Progress"
                            ? "active"
                            : ""
                    }`}
                    role="button"
                    tabIndex="0"
                    onClick={() =>
                        setSelectedFilter(
                            selectedFilter === "In Progress"
                                ? "All"
                                : "In Progress"
                        )
                    }
                    onKeyDown={(event) =>
                        handleStatisticKeyDown(
                            event,
                            "In Progress"
                        )
                    }
                >

                    <div className="client-stat-top">

                        <div className="client-stat-icon">
                            ⚙️
                        </div>


                        <span className="client-stat-arrow">
                            →
                        </span>

                    </div>


                    <div className="client-stat-number">
                        {inProgressTickets}
                    </div>


                    <div className="client-stat-title">
                        In Progress
                    </div>


                    <div className="client-stat-description">
                        Currently being worked on
                    </div>

                </div>


                {/* =================================================
                    RESOLVED
                ================================================= */}

                <div
                    className={`client-stat-card resolved ${
                        selectedFilter === "Resolved"
                            ? "active"
                            : ""
                    }`}
                    role="button"
                    tabIndex="0"
                    onClick={() =>
                        setSelectedFilter(
                            selectedFilter === "Resolved"
                                ? "All"
                                : "Resolved"
                        )
                    }
                    onKeyDown={(event) =>
                        handleStatisticKeyDown(
                            event,
                            "Resolved"
                        )
                    }
                >

                    <div className="client-stat-top">

                        <div className="client-stat-icon">
                            ✓
                        </div>


                        <span className="client-stat-arrow">
                            →
                        </span>

                    </div>


                    <div className="client-stat-number">
                        {resolvedTickets}
                    </div>


                    <div className="client-stat-title">
                        Resolved
                    </div>


                    <div className="client-stat-description">
                        Successfully completed
                    </div>

                </div>

            </div>


            {/* =================================================
                TICKETS SECTION
            ================================================= */}

            <div className="client-tickets-section">


                {/* =================================================
                    TICKET SECTION HEADER
                ================================================= */}

                <div className="client-section-header">

                    <div>

                        <h2>
                            Your Tickets
                        </h2>


                        <p>
                            Track and manage your support requests.
                        </p>

                    </div>


                    {/* =================================================
                        TICKET FILTERS
                    ================================================= */}

                    <div className="client-ticket-filters">

                        <button
                            type="button"
                            className={
                                selectedFilter === "All"
                                    ? "active"
                                    : ""
                            }
                            onClick={() =>
                                setSelectedFilter("All")
                            }
                        >
                            All
                        </button>


                        <button
                            type="button"
                            className={
                                selectedFilter === "Open"
                                    ? "active"
                                    : ""
                            }
                            onClick={() =>
                                setSelectedFilter("Open")
                            }
                        >
                            Open
                        </button>


                        <button
                            type="button"
                            className={
                                selectedFilter === "In Progress"
                                    ? "active"
                                    : ""
                            }
                            onClick={() =>
                                setSelectedFilter("In Progress")
                            }
                        >
                            In Progress
                        </button>


                        <button
                            type="button"
                            className={
                                selectedFilter === "Resolved"
                                    ? "active"
                                    : ""
                            }
                            onClick={() =>
                                setSelectedFilter("Resolved")
                            }
                        >
                            Resolved
                        </button>

                    </div>

                </div>


                {/* =================================================
                    TICKET LIST
                ================================================= */}

                <div className="client-ticket-list">


                    {displayedTickets.length === 0 ? (

                        /* =================================================
                            EMPTY STATE
                        ================================================= */

                        <div className="client-empty-state">

                            <div className="client-empty-icon">
                                📭
                            </div>


                            <h3>
                                No tickets found
                            </h3>


                            <p>
                                You don't have any tickets in
                                this category.
                            </p>


                            <button
                                type="button"
                                onClick={() => {

                                    setSelectedFilter("All");

                                    setShowModal(true);

                                }}
                            >
                                + Create a ticket
                            </button>

                        </div>

                    ) : (

                        /* =================================================
                            TICKET CARDS
                        ================================================= */

                        displayedTickets.map(ticket => (

                            <div
                                className="client-ticket-card"
                                key={ticket.ticketId}
                            >


                                {/* =================================================
                                    TICKET INFORMATION
                                ================================================= */}

                                <div className="client-ticket-main">

                                    <div className="client-ticket-number">
                                        Ticket #{ticket.ticketId}
                                    </div>


                                    <h3>
                                        {ticket.subject}
                                    </h3>


                                    <div className="client-ticket-meta">

                                        <span>
                                            Created{" "}
                                            {new Date(
                                                ticket.createdDate
                                            ).toLocaleDateString()}
                                        </span>


                                        <span className="meta-divider">
                                            •
                                        </span>


                                        <span>
                                            {ticket.category}
                                        </span>

                                    </div>

                                </div>


                                {/* =================================================
                                    STATUS + PRIORITY
                                ================================================= */}

                                <div className="client-ticket-middle">

                                    <span
                                        className={
                                            getStatusClass(
                                                ticket.status
                                            )
                                        }
                                    >

                                        <span className="status-dot">
                                        </span>

                                        {ticket.status}

                                    </span>


                                    <span
                                        className={
                                            getPriorityClass(
                                                ticket.priority
                                            )
                                        }
                                    >
                                        {ticket.priority}
                                    </span>

                                </div>


                                {/* =================================================
                                    VIEW TICKET
                                ================================================= */}

                                <div className="client-ticket-action">

                                    <button
                                        type="button"
                                        onClick={() =>
                                            openTicket(
                                                ticket.ticketId
                                            )
                                        }
                                    >

                                        View Ticket

                                        <span>
                                            →
                                        </span>

                                    </button>

                                </div>

                            </div>

                        ))

                    )}

                </div>


                {/* =================================================
                    MORE TICKETS INDICATOR
                ================================================= */}

                {filteredTickets.length > 6 && (

                    <div className="client-more-tickets">

                        Showing 6 of {filteredTickets.length} tickets

                    </div>

                )}

            </div>


           
            {/* =================================================
                CREATE TICKET MODAL
            ================================================= */}

            <CreateTicketModal
                show={showModal}
                onClose={() => setShowModal(false)}
                onTicketCreated={() => {

                    setShowModal(false);

                    loadTickets();

                }}
            />


            {/* =================================================
                TICKET DETAILS MODAL
            ================================================= */}

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