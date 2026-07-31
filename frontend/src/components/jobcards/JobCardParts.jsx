import { useState } from "react";

function JobCardParts({
    parts = [],
    onAdd,
    onDelete,
    role,
    status
}) {
    //--------------------------------------------------
    // State
    //--------------------------------------------------
    const [partName, setPartName] = useState("");
    const [quantity, setQuantity] = useState("");
    const [saving, setSaving] = useState(false);

    //--------------------------------------------------
    // Technician Read Only
    //--------------------------------------------------
    const technicianReadOnly =
        role === "Technician" &&
        status === "Completed";

    //--------------------------------------------------
    // Total Parts
    //--------------------------------------------------
    const totalParts = parts.reduce(
        (total, part) =>
            total + Number(part.quantity || 0),
        0
    );

    //--------------------------------------------------
    // Add Part
    //--------------------------------------------------
    const addPart = async () => {
        //--------------------------------------------------
        // Validation
        //--------------------------------------------------
        if (!partName.trim()) {
            alert("Please enter a part name.");
            return;
        }

        if (!quantity || parseInt(quantity, 10) <= 0) {
            alert("Quantity must be greater than zero.");
            return;
        }

        try {
            setSaving(true);

            await onAdd({
                partName: partName.trim(),
                quantity: parseInt(quantity, 10)
            });

            //--------------------------------------------------
            // Clear Form
            //--------------------------------------------------
            setPartName("");
            setQuantity("");
        }
        finally {
            setSaving(false);
        }
    };

    return (
        <div className="card shadow mt-4">
            <div className="card-header bg-secondary text-white">
                <h5 className="mb-0">Parts Used</h5>
            </div>

            <div className="card-body">
                {/*==========================================
                    Parts Table
                ==========================================*/}
                <table className="table table-striped table-bordered">
                    <thead className="table-light">
                        <tr>
                            <th>Part Name</th>
                            <th style={{ width: "150px" }}>Quantity</th>
                            {
                                !technicianReadOnly &&
                                <th style={{ width: "120px" }}>Actions</th>
                            }
                        </tr>
                    </thead>
                    <tbody>
                        {
                            parts.length === 0 ?
                                <tr>
                                    <td
                                        colSpan={technicianReadOnly ? 2 : 3}
                                        className="text-center text-muted py-4"
                                    >
                                        No parts have been added.
                                    </td>
                                </tr>
                                :
                                parts.map((part) => (
                                    <tr key={part.partId}>
                                        <td>{part.partName}</td>
                                        <td>{part.quantity}</td>
                                        {
                                            !technicianReadOnly &&
                                            <td>
                                                <button
                                                    className="btn btn-danger btn-sm"
                                                    disabled={saving}
                                                    onClick={async () => {
                                                        if (!window.confirm("Delete this part?")) return;
                                                        try {
                                                            await onDelete(part.partId);
                                                        }
                                                        catch {
                                                            alert("Failed to delete part.");
                                                        }
                                                    }}
                                                >
                                                    Delete
                                                </button>
                                            </td>
                                        }
                                    </tr>
                                ))
                        }
                    </tbody>
                </table>

                {/*==========================================
                    Total Parts
                ==========================================*/}
                <div className="alert alert-info">
                    <strong>Total Quantity Used:</strong> {totalParts}
                </div>

                {/*==========================================
                    Add Part Form
                ==========================================*/}
                {
                    !technicianReadOnly && (
                        <>
                            <hr />
                            <h6 className="mb-3">Add Part</h6>
                            <div className="row g-2">
                                <div className="col-md-7">
                                    <input
                                        className="form-control"
                                        placeholder="Part Name"
                                        value={partName}
                                        onChange={(e) => setPartName(e.target.value)}
                                        disabled={saving}
                                    />
                                </div>
                                <div className="col-md-2">
                                    <input
                                        type="number"
                                        min="1"
                                        className="form-control"
                                        placeholder="Qty"
                                        value={quantity}
                                        onChange={(e) => setQuantity(e.target.value)}
                                        disabled={saving}
                                    />
                                </div>
                                <div className="col-md-3 d-grid">
                                    <button
                                        className="btn btn-success"
                                        onClick={addPart}
                                        disabled={saving}
                                    >
                                        {saving ? "Saving..." : "Add Part"}
                                    </button>
                                </div>
                            </div>
                        </>
                    )
                }

                {/*==========================================
                    Completed Job Card Message
                ==========================================*/}
                {
                    technicianReadOnly && (
                        <div className="alert alert-warning mt-3">
                            <strong>Job Card Completed</strong>
                            <br />
                            Parts can no longer be added or deleted.
                        </div>
                    )
                }
            </div>
        </div>
    );
}

export default JobCardParts;
