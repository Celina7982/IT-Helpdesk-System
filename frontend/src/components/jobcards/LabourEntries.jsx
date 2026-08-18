import { useState } from "react";

function LabourEntries({
    labourEntries = [],
    onAdd,
    onUpdate,
    onDelete,
    role,
    status
}) {
    const [hoursWorked, setHoursWorked] = useState("");
    const [workPerformed, setWorkPerformed] = useState("");
    const [saving, setSaving] = useState(false);

    // Edit State
    const [editingId, setEditingId] = useState(null);
    const [editHours, setEditHours] = useState("");
    const [editWork, setEditWork] = useState("");

    // Admin can edit even if completed; Technicians are read-only when completed
    const isReadOnly = role === "Technician" && status === "Completed";

    const startEdit = (entry) => {
        setEditingId(entry.labourId);
        setEditHours(entry.hoursWorked);
        setEditWork(entry.workPerformed);
    };

    const cancelEdit = () => {
        setEditingId(null);
        setEditHours("");
        setEditWork("");
    };

    const handleUpdate = async (labourId) => {
        if (!editHours || !editWork.trim()) {
            alert("Please complete all fields.");
            return;
        }

        try {
            setSaving(true);
            await onUpdate(labourId, {
                hoursWorked: parseFloat(editHours),
                workPerformed: editWork,
                dateWorked: new Date().toISOString()
            });
            cancelEdit();
        } finally {
            setSaving(false);
        }
    };

    const addEntry = async () => {
        if (!hoursWorked || !workPerformed.trim()) {
            alert("Please complete all fields.");
            return;
        }

        try {
            setSaving(true);
            await onAdd({
                hoursWorked: parseFloat(hoursWorked),
                workPerformed,
                dateWorked: new Date().toISOString()
            });
            setHoursWorked("");
            setWorkPerformed("");
        } finally {
            setSaving(false);
        }
    };

    const totalHours = labourEntries.reduce(
        (total, entry) => total + Number(entry.hoursWorked || 0),
        0
    );

    return (
        <div className="card shadow mt-4">
            <div className="card-header bg-secondary text-white">
                <h5 className="mb-0">Technician Labour Entries</h5>
            </div>

            <div className="card-body">
                {/*==========================================
                    Labour Table
                ==========================================*/}
                <table className="table table-striped table-bordered align-middle">
                    <thead className="table-light">
                        <tr>
                            <th style={{ width: "110px" }}>Date</th>
                            <th style={{ width: "160px" }}>User</th>
                            <th>Details (Work Performed)</th>
                            <th style={{ width: "90px" }}>Hours</th>
                            {!isReadOnly && <th style={{ width: "140px" }}>Actions</th>}
                        </tr>
                    </thead>
                    <tbody>
                        {labourEntries.length === 0 ? (
                            <tr>
                                <td
                                    colSpan={isReadOnly ? 4 : 5}
                                    className="text-center text-muted py-4"
                                >
                                    No labour entries have been added.
                                </td>
                            </tr>
                        ) : (
                            labourEntries.map((entry) => (
                                <tr key={entry.labourId}>
                                    {editingId === entry.labourId ? (
                                        <>
                                            <td>
                                                {new Date(entry.dateWorked).toLocaleDateString()}
                                            </td>
                                            <td>{entry.userName || entry.createdByName || "N/A"}</td>
                                            <td>
                                                <input
                                                    className="form-control form-control-sm"
                                                    value={editWork}
                                                    onChange={(e) => setEditWork(e.target.value)}
                                                />
                                            </td>
                                            <td>
                                                <input
                                                    type="number"
                                                    step="0.25"
                                                    className="form-control form-control-sm"
                                                    value={editHours}
                                                    onChange={(e) => setEditHours(e.target.value)}
                                                />
                                            </td>
                                            <td>
                                                <button
                                                    className="btn btn-sm btn-success me-1"
                                                    disabled={saving}
                                                    onClick={() => handleUpdate(entry.labourId)}
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
                                            <td>
                                                {new Date(entry.dateWorked).toLocaleDateString()}
                                            </td>
                                            <td>
                                                <strong>{entry.userName || entry.createdByName || "System"}</strong>
                                            </td>
                                            <td>{entry.workPerformed}</td>
                                            <td>{entry.hoursWorked}</td>
                                            {!isReadOnly && (
                                                <td>
                                                    <button
                                                        className="btn btn-primary btn-sm me-1"
                                                        disabled={saving}
                                                        onClick={() => startEdit(entry)}
                                                    >
                                                        Edit
                                                    </button>
                                                    <button
                                                        className="btn btn-danger btn-sm"
                                                        disabled={saving}
                                                        onClick={async () => {
                                                            if (!window.confirm("Delete this labour entry?")) return;
                                                            try {
                                                                await onDelete(entry.labourId);
                                                            } catch {
                                                                alert("Failed to delete entry.");
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

                {/*==========================================
                    Total Hours
                ==========================================*/}
                <div className="alert alert-info">
                    <strong>Total Labour Hours:</strong> {totalHours.toFixed(2)} hrs
                </div>

                {/*==========================================
                    Add Labour Form
                ==========================================*/}
                {!isReadOnly && (
                    <>
                        <hr />
                        <h6 className="mb-3">Add New Labour Entry</h6>
                        <div className="row g-2">
                            <div className="col-md-2">
                                <input
                                    type="number"
                                    className="form-control"
                                    min="0.25"
                                    step="0.25"
                                    placeholder="Hours"
                                    value={hoursWorked}
                                    onChange={(e) => setHoursWorked(e.target.value)}
                                />
                            </div>
                            <div className="col-md-8">
                                <input
                                    className="form-control"
                                    placeholder="Describe the work completed..."
                                    value={workPerformed}
                                    onChange={(e) => setWorkPerformed(e.target.value)}
                                />
                            </div>
                            <div className="col-md-2 d-grid">
                                <button
                                    className="btn btn-success"
                                    onClick={addEntry}
                                    disabled={saving}
                                >
                                    {saving ? "Saving..." : "Add Entry"}
                                </button>
                            </div>
                        </div>
                    </>
                )}
            </div>
        </div>
    );
}

export default LabourEntries;