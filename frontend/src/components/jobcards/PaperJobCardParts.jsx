
import { useState } from "react";
import { jwtDecode } from "jwt-decode";
import jobCardService from "../../services/jobCardService";


function PaperJobCardParts({ jobCard, onSaved, role }) {

    //--------------------------------------------------
    // State
    //--------------------------------------------------

    const [technicianId, setTechnicianId] = useState("");
    const [partName, setPartName] = useState("");
    const [quantity, setQuantity] = useState("");
    const [saving, setSaving] = useState(false);

    //--------------------------------------------------
    // Permissions
    //--------------------------------------------------

    const isCompleted =
        jobCard.status?.toLowerCase() === "completed";

    const canAddPart =
        role === "Admin" ||
        (role === "Technician" && !isCompleted);

    //--------------------------------------------------
    // Assigned Technicians
    //--------------------------------------------------

    const assignedTechnicians =
        jobCard.assignedTechnicians || [];

//--------------------------------------------------
// Restrict responsible technician selection
//--------------------------------------------------

const token = localStorage.getItem("token");

let currentUserId = null;

try {
    if (token) {
        const decoded = jwtDecode(token);

        const id =
            decoded.nameid ||
            decoded["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"] ||
            decoded.sub;

        if (id != null && id !== "" && Number.isInteger(Number(id))) {
            currentUserId = Number(id);
        }
    }
} catch (error) {
    console.error("Unable to read current user ID:", error);
}

const currentAssignment = assignedTechnicians.find(
    (technician) =>
        Number(technician.technicianId) === currentUserId
);

const isLeadTechnician =
    currentAssignment?.isLeadTechnician === true;

const selectableTechnicians =
    role === "Admin" || isLeadTechnician
        ? assignedTechnicians
        : assignedTechnicians.filter(
              (technician) =>
                  Number(technician.technicianId) === currentUserId
          );
        
    //--------------------------------------------------
    // Save Paper Job Card Part
    //--------------------------------------------------

    const handleAddPart = async () => {

        if (!canAddPart || saving) return;

        if (!technicianId) {
            alert("Please select the responsible technician.");
            return;
        }

        if (!partName.trim()) {
            alert("Please enter the part name.");
            return;
        }

        const parsedQuantity = Number(quantity);

        if (
            !Number.isInteger(parsedQuantity) ||
            parsedQuantity < 1 ||
            parsedQuantity > 9999
        ) {
            alert("Quantity must be between 1 and 9999.");
            return;
        }

        try {
            setSaving(true);

            await jobCardService.addPaperPart(
                jobCard.jobCardId,
                {
                    responsibleTechnicianId: Number(technicianId),
                    partName: partName.trim(),
                    quantity: parsedQuantity
                }
            );

            setTechnicianId("");
            setPartName("");
            setQuantity("");

            await onSaved();

            alert("Paper Job Card part added successfully.");

        } catch (error) {
            console.error(error);

            alert(
                error.response?.data?.message ||
                "Unable to add Paper Job Card part."
            );

        } finally {
            setSaving(false);
        }
    };

    //--------------------------------------------------
    // Render
    //--------------------------------------------------

    return (
        <div className="card shadow mt-4">

            <div className="card-header bg-primary text-white">
                <h5 className="mb-0">
                    Paper Job Card — Parts Used
                </h5>
            </div>

            <div className="card-body">

                <p className="text-muted">
                    Record parts used and the assigned
                    technician responsible.
                </p>

                {canAddPart ? (
                    <>
                        <div className="mb-3">
                            <label className="form-label">
                                Responsible Technician
                            </label>

                            <select
                                className="form-select"
                                value={technicianId}
                                onChange={(e) =>
                                    setTechnicianId(e.target.value)
                                }
                            >
                                <option value="">
                                    Select Technician
                                </option>

                             {selectableTechnicians.map((technician) => (
    <option
        key={technician.technicianId}
        value={technician.technicianId}
    >
        {technician.technicianName}
        {technician.isLeadTechnician ? " (Lead)" : ""}
    </option>
))}


                            </select>


{/* Existing Paper Parts */}

                        </div>

                        <div className="mb-3">
                            <label className="form-label">
                                Part Name
                            </label>

                            <input
                                type="text"
                                className="form-control"
                                value={partName}
                                onChange={(e) =>
                                    setPartName(e.target.value)
                                }
                                placeholder="Enter part name"
                            />
                        </div>

                        <div className="mb-3">
                            <label className="form-label">
                                Quantity
                            </label>

                            <input
                                type="number"
                                min="1"
                                className="form-control"
                                value={quantity}
                                onChange={(e) =>
                                    setQuantity(e.target.value)
                                }
                                placeholder="Enter quantity"
                            />
                        </div>

                       
<button
    type="button"
    className="btn btn-success"
    onClick={handleAddPart}
    disabled={saving}
>
    {saving ? "Saving..." : "Add Part"}
</button>

                    </>
               
                ) : (
                    <div className="alert alert-secondary mb-0">
                        Parts cannot be added with your current
                        permissions or after the Job Card
                        has been completed.
                    </div>
                )}

                {/* Existing Paper Parts */}

                <hr className="my-4" />

                <h5 className="mb-3">
                    Parts Used History
                </h5>

                <div className="table-responsive">
                    <table className="table table-striped table-bordered align-middle">
                        <thead className="table-light">
                            <tr>
                                <th>Part Name</th>
                                <th>Quantity</th>
                                <th>Responsible Technician</th>
                                <th>Captured By</th>
                                <th>Date Added</th>
                            </tr>
                        </thead>

                        <tbody>
                            {(jobCard.parts || [])
                                .filter(part => part.isPaperPartEntry === true)
                                .length === 0 ? (
                                <tr>
                                    <td
                                        colSpan={5}
                                        className="text-center text-muted py-4"
                                    >
                                        No Paper Job Card parts recorded yet.
                                    </td>
                                </tr>
                            ) : (
                                (jobCard.parts || [])
                                    .filter(part => part.isPaperPartEntry === true)
                                    .map(part => (
                                        <tr key={part.partId}>
                                            <td>{part.partName}</td>
                                            <td>{part.quantity}</td>
                                            <td>{part.responsibleTechnicianName || "-"}</td>
                                            <td>{part.capturedByUserName || "-"}</td>
                                            <td>
                                                {part.dateAdded
                                                    ? new Date(part.dateAdded).toLocaleString()
                                                    : "-"}
                                            </td>
                                        </tr>
                                    ))
                            )}
                        </tbody>
                    </table>
                </div>

            </div>
        </div>
    );
}


export default PaperJobCardParts;
