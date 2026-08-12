import { useEffect, useState } from "react";
import ticketService from "../../services/ticketService";

function TicketDetailsModal({ show, onClose, ticketId }) {
    const [ticket, setTicket] = useState(null);
    const [comments, setComments] = useState([]);
    const [newComment, setNewComment] = useState("");
    
    const [loading, setLoading] = useState(false);
    const [submittingComment, setSubmittingComment] = useState(false);

    useEffect(() => {
        if (!show || !ticketId) return;

        loadTicketAndComments();
    }, [show, ticketId]);

    const loadTicketAndComments = async () => {
        setLoading(true);

        try {
            // Fetch ticket details and comments concurrently
            const [ticketData, commentsData] = await Promise.all([
                ticketService.getTicketDetails(ticketId),
                ticketService.getComments(ticketId)
            ]);

            setTicket(ticketData);
            setComments(commentsData || []);
        } catch (error) {
            console.error("Failed to fetch modal data:", error);
            alert("Unable to load ticket details or comments.");
        } finally {
            setLoading(false);
        }
    };

    const handleAddComment = async (e) => {
        e.preventDefault();
        if (!newComment.trim()) return;

        try {
            setSubmittingComment(true);

            const createdComment = await ticketService.addComment(ticketId, newComment);

            if (createdComment && typeof createdComment === "object") {
                setComments((prev) => [...prev, createdComment]);
            } else {
                const refreshedComments = await ticketService.getComments(ticketId);
                setComments(refreshedComments || []);
            }

            setNewComment("");
        } catch (error) {
            console.error("Full error response:", error.response);
            alert(`Failed to submit comment: ${error.response?.status || "Error"} - ${error.response?.data?.message || error.message}`);
        } finally {
            setSubmittingComment(false);
        }
    };

    if (!show) return null;

    return (
        <div
            className="modal fade show"
            style={{
                display: "block",
                backgroundColor: "rgba(0,0,0,0.5)"
            }}
        >
            <div className="modal-dialog modal-lg">
                <div className="modal-content">
                    <div className="modal-header">
                        <h5 className="modal-title">
                            Ticket #{ticket?.ticketId || ticketId}
                        </h5>

                        <button
                            className="btn-close"
                            onClick={onClose}
                        ></button>
                    </div>

                    <div className="modal-body">
                        {loading ? (
                            <div className="text-center py-3">
                                <p>Loading details...</p>
                            </div>
                        ) : (
                            <>
                                {/* --- TICKET METADATA TABLE --- */}
                                {ticket && (
                                    <table className="table">
                                        <tbody>
                                            <tr>
                                                <th>Subject</th>
                                                <td>{ticket.subject}</td>
                                            </tr>
                                            <tr>
                                                <th>Description</th>
                                                <td>{ticket.description}</td>
                                            </tr>
                                            <tr>
                                                <th>Customer</th>
                                                <td>{ticket.customerName}</td>
                                            </tr>
                                            <tr>
                                                <th>Company</th>
                                                <td>{ticket.companyName}</td>
                                            </tr>
                                            <tr>
                                                <th>Category</th>
                                                <td>{ticket.category}</td>
                                            </tr>
                                            <tr>
                                                <th>Priority</th>
                                                <td>{ticket.priority}</td>
                                            </tr>
                                            <tr>
                                                <th>Status</th>
                                                <td>{ticket.status}</td>
                                            </tr>
                                            <tr>
                                                <th>Assigned Technician</th>
                                                <td>{ticket.assignedTechnician}</td>
                                            </tr>
                                            <tr>
                                                <th>Created</th>
                                                <td>
                                                    {ticket.createdDate ? new Date(ticket.createdDate).toLocaleString() : ""}
                                                </td>
                                            </tr>
                                            <tr>
                                                <th>Escalated</th>
                                                <td>{ticket.isEscalated ? "Yes" : "No"}</td>
                                            </tr>
                                            {ticket.isEscalated && (
                                                <tr>
                                                    <th>Escalation Reason</th>
                                                    <td>{ticket.escalationReason}</td>
                                                </tr>
                                            )}
                                        </tbody>
                                    </table>
                                )}

                                <hr className="my-4" />

                                {/* --- COMMENTS SECTION --- */}
                                <h5>Comments</h5>

                                {/* List of Existing Comments */}
                                <div 
                                    className="border rounded p-3 mb-3 bg-light"
                                    style={{ maxHeight: "250px", overflowY: "auto" }}
                                >
                                    {comments.length === 0 ? (
                                        <p className="text-muted mb-0 small">No comments posted yet.</p>
                                    ) : (
                                        comments.map((item, idx) => (
                                            <div key={item.commentId || item.id || idx} className="card mb-2 shadow-sm">
                                                <div className="card-body p-2">
                                                    <div className="d-flex justify-content-between">
                                                       
                                                        <strong className="small">
                                                            {item.authorName}
                                                        </strong>

                                                        <small className="text-muted">
                                                            {/* Matched to backend C# CreatedDate */}
                                                            {item.createdDate || item.createdAt ? new Date(item.createdDate || item.createdAt).toLocaleString() : ""}
                                                        </small>
                                                    </div>
                                                    <p className="mb-0 mt-1 small">
                                                        {/* Matched to backend C# Message */}
                                                        {item.message || item.commentText || item.text}
                                                    </p>
                                                </div>
                                            </div>
                                        ))
                                    )}
                                </div>

                                {/* Form to Post a Comment */}
                                <form onSubmit={handleAddComment}>
                                    <div className="mb-2">
                                        <textarea
                                            className="form-control"
                                            rows="3"
                                            placeholder="Write a comment..."
                                            value={newComment}
                                            onChange={(e) => setNewComment(e.target.value)}
                                            required
                                        ></textarea>
                                    </div>
                                    <button
                                        type="submit"
                                        className="btn btn-primary btn-sm"
                                        disabled={submittingComment}
                                    >
                                        {submittingComment ? "Posting..." : "Post Comment"}
                                    </button>
                                </form>
                            </>
                        )}
                    </div>

                    <div className="modal-footer">
                        <button
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