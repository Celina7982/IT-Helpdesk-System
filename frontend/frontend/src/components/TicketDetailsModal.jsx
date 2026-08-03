import { useState, useEffect } from "react";
import clientService from "../services/clientService";

function TicketDetailsModal({ ticketId, show, onClose }) {

    const [ticket, setTicket] = useState(null);
    const [comments, setComments] = useState([]);
    const [newComment, setNewComment] = useState("");
    const [loading, setLoading] = useState(false);
    const [submitting, setSubmitting] = useState(false);

    useEffect(() => {
        if (show && ticketId) {
            fetchTicketAndComments();
        }
    }, [show, ticketId]);

   // 1. Declare the function first
const fetchTicketAndComments = async () => {
    setLoading(true);
    try {
        const ticketData = await clientService.getTicketById(ticketId);
        const commentsData = await clientService.getComments(ticketId);

        setTicket(ticketData);
        setComments(commentsData);
    } catch (error) {
        console.error("Error fetching ticket details or comments:", error);
    } finally {
        setLoading(false);
    }
};

// 2. Now useEffect can call it without errors
useEffect(() => {
    if (show && ticketId) {
        fetchTicketAndComments();
    }
}, [show, ticketId]);

    const handleAddComment = async (e) => {
        e.preventDefault();
        if (!newComment.trim()) return;

        setSubmitting(true);

        try {
            const addedComment = await clientService.addComment(ticketId, {
                message: newComment
            });

            setComments([...comments, addedComment]);
            setNewComment("");
        } catch (error) {
            console.error("Failed to post comment:", error);
            alert("Unable to post comment.");
        } finally {
            setSubmitting(false);
        }
    };

    if (!show) return null;

    return (
        <div
            className="modal d-block"
            style={{ backgroundColor: "rgba(0,0,0,0.5)" }}
        >
            <div className="modal-dialog modal-lg modal-dialog-scrollable">
                <div className="modal-content">

                    <div className="modal-header">
                        <h5 className="modal-title">
                            Ticket #{ticketId} {ticket && `- ${ticket.subject}`}
                        </h5>
                        <button
                            className="btn-close"
                            onClick={onClose}
                        />
                    </div>

                    <div className="modal-body">

                        {loading ? (
                            <div className="text-center py-4">
                                <div className="spinner-border text-primary" role="status">
                                    <span className="visually-hidden">Loading...</span>
                                </div>
                            </div>
                        ) : ticket ? (
                            <>
                                {/* Ticket Details Summary Header */}
                                <div className="card mb-4 border-light shadow-sm">
                                    <div className="card-body">

                                        <div className="d-flex justify-content-between align-items-center mb-2">
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
                                            <small className="text-muted">
                                                Created: {new Date(ticket.createdDate).toLocaleString()}
                                            </small>
                                        </div>

                                        <p className="card-text my-3">
                                            {ticket.description}
                                        </p>

                                        <div className="row text-muted small border-top pt-2">
                                            <div className="col-md-4">
                                                <strong>Category:</strong> {ticket.category}
                                            </div>
                                            <div className="col-md-4">
                                                <strong>Priority:</strong> {ticket.priority}
                                            </div>
                                            <div className="col-md-4">
                                                <strong>Company:</strong> {ticket.companyName}
                                            </div>
                                        </div>

                                    </div>
                                </div>

                                {/* Ticket Comments Thread */}
                                <h6 className="fw-bold mb-3">Discussion & Activity</h6>

                                <div className="mb-4">
                                    {comments.length === 0 ? (
                                        <div className="text-muted small italic">
                                            No comments yet on this ticket.
                                        </div>
                                    ) : (
                                        comments.map((comment) => (
                                            <div
                                                key={comment.commentId || comment.id}
                                                className="card mb-2 bg-light border-0"
                                            >
                                                <div className="card-body p-3">
                                                    <div className="d-flex justify-content-between mb-1">
                                                        <strong className="small">
                                                            {comment.authorName || comment.userName || "User"}
                                                        </strong>
                                                        <small className="text-muted">
                                                            {new Date(comment.createdDate || comment.createdAt).toLocaleString()}
                                                        </small>
                                                    </div>
                                                    <p
                                                        className="card-text mb-0 small"
                                                        style={{ whiteSpace: "pre-wrap" }}
                                                    >
                                                        {comment.message || comment.content}
                                                    </p>
                                                </div>
                                            </div>
                                        ))
                                    )}
                                </div>

                                {/* Post Comment Input Form */}
                                <form onSubmit={handleAddComment}>
                                    <div className="mb-2">
                                        <textarea
                                            className="form-control"
                                            rows="3"
                                            placeholder="Write an update or reply..."
                                            value={newComment}
                                            onChange={(e) => setNewComment(e.target.value)}
                                            required
                                        />
                                    </div>
                                    <div className="text-end">
                                        <button
                                            type="submit"
                                            className="btn btn-primary btn-sm"
                                            disabled={submitting}
                                        >
                                            {submitting ? "Posting..." : "Post Comment"}
                                        </button>
                                    </div>
                                </form>
                            </>
                        ) : (
                            <div className="text-danger">
                                Unable to load ticket details.
                            </div>
                        )}

                    </div>

                    <div className="modal-footer">
                        <button
                            type="button"
                            className="btn btn-secondary"
                            onClick={onClose}
                        >
                            Close
                        </button>
                    </div>

                </div>
            </div>
        </div>
    );
}

export default TicketDetailsModal;