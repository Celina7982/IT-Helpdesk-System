import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import ticketService from "../../services/ticketService";
import jobCardService from "../../services/jobCardService";


function TicketDetailsModal({ show, onClose, ticketId }) {

<<<<<<< HEAD
    const navigate = useNavigate();

=======
>>>>>>> b57c8eb9e8154251e971dc56694082053e823d09
    const [ticket, setTicket] = useState(null);
    const [jobCard, setJobCard] = useState(null);
    const [comments, setComments] = useState([]);
    const [newComment, setNewComment] = useState("");

    const [loading, setLoading] = useState(false);
    const [submittingComment, setSubmittingComment] = useState(false);

    const [editingCommentId, setEditingCommentId] = useState(null);
    const [editingMessage, setEditingMessage] = useState("");
    const [updatingComment, setUpdatingComment] = useState(false);


    //-------------------------------------------------------
    // Load Ticket + Comments
    //-------------------------------------------------------

    useEffect(() => {

        if (!show || !ticketId) {
            return;
        }

        const fetchTicketAndComments = async () => {

            setLoading(true);

            try {

                const [ticketData, commentsData] = await Promise.all([
                    ticketService.getTicketDetails(ticketId),
                    ticketService.getComments(ticketId)
                ]);

                setTicket(ticketData);
                setComments(commentsData || []);

            }
            catch (error) {

                console.error(
                    "Failed to fetch ticket details and comments:",
                    error
                );

                alert("Unable to load ticket details or comments.");

            }
            finally {

                setLoading(false);

            }

        };

        fetchTicketAndComments();

    }, [show, ticketId]);

<<<<<<< HEAD
    
const loadTicketAndComments = async () => {

    setLoading(true);

    try {

        // ---------------------------------------------
        // Load ticket + comments
        // ---------------------------------------------

        const [ticketData, commentsData] = await Promise.all([
            ticketService.getTicketDetails(ticketId),
            ticketService.getComments(ticketId)
        ]);

        setTicket(ticketData);
        setComments(commentsData || []);


        // ---------------------------------------------
        // Check whether this ticket has a Job Card
        // ---------------------------------------------

        try {

            const jobCardData =
                await jobCardService.getByTicketId(ticketId);

            setJobCard(jobCardData);

        } catch (jobCardError) {

            // 404 simply means this ticket has no Job Card.
            if (jobCardError.response?.status === 404) {

                setJobCard(null);

            } else {

                console.error(
                    "Failed to load Job Card:",
                    jobCardError
                );

                setJobCard(null);
            }
        }

    } catch (error) {

        console.error(
            "Failed to fetch modal data:",
            error
        );

        alert(
            "Unable to load ticket details or comments."
        );

    } finally {

        setLoading(false);

    }
};

=======

    //-------------------------------------------------------
    // Add Comment
    //-------------------------------------------------------
>>>>>>> b57c8eb9e8154251e971dc56694082053e823d09

    const handleAddComment = async (e) => {

        e.preventDefault();

        if (!newComment.trim()) {
            return;
        }

        try {

            setSubmittingComment(true);

            const createdComment = await ticketService.addComment(
                ticketId,
                newComment.trim()
            );

            if (createdComment && typeof createdComment === "object") {

                setComments(prev => [
                    ...prev,
                    createdComment
                ]);

            }
            else {

                const refreshedComments =
                    await ticketService.getComments(ticketId);

                setComments(refreshedComments || []);

            }

            setNewComment("");

        }
        catch (error) {

            console.error("Failed to submit comment:", error);

            alert(
                error.response?.data?.message ||
                "Failed to submit comment."
            );

        }
        finally {

            setSubmittingComment(false);

        }

    };


    //-------------------------------------------------------
    // Start Editing Comment
    //-------------------------------------------------------

    const handleStartEdit = (comment) => {

        setEditingCommentId(comment.commentId);

        setEditingMessage(
            comment.message ||
            comment.commentText ||
            comment.text ||
            ""
        );

    };


    //-------------------------------------------------------
    // Cancel Editing
    //-------------------------------------------------------

    const handleCancelEdit = () => {

        setEditingCommentId(null);

        setEditingMessage("");

    };


    //-------------------------------------------------------
    // Save Edited Comment
    //-------------------------------------------------------

    const handleUpdateComment = async (commentId) => {

        if (!editingMessage.trim()) {

            alert("Comment cannot be empty.");

            return;

        }

        try {

            setUpdatingComment(true);

            const updatedComment =
                await ticketService.updateComment(
                    ticketId,
                    commentId,
                    editingMessage.trim()
                );

            setComments(prev =>
                prev.map(comment =>
                    comment.commentId === commentId
                        ? {
                            ...comment,
                            ...updatedComment
                        }
                        : comment
                )
            );

            setEditingCommentId(null);
            setEditingMessage("");

        }
        catch (error) {

            console.error(
                "Failed to update comment:",
                error
            );

            alert(
                error.response?.data?.message ||
                "Unable to update comment."
            );

        }
        finally {

            setUpdatingComment(false);

        }

    };


    //-------------------------------------------------------
    // Close modal
    //-------------------------------------------------------

    const handleClose = () => {

        setEditingCommentId(null);
        setEditingMessage("");
        setNewComment("");

        onClose();

    };


    //-------------------------------------------------------
    // Don't render if modal is closed
    //-------------------------------------------------------

    if (!show) {
        return null;
    }


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


                    {/* HEADER */}

                    <div className="modal-header">

                        <h5 className="modal-title">
                            Ticket #{ticket?.ticketId || ticketId}
                        </h5>

                        <button
                            type="button"
                            className="btn-close"
                            onClick={handleClose}
                        >
                        </button>

                    </div>


                    {/* BODY */}

                    <div className="modal-body">

                        {loading ? (

                            <div className="text-center py-3">

                                <div className="spinner-border text-primary">
                                </div>

                                <p className="mt-2">
                                    Loading details...
                                </p>

                            </div>

                        ) : (

                            <>

                                {/* TICKET DETAILS */}

                                {ticket && (

                                    <table className="table">

                                        <tbody>

                                            <tr>
                                                <th>Subject</th>
                                                <td>
                                                    {ticket.subject}
                                                </td>
                                            </tr>

                                            <tr>
                                                <th>Description</th>
                                                <td>
                                                    {ticket.description}
                                                </td>
                                            </tr>

                                            <tr>
                                                <th>Customer</th>
                                                <td>
                                                    {ticket.customerName}
                                                </td>
                                            </tr>

                                            <tr>
                                                <th>Company</th>
                                                <td>
                                                    {ticket.companyName}
                                                </td>
                                            </tr>

                                            <tr>
                                                <th>Category</th>
                                                <td>
                                                    {ticket.category}
                                                </td>
                                            </tr>

                                            <tr>
                                                <th>Priority</th>
                                                <td>
                                                    {ticket.priority}
                                                </td>
                                            </tr>

                                            <tr>
                                                <th>Status</th>
                                                <td>
                                                    {ticket.status}
                                                </td>
                                            </tr>

                                            <tr>
                                                <th>
                                                    Assigned Technician
                                                </th>

                                                <td>
                                                    {ticket.assignedTechnician ||
                                                        "Unassigned"}
                                                </td>
                                            </tr>

                                            <tr>
                                                <th>Created</th>

                                                <td>
                                                    {ticket.createdDate
                                                        ? new Date(
                                                            ticket.createdDate
                                                        ).toLocaleString()
                                                        : ""}
                                                </td>

                                            </tr>

                                            <tr>
                                                <th>Escalated</th>

                                                <td>
                                                    {ticket.isEscalated
                                                        ? "Yes"
                                                        : "No"}
                                                </td>

                                            </tr>

                                            {ticket.isEscalated && (

                                                <tr>

                                                    <th>
                                                        Escalation Reason
                                                    </th>

                                                    <td>
                                                        {ticket.escalationReason}
                                                    </td>

                                                </tr>

                                            )}

                                        </tbody>

                                    </table>

                                )}


                                <hr className="my-4" />

<<<<<<< HEAD
                                <hr className="my-4" />

{/* ---------------------------------------- */}
{/* JOB CARD */}
{/* ---------------------------------------- */}

{jobCard && (
    <div className="mb-4">

        <div className="d-flex justify-content-between align-items-center">

            <div>
                <h5 className="mb-1">
                    Job Card
                </h5>

                <small className="text-muted">
                    {jobCard.jobNumber
                        ? `Job Card ${jobCard.jobNumber}`
                        : `Job Card #${jobCard.jobCardId}`}
                </small>
            </div>

            <button
                type="button"
                className="btn btn-primary"
                onClick={() => {
                    onClose();

                    navigate(
                        `/admin/jobcards/${jobCard.jobCardId}`
                    );
                }}
            >
                View / Edit Job Card
            </button>

        </div>

    </div>
)}

<hr className="my-4" />

<h5>Comments</h5>
=======
>>>>>>> b57c8eb9e8154251e971dc56694082053e823d09

                                {/* COMMENTS */}

                                <h5 className="mb-3">
                                    Comments
                                </h5>


                                <div
                                    className="border rounded p-3 mb-3 bg-light"
                                    style={{
                                        maxHeight: "300px",
                                        overflowY: "auto"
                                    }}
                                >

                                    {comments.length === 0 ? (

                                        <p className="text-muted mb-0 small">
                                            No comments posted yet.
                                        </p>

                                    ) : (

                                        comments.map((item, index) => {

                                            const commentId =
                                                item.commentId ||
                                                item.id;

                                            const message =
                                                item.message ||
                                                item.commentText ||
                                                item.text ||
                                                "";

                                            return (

                                                <div
                                                    key={commentId || index}
                                                    className="card mb-2 shadow-sm"
                                                >

                                                    <div className="card-body p-3">

                                                        {/* COMMENT HEADER */}

                                                        <div className="d-flex justify-content-between align-items-center">

                                                            <strong className="small">
                                                                {item.authorName}
                                                            </strong>

                                                            <small className="text-muted">

                                                                {item.createdDate ||
                                                                item.createdAt
                                                                    ? new Date(
                                                                        item.createdDate ||
                                                                        item.createdAt
                                                                    ).toLocaleString()
                                                                    : ""}

                                                            </small>

                                                        </div>


                                                        {/* EDIT MODE */}

                                                        {editingCommentId ===
                                                        commentId ? (

                                                            <div className="mt-2">

                                                                <textarea
                                                                    className="form-control mb-2"
                                                                    rows="3"
                                                                    value={
                                                                        editingMessage
                                                                    }
                                                                    onChange={(e) =>
                                                                        setEditingMessage(
                                                                            e.target.value
                                                                        )
                                                                    }
                                                                />

                                                                <button
                                                                    type="button"
                                                                    className="btn btn-success btn-sm me-2"
                                                                    onClick={() =>
                                                                        handleUpdateComment(
                                                                            commentId
                                                                        )
                                                                    }
                                                                    disabled={
                                                                        updatingComment
                                                                    }
                                                                >

                                                                    {updatingComment
                                                                        ? "Saving..."
                                                                        : "Save"}

                                                                </button>

                                                                <button
                                                                    type="button"
                                                                    className="btn btn-secondary btn-sm"
                                                                    onClick={
                                                                        handleCancelEdit
                                                                    }
                                                                    disabled={
                                                                        updatingComment
                                                                    }
                                                                >
                                                                    Cancel
                                                                </button>

                                                            </div>

                                                        ) : (

                                                            <>

                                                                <p className="mb-2 mt-2 small">
                                                                    {message}
                                                                </p>

                                                                <button
                                                                    type="button"
                                                                    className="btn btn-outline-primary btn-sm"
                                                                    onClick={() =>
                                                                        handleStartEdit(
                                                                            item
                                                                        )
                                                                    }
                                                                >
                                                                    Edit
                                                                </button>

                                                            </>

                                                        )}

                                                    </div>

                                                </div>

                                            );

                                        })

                                    )}

                                </div>


                                {/* ADD COMMENT */}

                                <form onSubmit={handleAddComment}>

                                    <div className="mb-2">

                                        <textarea
                                            className="form-control"
                                            rows="3"
                                            placeholder="Write a comment..."
                                            value={newComment}
                                            onChange={(e) =>
                                                setNewComment(
                                                    e.target.value
                                                )
                                            }
                                            required
                                        >
                                        </textarea>

                                    </div>

                                    <button
                                        type="submit"
                                        className="btn btn-primary btn-sm"
                                        disabled={submittingComment}
                                    >

                                        {submittingComment
                                            ? "Posting..."
                                            : "Post Comment"}

                                    </button>

                                </form>

                            </>

                        )}

                    </div>


                    {/* FOOTER */}

                    <div className="modal-footer">

                        <button
                            type="button"
                            className="btn btn-secondary"
                            onClick={handleClose}
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