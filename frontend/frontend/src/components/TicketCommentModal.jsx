import { useState, useEffect } from "react";
import ticketService from "../services/ticketService";

function TicketCommentModal({ ticket, onClose }) {

    const [comments, setComments] = useState([]);
    const [newComment, setNewComment] = useState("");
    const [loadingComments, setLoadingComments] = useState(false);
    const [submitting, setSubmitting] = useState(false);
    const [error, setError] = useState("");

    const fetchComments = async () => {

        try {

            setLoadingComments(true);
            setError("");

            // Adjust function name if your ticketService method uses getComments or getTicketComments
            const data = await ticketService.getTicketComments(ticket.ticketId);
            setComments(data);

        } catch (err) {

            console.error("Failed to load comments:", err);
            setError("Unable to load comments.");

        } finally {

            setLoadingComments(false);

        }

    };

    useEffect(() => {

        if (ticket?.ticketId) {
            fetchComments();
        }

    }, [ticket]);

    const handleAddComment = async (e) => {

        e.preventDefault();

        if (!newComment.trim()) return;

        try {

            setSubmitting(true);
            setError("");

            // Adjust method name if your service uses addComment or addTicketComment
            await ticketService.addTicketComment(ticket.ticketId, newComment);

            setNewComment("");
            fetchComments(); // Reload comment list

        } catch (err) {

            console.error("Failed to add comment:", err);
            setError("Unable to post comment.");

        } finally {

            setSubmitting(false);

        }

    };

    if (!ticket) return null;

    return (

        <div
            className="modal d-block show"
            tabIndex="-1"
            style={{ backgroundColor: "rgba(0,0,0,0.5)", zIndex: 1050 }}
        >

            <div className="modal-dialog modal-lg modal-dialog-scrollable">

                <div className="modal-content">

                    <div className="modal-header bg-dark text-white">

                        <h5 className="modal-title">
                            Ticket #{ticket.ticketId}: {ticket.subject}
                        </h5>

                        <button
                            type="button"
                            className="btn-close btn-close-white"
                            onClick={onClose}
                        ></button>

                    </div>

                    <div className="modal-body">

                        {error && (
                            <div className="alert alert-danger py-2">{error}</div>
                        )}

                        <h6 className="fw-bold mb-3">Comment History</h6>

                        {loadingComments ? (

                            <p className="text-muted">Loading comments...</p>

                        ) : comments.length === 0 ? (

                            <p className="text-muted fst-italic">No comments added yet.</p>

                        ) : (

                            <div className="d-flex flex-column gap-2 mb-4">

                                {comments.map((comment, index) => (

                                    <div key={comment.commentId || index} className="p-3 border rounded bg-light">

                                        <div className="d-flex justify-content-between align-items-center mb-1">

                                            <strong className="text-primary">
                                                {comment.authorName || comment.userEmail || "Technician"}
                                            </strong>

                                            <small className="text-muted">
                                                {comment.createdDate ? new Date(comment.createdDate).toLocaleString() : ""}
                                            </small>

                                        </div>

                                        <p className="mb-0">{comment.message || comment.text || comment.commentText}</p>

                                    </div>

                                ))}

                            </div>

                        )}

                        <hr />

                        <form onSubmit={handleAddComment}>

                            <div className="mb-3">

                                <label className="form-label fw-semibold">
                                    Add New Comment / Note
                                </label>

                                <textarea
                                    className="form-control"
                                    rows="3"
                                    placeholder="Write a comment..."
                                    value={newComment}
                                    onChange={(e) => setNewComment(e.target.value)}
                                    required
                                />

                            </div>

                            <button
                                type="submit"
                                className="btn btn-success"
                                disabled={submitting || !newComment.trim()}
                            >
                                {submitting ? "Posting..." : "Post Comment"}
                            </button>

                        </form>

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

export default TicketCommentModal;