
import { useState } from "react";
import { jwtDecode } from "jwt-decode";
import jobCardService from "../../services/jobCardService";

function PaperJobCardWork({ jobCard, onSaved, role }) {

    //--------------------------------------------------
    // State
    //--------------------------------------------------

    const [technicianId, setTechnicianId] = useState("");
    const [workPerformed, setWorkPerformed] = useState("");
    const [saving, setSaving] = useState(false);

    //--------------------------------------------------
    // Permissions
    //--------------------------------------------------

    
const isCompleted =
    jobCard.status?.toLowerCase() === "completed";

const canAddWork =
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

            if (
                id != null &&
                id !== "" &&
                Number.isInteger(Number(id))
            ) {
                currentUserId = Number(id);
            }
        }
    } catch (error) {
        console.error("Unable to read current user ID:", error);
    }

    const currentAssignment = assignedTechnicians.find(
        technician =>
            Number(technician.technicianId) === currentUserId
    );

    const isLeadTechnician =
        currentAssignment?.isLeadTechnician === true;

    const selectableTechnicians =
        role === "Admin" || isLeadTechnician
            ? assignedTechnicians
            : assignedTechnicians.filter(
                technician =>
                    Number(technician.technicianId) === currentUserId
            );


    //--------------------------------------------------
    // Save Work Completed
    //--------------------------------------------------

    const handleSave = async () => {

        if (!canAddWork || saving) return;

        if (!technicianId) {
            alert("Please select the responsible technician.");
            return;
        }

        if (!workPerformed.trim()) {
            alert("Please describe the work completed.");
            return;
        }

        try {

            setSaving(true);

            await jobCardService.addPaperWorkEntry(
                jobCard.jobCardId,
                {
                    technicianId: Number(technicianId),
                    workPerformed: workPerformed.trim()
                }
            );

            setTechnicianId("");
            setWorkPerformed("");

            await onSaved();

            alert("Work completed recorded successfully.");

        } catch (error) {

            console.error(error);

            alert(
                error.response?.data?.message ||
                "Unable to record work completed."
            );

        } finally {

            setSaving(false);

        }

    };

    //--------------------------------------------------
    // Render
    //--------------------------------------------------


    //--------------------------------------------------
    // Existing Paper Work Entries
    //--------------------------------------------------

    const workEntries = (
        jobCard.labourEntries ||
        jobCard.labours ||
        []
    ).filter(
        (entry) => entry.isPaperWorkEntry === true
    );


    return (

        <div className="card mt-4">

            <div className="card-header bg-primary text-white">
                <strong>Paper Job Card — Work Completed</strong>
            </div>

            <div className="card-body">

                <p className="text-muted">
                    Record the work completed and select the
                    assigned technician responsible.
                    Labour hours are not recorded here.
                </p>

                {canAddWork ? (

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
                                disabled={saving}
                            >

                                <option value="">
                                    Select Technician
                                </option>

                                {selectableTechnicians.map(
                                    (technician) => (

                                        <option
                                            key={technician.technicianId}
                                            value={technician.technicianId}
                                        >
                                            {technician.technicianName}
                                            {technician.isLeadTechnician
                                                ? " (Lead)"
                                                : ""}
                                        </option>

                                    )
                                )}

                            </select>

                        </div>

                        <div className="mb-3">

                            <label className="form-label">
                                Work Completed
                            </label>

                            <textarea
                                className="form-control"
                                rows={4}
                                placeholder="Describe the work completed..."
                                value={workPerformed}
                                onChange={(e) =>
                                    setWorkPerformed(e.target.value)
                                }
                                disabled={saving}
                            />

                        </div>

                        <button
                            type="button"
                            className="btn btn-success"
                            onClick={handleSave}
                            disabled={saving}
                        >
                            {saving
                                ? "Saving..."
                                : "Add Work Completed"}
                        </button>

                    </>

                ) : (

                    <div className="alert alert-secondary mb-0">
                        Work Completed entries cannot be added
                        with your current permissions or
                        after the Job Card has been completed.
                    </div>

                )}

            </div>

            
                {/* Existing Work Completed Entries */}

                <hr className="my-4" />

                <h5 className="mb-3">
                    Work Completed History
                </h5>

                <div className="table-responsive">

                    <table className="table table-striped table-bordered align-middle">

                        <thead className="table-light">

                            <tr>
                                <th>Work Completed</th>
                                <th>Responsible Technician</th>
                                <th>Captured By</th>
                                <th>Date Captured</th>
                            </tr>

                        </thead>

                        <tbody>

                            {workEntries.length === 0 ? (

                                <tr>
                                    <td
                                        colSpan={4}
                                        className="text-center text-muted py-4"
                                    >
                                        No work completed entries recorded yet.
                                    </td>
                                </tr>

                            ) : (

                                workEntries.map((entry) => (

                                    <tr key={entry.labourId}>

                                        <td>
                                            {entry.workPerformed}
                                        </td>

                                        <td>
                                            {entry.technicianName || "-"}
                                        </td>

                                        <td>
                                            {entry.capturedByUserName || "-"}
                                        </td>

                                        <td>
                                            {entry.dateWorked
                                                ? new Date(
                                                    entry.dateWorked
                                                ).toLocaleString()
                                                : "-"}
                                        </td>

                                    </tr>

                                ))

                            )}

                        </tbody>

                    </table>

                </div>


        </div>

    );

}

export default PaperJobCardWork;
