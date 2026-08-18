import { useState } from "react";

function JobCardParts({
    parts = [],
    onAdd,
    onUpdate,
    onDelete,
    role,
    status
}) {
    const [partName, setPartName] = useState("");
    const [quantity, setQuantity] = useState("");
    const [saving, setSaving] = useState(false);

    // Edit State
    const [editingId, setEditingId] = useState(null);
    const [editName, setEditName] = useState("");
    const [editQty, setEditQty] = useState("");

    const technicianReadOnly = role === "Technician" && status === "Completed";

    const startEdit = (part) => {
        setEditingId(part.partId);
        setEditName(part.partName);
        setEditQty(part.quantity);
    };

    const cancelEdit = () => {
        setEditingId(null);
        setEditName("");
        setEditQty("");
    };

    const handleUpdate = async (partId) => {
        if (!editName.trim() || !editQty || parseInt(editQty, 10) <= 0) {
            alert("Please provide a valid part name and quantity.");
            return;
        }

        try {
            setSaving(true);
            await onUpdate(partId, {
                partName: editName.trim(),
                quantity: parseInt(editQty, 10)
            });
            cancelEdit();
        } finally {
            setSaving(false);
        }
    };

    return (
        <div className="card shadow mt-4">
            <div className="card-header bg-secondary text-white">
                <h5 className="mb-0">Parts Used</h5>
            </div>

            <div className="card-body">
                <table className="table table-striped table-bordered align-middle">
                    <thead className="table-light">
                        <tr>
                            <th>Part Name</th>
                            <th style={{ width: "150px" }}>Quantity</th>
                            {!technicianReadOnly && <th style={{ width: "150px" }}>Actions</th>}
                        </tr>
                    </thead>
                    <tbody>
                        {parts.length === 0 ? (
                            <tr>
                                <td colSpan={technicianReadOnly ? 2 : 3} className="text-center text-muted py-4">
                                    No parts have been added.
                                </td>
                            </tr>
                        ) : (
                            parts.map((part) => (
                                <tr key={part.partId}>
                                    {editingId === part.partId ? (
                                        <>
                                            <td>
                                                <input
                                                    className="form-control form-control-sm"
                                                    value={editName}
                                                    onChange={(e) => setEditName(e.target.value)}
                                                />
                                            </td>
                                            <td>
                                                <input
                                                    type="number"
                                                    min="1"
                                                    className="form-control form-control-sm"
                                                    value={editQty}
                                                    onChange={(e) => setEditQty(e.target.value)}
                                                />
                                            </td>
                                            <td>
                                                <button
                                                    className="btn btn-sm btn-success me-1"
                                                    disabled={saving}
                                                    onClick={() => handleUpdate(part.partId)}
                                                >
                                                    Save
                                                </button>
                                                <button
                                                    className="btn btn-sm btn-secondary"
                                                    disabled={saving}
                                                    onClick={cancelEdit}
                                                >
                                                    Cancel
                                                </button>
                                            </td>
                                        </>
                                    ) : (
                                        <>
                                            <td>{part.partName}</td>
                                            <td>{part.quantity}</td>
                                            {!technicianReadOnly && (
                                                <td>
                                                    <button
                                                        className="btn btn-primary btn-sm me-1"
                                                        disabled={saving}
                                                        onClick={() => startEdit(part)}
                                                    >
                                                        Edit
                                                    </button>
                                                    <button
                                                        className="btn btn-danger btn-sm"
                                                        disabled={saving}
                                                        onClick={async () => {
                                                            if (!window.confirm("Delete this part?")) return;
                                                            try {
                                                                await onDelete(part.partId);
                                                            } catch {
                                                                alert("Failed to delete part.");
                                                            }
                                                        }}
                                                    >
                                                        Delete
                                                    </button>
                                                </td>
                                            )}
                                        </>
                                    )}
                                </tr>
                            ))
                        )}
                    </tbody>
                </table>
            </div>
        </div>
    );
}

export default JobCardParts;