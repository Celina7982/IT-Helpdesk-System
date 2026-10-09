import { useEffect, useRef, useState } from "react";
import { useNavigate } from "react-router-dom";
import { jwtDecode } from "jwt-decode";
import ticketService from "../../services/ticketService";
import userService from "../../services/userService";
import jobCardService from "../../services/jobCardService";
import slaTicketService from "../../services/slaTicketService";

const getErrorMessage = (error, fallback) => {
    const data = error?.response?.data;
    return typeof data === "string"
        ? data
        : data?.message || data?.detail || data?.title || fallback;
};

const formatDate = (value) => {
    if (!value) return "—";
    const date = new Date(value);
    return Number.isNaN(date.getTime()) ? "—" : date.toLocaleString();
};

function TicketDetailsModal({ show, onClose, ticketId }) {
    const navigate = useNavigate();
    const requestVersion = useRef(0);
    const activeTicketId = useRef(ticketId);
    activeTicketId.current = ticketId;

    let loggedInUserId = null;
    let loggedInUserRole = "";
    const token = localStorage.getItem("token");

    if (token) {
        try {
            const decoded = jwtDecode(token);
            const id = decoded.nameid ||
                decoded["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"] ||
                decoded.sub;
            loggedInUserId = id == null ? null : Number(id);
            const role = decoded.role ||
                decoded["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] || "";
            loggedInUserRole = Array.isArray(role) ? role[0] : role;
        } catch (error) {
            console.error("Unable to decode login token:", error);
        }
    }

    const isAdmin = loggedInUserRole === "Admin";
    const isStaff = isAdmin || loggedInUserRole === "Technician";
    const basePath = isAdmin ? "/admin" : "/technician";

    const [ticket, setTicket] = useState(null);
    const [jobCard, setJobCard] = useState(null);
    const [slaReport, setSlaReport] = useState(null);
    const [comments, setComments] = useState([]);
    const [newComment, setNewComment] = useState("");
    const [isInternalNote, setIsInternalNote] = useState(false);
    const [loading, setLoading] = useState(false);
    const [loadError, setLoadError] = useState("");
    const [submittingComment, setSubmittingComment] = useState(false);
    const [editingCommentId, setEditingCommentId] = useState(null);
    const [editingMessage, setEditingMessage] = useState("");
    const [updatingComment, setUpdatingComment] = useState(false);

    const [ticketAssignees, setTicketAssignees] = useState([]);
    const [loadingAssignees, setLoadingAssignees] = useState(false);
    const [assigneesError, setAssigneesError] = useState("");
    const [availableAssignees, setAvailableAssignees] = useState([]);
    const [selectedAssigneeId, setSelectedAssigneeId] = useState("");
    const [savingAssignee, setSavingAssignee] = useState(false);
    const [removingAssigneeId, setRemovingAssigneeId] = useState(null);
    const [assigneeActionError, setAssigneeActionError] = useState("");
    const [assigneeActionSuccess, setAssigneeActionSuccess] = useState("");
    const [assignableUsersError, setAssignableUsersError] = useState("");
    const [serviceActionLoading, setServiceActionLoading] = useState(false);

    useEffect(() => {
        const version = ++requestVersion.current;
        if (!show || !ticketId) return;
        const current = () => version === requestVersion.current;

        const load = async () => {
            setLoading(true);
            setLoadError("");
            setTicket(null);
            setComments([]);
            setJobCard(null);
            setSlaReport(null);
            setTicketAssignees([]);
            setAvailableAssignees([]);
            setSelectedAssigneeId("");
            setAssigneesError("");
            setAssignableUsersError("");
            setAssigneeActionError("");
            setAssigneeActionSuccess("");
            setLoadingAssignees(isStaff);
            setNewComment("");
            setIsInternalNote(false);
            setEditingCommentId(null);
            setEditingMessage("");

            try {
                const [ticketData, commentsData] = await Promise.all([
                    ticketService.getTicketDetails(ticketId),
                    ticketService.getComments(ticketId)
                ]);
                if (!current()) return;
                setTicket(ticketData);
                setComments(Array.isArray(commentsData) ? commentsData : []);
            } catch (error) {
                if (current()) {
                    console.error("Failed to load ticket details:", error);
                    setLoadError(getErrorMessage(error, "Unable to load ticket details or comments."));
                    setLoadingAssignees(false);
                    setLoading(false);
                }
                return;
            }

            // Independent requests: one failure must not prevent the others.
            const tasks = [];

            if (isStaff) {
                tasks.push(
                    ticketService.getTicketAssignees(ticketId)
                        .then((data) => {
                            if (current()) setTicketAssignees(Array.isArray(data) ? data : []);
                        })
                        .catch((error) => {
                            console.error("Failed to load ticket assignees:", error);
                            if (current()) setAssigneesError("Unable to load assigned users.");
                        })
                        .finally(() => {
                            if (current()) setLoadingAssignees(false);
                        })
                );
            }

            if (isAdmin) {
                tasks.push(
                    userService.getAssignableUsers()
                        .then((data) => {
                            if (current()) setAvailableAssignees(Array.isArray(data) ? data : []);
                        })
                        .catch((error) => {
                            console.error("Failed to load assignable users:", error);
                            if (current()) setAssignableUsersError("Unable to load users available for assignment.");
                        })
                );
            }

            if (isStaff) {
                tasks.push(
                    jobCardService.getByTicketId(ticketId)
                        .then((data) => { if (current()) setJobCard(data); })
                        .catch((error) => {
                            if (error?.response?.status !== 404) {
                                console.error("Failed to load Job Card:", error);
                            }
                            if (current()) setJobCard(null);
                        })
                );
                tasks.push(
                    slaTicketService.getByTicketId(ticketId)
                        .then((data) => { if (current()) setSlaReport(data); })
                        .catch((error) => {
                            if (error?.response?.status !== 404) {
                                console.error("Failed to load SLA Report:", error);
                            }
                            if (current()) setSlaReport(null);
                        })
                );
            }

            await Promise.all(tasks);
            if (current()) setLoading(false);
        };

        load();
        return () => { requestVersion.current += 1; };
    }, [show, ticketId, isAdmin, isStaff]);

    const refreshAssignments = async (expectedTicketId) => {
        const [assignees, details] = await Promise.all([
            ticketService.getTicketAssignees(expectedTicketId),
            ticketService.getTicketDetails(expectedTicketId)
        ]);
        if (activeTicketId.current !== expectedTicketId) return;
        setTicketAssignees(Array.isArray(assignees) ? assignees : []);
        setTicket(details);
        setAssigneesError("");
    };

    const handleAddAssignee = async () => {
        if (!isAdmin || !selectedAssigneeId || savingAssignee || removingAssigneeId !== null) return;
        const currentId = ticketId;
        setSavingAssignee(true);
        setAssigneeActionError("");
        setAssigneeActionSuccess("");
        try {
            await ticketService.addTicketAssignee(currentId, Number(selectedAssigneeId));
            await refreshAssignments(currentId);
            if (activeTicketId.current === currentId) {
                setSelectedAssigneeId("");
                setAssigneeActionSuccess("User assigned successfully.");
            }
        } catch (error) {
            console.error("Failed to add ticket assignee:", error);
            if (activeTicketId.current === currentId) {
                setAssigneeActionError(getErrorMessage(error, "Unable to assign this user."));
            }
        } finally {
            setSavingAssignee(false);
        }
    };

    const handleRemoveAssignee = async (userId) => {
        if (!isAdmin || savingAssignee || removingAssigneeId !== null) return;
        if (!window.confirm("Remove this user from the ticket?")) return;
        const currentId = ticketId;
        setRemovingAssigneeId(userId);
        setAssigneeActionError("");
        setAssigneeActionSuccess("");
        try {
            await ticketService.removeTicketAssignee(currentId, userId);
            await refreshAssignments(currentId);
            if (activeTicketId.current === currentId) {
                setAssigneeActionSuccess("User removed successfully.");
            }
        } catch (error) {
            console.error("Failed to remove ticket assignee:", error);
            if (activeTicketId.current === currentId) {
                setAssigneeActionError(getErrorMessage(error, "Unable to remove this user."));
            }
        } finally {
            setRemovingAssigneeId(null);
        }
    };

    const handleCreateJobCard = async () => {
        if (!isStaff || serviceActionLoading) return;
        setServiceActionLoading(true);
        try {
            const created = await jobCardService.createFromTicket(ticketId);
            if (!created?.jobCardId) throw new Error("Job Card ID missing from response.");
            setJobCard(created);
            onClose();
            navigate(`${basePath}/jobcards/${created.jobCardId}`);
        } catch (error) {
            console.error("Failed to create Job Card:", error);
            alert(getErrorMessage(error, "Unable to create Job Card."));
        } finally {
            setServiceActionLoading(false);
        }
    };

    const handleViewJobCard = () => {
        if (!isStaff || !jobCard?.jobCardId) return;
        onClose();
        navigate(`${basePath}/jobcards/${jobCard.jobCardId}`);
    };

    const handleCreateSlaReport = async () => {
        if (!isStaff || serviceActionLoading) return;
        setServiceActionLoading(true);
        try {
            const created = await slaTicketService.createFromTicket(ticketId);
            if (!created?.slaTicketId) throw new Error("SLA ID missing from response.");
            setSlaReport(created);
            onClose();
            navigate(`${basePath}/sla/${created.slaTicketId}`);
        } catch (error) {
            console.error("Failed to create SLA Report:", error);
            alert(getErrorMessage(error, "Unable to create SLA Report."));
        } finally {
            setServiceActionLoading(false);
        }
    };

    const handleViewSlaReport = () => {
        if (!isStaff || !slaReport?.slaTicketId) return;
        onClose();
        navigate(`${basePath}/sla/${slaReport.slaTicketId}`);
    };

    const handleAddComment = async (event) => {
        event.preventDefault();
        if (!newComment.trim() || submittingComment) return;
        const currentId = ticketId;
        setSubmittingComment(true);
        try {
            const created = await ticketService.addComment(
                currentId, newComment.trim(), isStaff && isInternalNote
            );
            if (activeTicketId.current !== currentId) return;
            if (created && typeof created === "object" && !Array.isArray(created)) {
                setComments((prev) => [...prev, created]);
            } else {
                const refreshed = await ticketService.getComments(currentId);
                if (activeTicketId.current === currentId) {
                    setComments(Array.isArray(refreshed) ? refreshed : []);
                }
            }
            setNewComment("");
            setIsInternalNote(false);
        } catch (error) {
            console.error("Failed to submit comment:", error);
            alert(getErrorMessage(error, "Failed to submit comment."));
        } finally {
            setSubmittingComment(false);
        }
    };

    const handleStartEdit = (comment) => {
        setEditingCommentId(comment.commentId ?? comment.id);
        setEditingMessage(comment.message ?? comment.commentText ?? comment.text ?? "");
    };

    const handleCancelEdit = () => {
        setEditingCommentId(null);
        setEditingMessage("");
    };

    const handleUpdateComment = async (commentId) => {
        if (!editingMessage.trim() || updatingComment) return;
        const currentId = ticketId;
        setUpdatingComment(true);
        try {
            const updated = await ticketService.updateComment(currentId, commentId, editingMessage.trim());
            if (activeTicketId.current !== currentId) return;
            if (updated && typeof updated === "object" && !Array.isArray(updated)) {
                setComments((prev) => prev.map((comment) =>
                    String(comment.commentId ?? comment.id) === String(commentId)
                        ? { ...comment, ...updated }
                        : comment
                ));
            } else {
                const refreshed = await ticketService.getComments(currentId);
                if (activeTicketId.current === currentId) {
                    setComments(Array.isArray(refreshed) ? refreshed : []);
                }
            }
            handleCancelEdit();
        } catch (error) {
            console.error("Failed to update comment:", error);
            alert(getErrorMessage(error, "Unable to update comment."));
        } finally {
            setUpdatingComment(false);
        }
    };

    const handleClose = () => {
        requestVersion.current += 1;
        setEditingCommentId(null);
        setEditingMessage("");
        setNewComment("");
        setIsInternalNote(false);
        onClose();
    };

    if (!show) return null;

    const assignedIds = new Set(ticketAssignees.map((item) => String(item.userId)));
    const selectableUsers = availableAssignees.filter((item) => !assignedIds.has(String(item.userId)));
    const primaryName = ticket?.assignedTechnician?.trim();
    const primaryMatches = ticketAssignees.filter((item) => item.fullName?.trim() === primaryName);
    const primaryUserId = primaryName && primaryMatches.length === 1 ? primaryMatches[0].userId : null;
    const assignmentBusy = savingAssignee || removingAssigneeId !== null;

    const detailRows = ticket ? [
        ["Subject", ticket.subject],
        ["Description", ticket.description],
        ["Customer", ticket.customerName],
        ["Company", ticket.companyName],
        ["Category", ticket.category],
        ["Priority", ticket.priority],
        ["Status", ticket.status],
        ["Assigned Technician", ticket.assignedTechnician || "Unassigned"],
        ["Created", formatDate(ticket.createdDate)],
        ["Escalated", ticket.isEscalated ? "Yes" : "No"],
        ...(ticket.isEscalated ? [["Escalation Reason", ticket.escalationReason]] : [])
    ] : [];

    return (
        <div className="modal fade show" style={{ display: "block", backgroundColor: "rgba(0,0,0,0.5)" }} role="dialog" aria-modal="true" aria-label={`Ticket ${ticketId} details`}>
            <div className="modal-dialog modal-lg modal-dialog-scrollable">
                <div className="modal-content">
                    <div className="modal-header">
                        <h5 className="modal-title">Ticket #{ticket?.ticketId || ticketId}</h5>
                        <button type="button" className="btn-close" aria-label="Close" onClick={handleClose} />
                    </div>

                    <div className="modal-body">
                        {loading ? (
                            <div className="text-center py-4" role="status">
                                <div className="spinner-border text-primary" />
                                <p className="mt-2 mb-0">Loading ticket details...</p>
                            </div>
                        ) : loadError ? (
                            <div className="alert alert-danger" role="alert">{loadError}</div>
                        ) : !ticket ? (
                            <div className="alert alert-secondary">No ticket details available.</div>
                        ) : (
                            <>
                                <h6 className="fw-semibold mb-3">Ticket Information</h6>
                                <div className="table-responsive">
                                    <table className="table table-sm align-middle">
                                        <tbody>
                                            {detailRows.map(([label, value]) => (
                                                <tr key={label}>
                                                    <th scope="row" style={{ width: "35%" }}>{label}</th>
                                                    <td style={{ overflowWrap: "anywhere" }}>{value || "—"}</td>
                                                </tr>
                                            ))}
                                            {isStaff && (
                                                <tr>
                                                    <th scope="row">Assigned Users</th>
                                                    <td>
                                                        {loadingAssignees ? (
                                                            <span className="text-muted">Loading assigned users...</span>
                                                        ) : assigneesError ? (
                                                            <span className="text-danger">{assigneesError}</span>
                                                        ) : ticketAssignees.length === 0 ? (
                                                            <span className="text-muted">No users assigned.</span>
                                                        ) : (
                                                            <div className="d-flex flex-column gap-2">
                                                                {ticketAssignees.map((assignee) => (
                                                                    <div key={assignee.userId} className="d-flex flex-wrap align-items-center gap-2">
                                                                        <span>{assignee.fullName}</span>
                                                                        <span className="badge bg-secondary">{assignee.role}</span>
                                                                        {primaryUserId != null && String(assignee.userId) === String(primaryUserId) ? (
                                                                            <span className="badge bg-success">Primary</span>
                                                                        ) : primaryUserId != null ? (
                                                                            <span className="badge bg-light text-dark border">Additional</span>
                                                                        ) : null}
                                                                        {isAdmin && (
                                                                            <button
                                                                                type="button"
                                                                                className="btn btn-outline-danger btn-sm"
                                                                                onClick={() => handleRemoveAssignee(assignee.userId)}
                                                                                disabled={assignmentBusy}
                                                                                aria-label={`Remove ${assignee.fullName}`}
                                                                            >
                                                                                {removingAssigneeId === assignee.userId ? "Removing..." : "Remove"}
                                                                            </button>
                                                                        )}
                                                                    </div>
                                                                ))}
                                                            </div>
                                                        )}
                                                    </td>
                                                </tr>
                                            )}
                                        </tbody>
                                    </table>
                                </div>

                                {isAdmin && (
                                    <section className="border rounded p-3 mt-3" aria-label="Manage ticket assignees">
                                        <h6 className="fw-semibold mb-2">Assign Another User</h6>
                                        <p className="text-muted small mb-3">Select an active Admin or Technician who is not already assigned.</p>
                                        {assignableUsersError && <div className="alert alert-warning py-2" role="alert">{assignableUsersError}</div>}
                                        {assigneeActionError && <div className="alert alert-danger py-2" role="alert">{assigneeActionError}</div>}
                                        {assigneeActionSuccess && <div className="alert alert-success py-2" role="status">{assigneeActionSuccess}</div>}
                                        <div className="d-flex flex-wrap gap-2 align-items-center">
                                            <select
                                                className="form-select"
                                                style={{ flex: "1 1 230px" }}
                                                aria-label="Choose user to assign"
                                                value={selectedAssigneeId}
                                                onChange={(event) => setSelectedAssigneeId(event.target.value)}
                                                disabled={assignmentBusy || !!assignableUsersError || !!assigneesError || loadingAssignees}
                                            >
                                                <option value="">Select a user...</option>
                                                {selectableUsers.map((user) => (
                                                    <option key={user.userId} value={user.userId}>
                                                        {user.fullName} ({user.role})
                                                    </option>
                                                ))}
                                            </select>
                                            <button
                                                type="button"
                                                className="btn btn-primary"
                                                onClick={handleAddAssignee}
                                                disabled={!selectedAssigneeId || assignmentBusy || !!assigneesError || !!assignableUsersError || loadingAssignees}
                                            >
                                                {savingAssignee ? "Assigning..." : "Add User"}
                                            </button>
                                        </div>
                                        {!assignableUsersError && selectableUsers.length === 0 && (
                                            <small className="text-muted d-block mt-2">No additional eligible users available.</small>
                                        )}
                                    </section>
                                )}

                                {isStaff && ticket.status === "Resolved" && (
                                    <section className="mt-4">
                                        <hr />
                                        <h6 className="fw-semibold mb-3">Service Report</h6>
                                        <div className="d-flex flex-wrap gap-2">
                                            {jobCard ? (
                                                <button type="button" className="btn btn-primary" onClick={handleViewJobCard}>View Job Card</button>
                                            ) : !slaReport ? (
                                                <button type="button" className="btn btn-primary" onClick={handleCreateJobCard} disabled={serviceActionLoading}>
                                                    {serviceActionLoading ? "Please wait..." : "Create Job Card"}
                                                </button>
                                            ) : null}
                                            {slaReport ? (
                                                <button type="button" className="btn btn-outline-primary" onClick={handleViewSlaReport}>View SLA</button>
                                            ) : !jobCard ? (
                                                <button type="button" className="btn btn-outline-primary" onClick={handleCreateSlaReport} disabled={serviceActionLoading}>
                                                    {serviceActionLoading ? "Please wait..." : "SLA"}
                                                </button>
                                            ) : null}
                                        </div>
                                    </section>
                                )}

                                <section className="mt-4">
                                    <hr />
                                    <h6 className="fw-semibold mb-3">Comments</h6>
                                    <div className="border rounded p-3 mb-3 bg-light" style={{ maxHeight: 300, overflowY: "auto" }}>
                                        {comments.length === 0 ? (
                                            <p className="text-muted mb-0 small">No comments posted yet.</p>
                                        ) : comments.map((item, index) => {
                                            const commentId = item.commentId ?? item.id;
                                            const message = item.message ?? item.commentText ?? item.text ?? "";
                                            const canEdit = isAdmin || (loggedInUserId != null && String(item.authorUserId) === String(loggedInUserId));
                                            const editing = commentId != null && String(editingCommentId) === String(commentId);
                                            return (
                                                <div key={commentId ?? index} className="card mb-2 shadow-sm">
                                                    <div className="card-body p-3">
                                                        <div className="d-flex justify-content-between align-items-start flex-wrap gap-2">
                                                            <div className="d-flex flex-wrap align-items-center gap-2">
                                                                <strong className="small">{item.authorName || "Unknown user"}</strong>
                                                                {item.isInternal && <span className="badge bg-warning text-dark">Internal Note</span>}
                                                            </div>
                                                            <small className="text-muted">{formatDate(item.createdDate ?? item.createdAt)}</small>
                                                        </div>
                                                        {editing ? (
                                                            <div className="mt-2">
                                                                <textarea
                                                                    className="form-control mb-2"
                                                                    rows={3}
                                                                    value={editingMessage}
                                                                    onChange={(event) => setEditingMessage(event.target.value)}
                                                                />
                                                                <div className="d-flex gap-2">
                                                                    <button type="button" className="btn btn-success btn-sm" onClick={() => handleUpdateComment(commentId)} disabled={updatingComment || !editingMessage.trim()}>
                                                                        {updatingComment ? "Saving..." : "Save"}
                                                                    </button>
                                                                    <button type="button" className="btn btn-secondary btn-sm" onClick={handleCancelEdit} disabled={updatingComment}>Cancel</button>
                                                                </div>
                                                            </div>
                                                        ) : (
                                                            <>
                                                                <p className="small mt-2 mb-2" style={{ whiteSpace: "pre-wrap", overflowWrap: "anywhere" }}>{message}</p>
                                                                {canEdit && commentId != null && (
                                                                    <button type="button" className="btn btn-outline-primary btn-sm" onClick={() => handleStartEdit(item)}>Edit</button>
                                                                )}
                                                            </>
                                                        )}
                                                    </div>
                                                </div>
                                            );
                                        })}
                                    </div>
                                    <form onSubmit={handleAddComment}>
                                        <textarea
                                            className="form-control"
                                            rows={3}
                                            placeholder={isStaff && isInternalNote ? "Write an internal note..." : "Write a comment..."}
                                            value={newComment}
                                            onChange={(event) => setNewComment(event.target.value)}
                                            required
                                        />
                                        {isStaff && (
                                            <div className="form-check mt-2">
                                                <input
                                                    className="form-check-input"
                                                    type="checkbox"
                                                    id="ticket-details-internal-note"
                                                    checked={isInternalNote}
                                                    onChange={(event) => setIsInternalNote(event.target.checked)}
                                                />
                                                <label className="form-check-label" htmlFor="ticket-details-internal-note">Internal Note</label>
                                            </div>
                                        )}
                                        <button type="submit" className="btn btn-primary btn-sm mt-3" disabled={submittingComment || !newComment.trim()}>
                                            {submittingComment ? "Posting..." : isStaff && isInternalNote ? "Post Internal Note" : "Post Comment"}
                                        </button>
                                    </form>
                                </section>
                            </>
                        )}
                    </div>
                    <div className="modal-footer">
                        <button type="button" className="btn btn-secondary" onClick={handleClose}>Close</button>
                    </div>
                </div>
            </div>
        </div>
    );
}

export default TicketDetailsModal;
