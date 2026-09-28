import { useEffect, useState } from "react";
import notificationService from "../../services/notificationService";

function NotificationPanel({ onViewTicket }) {

  const [notifications, setNotifications] = useState([]);
const [unreadCount, setUnreadCount] = useState(0);
const [loading, setLoading] = useState(true);
const [showPanel, setShowPanel] = useState(false);
const [currentPage, setCurrentPage] = useState(1);
const [totalCount, setTotalCount] = useState(0);


const pageSize = 10;

const totalPages = Math.max(
    1,
    Math.ceil(totalCount / pageSize)
);
//--------------------------------------------------
// Load paginated notifications
//--------------------------------------------------

const loadNotifications = async () => {

    setLoading(true);

    try {

        const data =
            await notificationService.getMyNotifications(
                currentPage,
                pageSize
            );

        setNotifications(
            data.items || []
        );

        setTotalCount(
            data.totalCount || 0
        );

    }
    catch (error) {

        console.error(
            "Unable to load notifications:",
            error
        );

    }
    finally {

        setLoading(false);

    }
};
    


   //--------------------------------------------------
// Load unread notification count
//--------------------------------------------------

const loadUnreadCount = async () => {

    try {

        const data =
            await notificationService
                .getUnreadNotificationCount();

        setUnreadCount(
            data.count || 0
        );

    }
    catch (error) {

        console.error(
            "Unable to load unread notification count:",
            error
        );

    }
};


    //--------------------------------------------------
    // Initial Load
    //--------------------------------------------------

    useEffect(() => {

        loadNotifications();

        loadUnreadCount();

    }, [currentPage]);


    //--------------------------------------------------
    // Automatically refresh notifications
    // every 30 seconds
    //--------------------------------------------------

    useEffect(() => {

        const interval = setInterval(() => {

            loadNotifications();

            loadUnreadCount();

        }, 30000);

        return () => clearInterval(interval);

    }, [currentPage]);


    //--------------------------------------------------
    // View Notification / Ticket
    //--------------------------------------------------
    //
    // When the user clicks "View Ticket":
    //
    // 1. Mark notification as read
    // 2. Update notification locally
    // 3. Update unread count
    // 4. Close notification panel
    // 5. Open related ticket
    //
    //--------------------------------------------------

    const handleViewTicket = async (notification) => {

        try {

            //--------------------------------------------------
            // Mark as read if necessary
            //--------------------------------------------------

            if (!notification.isRead) {

                await notificationService.markAsRead(
                    notification.notificationId
                );

                setNotifications(
                    previousNotifications =>
                        previousNotifications.map(item =>
                            item.notificationId ===
                            notification.notificationId
                                ? {
                                    ...item,
                                    isRead: true
                                }
                                : item
                        )
                );

                setUnreadCount(
                    previousCount =>
                        Math.max(0, previousCount - 1)
                );

            }


            //--------------------------------------------------
            // Make sure notification has a TicketId
            //--------------------------------------------------

            if (!notification.ticketId) {

                console.warn(
                    "This notification is not linked to a ticket.",
                    notification
                );

                return;
            }


            //--------------------------------------------------
            // Close notification panel
            //--------------------------------------------------

            setShowPanel(false);


            //--------------------------------------------------
            // Open TicketDetailsModal
            //--------------------------------------------------

            if (onViewTicket) {

                onViewTicket(
                    notification.ticketId
                );

            }

        }
        catch (error) {

            console.error(
                "Unable to open notification:",
                error
            );

        }
    };


    //--------------------------------------------------
    // Delete notification
    //--------------------------------------------------

    const handleDelete = async (
        event,
        notificationId
    ) => {

        event.stopPropagation();

        try {

            await notificationService.deleteNotification(
                notificationId
            );

            setNotifications(
                previousNotifications =>
                    previousNotifications.filter(
                        notification =>
                            notification.notificationId !==
                            notificationId
                    )
            );

        }
        catch (error) {

            console.error(
                "Unable to delete notification:",
                error
            );

        }
    };


    //--------------------------------------------------
    // Format Date
    //--------------------------------------------------

    const formatDate = (date) => {

        if (!date)
            return "";

        return new Date(date).toLocaleString();

    };


    //--------------------------------------------------
    // Component
    //--------------------------------------------------

    return (

        <div
            className="position-relative"
            style={{
                display: "inline-block"
            }}
        >

            {/* =================================================
                NOTIFICATION BELL
            ================================================= */}

            <button
                type="button"
                className="btn btn-light position-relative"
                onClick={() =>
                    setShowPanel(!showPanel)
                }
                title="Notifications"
            >

                <span
                    style={{
                        fontSize: "22px"
                    }}
                >
                    🔔
                </span>


                {/* Unread Badge */}

                {unreadCount > 0 && (

                    <span
                        className="position-absolute top-0 start-100 translate-middle badge rounded-pill bg-danger"
                    >
                        {unreadCount}
                    </span>

                )}

            </button>


            {/* =================================================
                NOTIFICATION PANEL
            ================================================= */}

            {showPanel && (

                <div
                    className="card shadow position-absolute"
                    style={{
                        width: "380px",
                        right: "0",
                        top: "50px",
                        zIndex: 1050
                    }}
                >

                    {/* =================================================
                        HEADER
                    ================================================= */}

                    <div
                        className="card-header d-flex justify-content-between align-items-center"
                    >

                        <strong>
                            Notifications
                        </strong>

                        <button
                            type="button"
                            className="btn-close"
                            onClick={() =>
                                setShowPanel(false)
                            }
                        />

                    </div>


                    {/* =================================================
                        BODY
                    ================================================= */}

                    <div
                        className="card-body p-0"
                        style={{
                            maxHeight: "450px",
                            overflowY: "auto"
                        }}
                    >

                        {/* =================================================
                            LOADING
                        ================================================= */}

                        {loading ? (

                            <div className="text-center p-4">

                                <div className="spinner-border text-primary">
                                </div>

                                <p className="mt-2 mb-0">
                                    Loading notifications...
                                </p>

                            </div>

                        ) : notifications.length === 0 ? (

                            /* =================================================
                                NO NOTIFICATIONS
                            ================================================= */

                            <div
                                className="text-center p-4 text-muted"
                            >

                                <div
                                    style={{
                                        fontSize: "35px"
                                    }}
                                >
                                    🔔
                                </div>

                                <p className="mb-0">
                                    You have no notifications.
                                </p>

                            </div>

                        ) : (

                            /* =================================================
                                NOTIFICATIONS
                            ================================================= */

                            notifications.map(notification => (

                                <div
                                    key={notification.notificationId}
                                    className={
                                        notification.isRead
                                            ? "p-3 border-bottom"
                                            : "p-3 border-bottom bg-light"
                                    }
                                >

                                    {/* =================================================
                                        NOTIFICATION HEADER
                                    ================================================= */}

                                    <div
                                        className="d-flex justify-content-between align-items-start"
                                    >

                                        <div>

                                            {/* Unread Indicator */}

                                            {!notification.isRead && (

                                                <span
                                                    className="text-primary me-2"
                                                    style={{
                                                        fontSize: "12px"
                                                    }}
                                                    title="Unread"
                                                >
                                                    ●
                                                </span>

                                            )}

                                            <strong>
                                                {notification.title}
                                            </strong>

                                        </div>

                                    </div>


                                    {/* =================================================
                                        NOTIFICATION MESSAGE
                                    ================================================= */}

                                    <p
                                        className="mb-2 mt-2"
                                        style={{
                                            fontSize: "14px"
                                        }}
                                    >
                                        {notification.message}
                                    </p>


                                    {/* =================================================
                                        DATE
                                    ================================================= */}

                                    <small className="text-muted">

                                        {formatDate(
                                            notification.dateCreated
                                        )}

                                    </small>


                                    {/* =================================================
                                        ACTIONS
                                    ================================================= */}

                                    <div className="mt-2">

                                        {/* View Ticket */}

                                        {notification.ticketId && (

                                            <button
                                                type="button"
                                                className="btn btn-sm btn-outline-primary me-2"
                                                onClick={() =>
                                                    handleViewTicket(
                                                        notification
                                                    )
                                                }
                                            >
                                                View Ticket →
                                            </button>

                                        )}


                                        {/* Delete */}

                                        <button
                                            type="button"
                                            className="btn btn-sm btn-outline-danger"
                                            onClick={(event) =>
                                                handleDelete(
                                                    event,
                                                    notification.notificationId
                                                )
                                            }
                                        >
                                            Delete
                                        </button>

                                    </div>

                                </div>

                            ))

                        )}

                    </div>


                    {/* =================================================
                        PAGINATION
                    ================================================= */}

                    <div
                        className="card-footer d-flex justify-content-between align-items-center"
                    >

                        <button
                            type="button"
                            className="btn btn-sm btn-outline-secondary"
                            disabled={currentPage === 1}
                            onClick={() =>
                                setCurrentPage(
                                    previousPage =>
                                        Math.max(
                                            1,
                                            previousPage - 1
                                        )
                                )
                            }
                        >
                            ← Previous
                        </button>


                        <span className="small text-muted">
                           Page {currentPage} of {totalPages}
                        </span>


                        <button
                            type="button"
                            className="btn btn-sm btn-outline-secondary"
                            disabled={
                                currentPage >= totalPages
                            }
                            onClick={() =>
                                setCurrentPage(
                                    previousPage =>
                                        previousPage + 1
                                )
                            }
                        >
                            Next →
                        </button>

                    </div>

                </div>

            )}

        </div>

    );
}

export default NotificationPanel;