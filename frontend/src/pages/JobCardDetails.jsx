import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";

import jobCardService from "../services/jobCardService";
import userService from "../services/userService";

import JobCardHeader from "../components/jobcards/JobCardHeader";
import JobCardInformation from "../components/jobcards/JobCardInformation";
import PaperJobCardTime from "../components/jobcards/PaperJobCardTime";
import JobCardWorkDetails from "../components/jobcards/JobCardWorkDetails";
import PaperJobCardWork from "../components/jobcards/PaperJobCardWork";
import LabourEntries from "../components/jobcards/LabourEntries";
import JobCardParts from "../components/jobcards/JobCardParts";
import PaperJobCardParts from "../components/jobcards/PaperJobCardParts";
import { jwtDecode } from "jwt-decode";
import JobCardHistory from "../components/jobcards/JobCardHistory";

function JobCardDetails() {
    const { id } = useParams();

    const [jobCard, setJobCard] = useState(null);
    const [loading, setLoading] = useState(true);
    const [activeTab, setActiveTab] = useState("information");


    
    //--------------------------------------------------
    // Paper Job Card - Technician Assignment Editor
    //--------------------------------------------------

    const [showAssignmentEditor, setShowAssignmentEditor] =
        useState(false);

    const [selectedAssignmentTechnicianId, setSelectedAssignmentTechnicianId] =
        useState("");

    const [savingAssignment, setSavingAssignment] =
        useState(false);
        
        const [assignableUsers, setAssignableUsers] = useState([]);
const [loadingAssignableUsers, setLoadingAssignableUsers] =
    useState(false);


    const token = localStorage.getItem("token");

    let role = "";
    // prevents the page from crashing if the JWT is invalid.
    try {
        if (token) {
            const decoded = jwtDecode(token);

            role =
                decoded.role ||
                decoded["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];
        }
    } catch {
        role = "";
    }

    //--------------------------------------------------
    // Load Job Card
    //--------------------------------------------------

    const loadJobCard = async () => {
        try {
            const data = await jobCardService.getDetails(id);
            setJobCard(data);
        } catch (error) {
            console.error(error);
            alert("Unable to load Job Card.");
        } finally {
            setLoading(false);
        }
    };

    
    //--------------------------------------------------
    // Load Users Available for Paper Job Card Assignment
    //--------------------------------------------------

    const loadAssignableUsers = async () => {
        try {
            setLoadingAssignableUsers(true);

            const users = await userService.getAssignableUsers();

            setAssignableUsers(users);

        } catch (error) {
            console.error(
                "Unable to load assignable users:",
                error
            );

            alert("Unable to load available technicians.");

        } finally {
            setLoadingAssignableUsers(false);
        }
    };


    
    //--------------------------------------------------
    // Add Technician to Paper Job Card
    //--------------------------------------------------

    const handleAddPaperTechnician = async () => {

        if (!selectedAssignmentTechnicianId) {
            alert("Please select a technician.");
            return;
        }

        try {
            setSavingAssignment(true);

            await jobCardService.assignPaperTechnician(
                Number(id),
                Number(selectedAssignmentTechnicianId),
                false
            );

            alert("Technician assigned successfully.");

            setSelectedAssignmentTechnicianId("");

            await loadJobCard();

        } catch (error) {

            console.error(
                "Failed to assign technician:",
                error
            );

            alert(
                error.response?.data?.message ||
                "Unable to assign technician."
            );

        } finally {
            setSavingAssignment(false);
        }
    };


    useEffect(() => {
        loadJobCard();
    }, [id]);

    
    useEffect(() => {
        if (showAssignmentEditor) {
            loadAssignableUsers();
        }
    }, [showAssignmentEditor]);


    //--------------------------------------------------
    // Save Changes
    //--------------------------------------------------

    const saveJobCard = async () => {
        try {
            await jobCardService.update(jobCard.jobCardId, jobCard);
            await loadJobCard();
            alert("Job Card Updated Successfully.");
        } catch (error) {
            console.error(error);
            alert("Unable to update Job Card.");
        }
    };

    //--------------------------------------------------
    // Add Labour Entry
    //--------------------------------------------------

    const addLabourEntry = async (entry) => {
        try {
            await jobCardService.addLabourEntry(jobCard.jobCardId, entry);
            await loadJobCard();
            alert("Labour entry added successfully.");
        } catch (error) {
            console.error(error);
            alert("Unable to add labour entry.");
        }
    };

    //--------------------------------------------------
    // Add Part
    //--------------------------------------------------

    const addPart = async (part) => {
        try {
            await jobCardService.addPart(jobCard.jobCardId, part);
            await loadJobCard();
            alert("Part added successfully.");
        } catch (error) {
            console.error(error);
            alert("Unable to add part.");
        }
    };

    //--------------------------------------------------
    // Delete Part
    //--------------------------------------------------

    const deletePart = async (partId) => {
        try {
            await jobCardService.deletePart(jobCard.jobCardId, partId);
            await loadJobCard();
        } catch (error) {
            console.error(error);
            alert("Unable to delete part.");
        }
    };

    //--------------------------------------------------
    // Complete Job Card
    //--------------------------------------------------

    const completeJobCard = async () => {
        const confirmed = window.confirm(
            "Complete this Job Card?\n\nThis action cannot be undone."
        );

        if (!confirmed) return;

        try {
            await jobCardService.completeJobCard(jobCard.jobCardId);
            await loadJobCard();
            window.alert("Job Card completed.");
        } catch (error) {
            console.error(error);
            alert("Unable to complete Job Card.");
        }
    };

    //--------------------------------------------------

    if (loading) {
        return (
            <div className="text-center mt-5">
                <div className="spinner-border text-primary"></div>
                <p className="mt-3">Loading Job Card...</p>
            </div>
        );
    }

    if (!jobCard) {
        return <div className="alert alert-danger">Job Card not found.</div>;
    }

    //--------------------------------------------------

    return (
        <div className="container mt-4">
            <JobCardHeader jobCard={jobCard} />

            
{jobCard.jobCardType === 1 && role === "Admin" && (
    <div className="mt-3">
        
<button
    type="button"
    className="btn btn-outline-primary"
    onClick={() =>
        setShowAssignmentEditor(!showAssignmentEditor)
    }
>
    {showAssignmentEditor
        ? "Close Assignment Editor"
        : "Edit Assigned Technicians"}
</button>

    </div>
)}


{jobCard.jobCardType === 1 &&
    role === "Admin" &&
    showAssignmentEditor && (
        <div className="card mt-3">
            <div className="card-header">
                <strong>Manage Assigned Technicians</strong>
            </div>

            <div className="card-body">
               
<div className="mb-3">
    <h6 className="fw-bold">
        Currently Assigned Technicians
    </h6>

    {(jobCard.assignedTechnicians || []).length === 0 ? (
        <div className="alert alert-warning">
            No technicians are currently assigned.
        </div>
    ) : (
        <div className="list-group">
            {(jobCard.assignedTechnicians || []).map(
                (technician) => (
                    <div
                        key={technician.technicianId}
                        className="list-group-item d-flex justify-content-between align-items-center"
                    >
                        <span>
                            {technician.technicianName}
                        </span>

                        {technician.isLeadTechnician ? (
                            <span className="badge bg-primary">
                                Lead Technician
                            </span>
                        ) : (
                            <span className="badge bg-secondary">
                                Assigned Technician
                            </span>
                        )}
                    </div>
                )
            )}
        </div>
    )}
</div>

{/* Add Technician to Paper Job Card */}

<div className="mt-4">
    <h6 className="fw-bold">
        Add Technician
    </h6>

    <label className="form-label">
        Select Admin or Technician
    </label>

    <select
        className="form-select"
        value={selectedAssignmentTechnicianId}
        onChange={(e) =>
            setSelectedAssignmentTechnicianId(e.target.value)
        }
        disabled={loadingAssignableUsers || savingAssignment}
    >
        <option value="">
            {loadingAssignableUsers
                ? "Loading available technicians..."
                : "Select Technician"}
        </option>

        {assignableUsers
            .filter(
                (user) =>
                    !(jobCard.assignedTechnicians || []).some(
                        (assigned) =>
                            Number(assigned.technicianId) ===
                            Number(user.userId)
                    )
            )
            .map((user) => (
                <option
                    key={user.userId}
                    value={user.userId}
                >
                    {user.fullName}
                </option>
            ))}
    </select>


<div className="mt-3">
    <button
        type="button"
        className="btn btn-primary"
        onClick={handleAddPaperTechnician}
        disabled={
            !selectedAssignmentTechnicianId ||
            savingAssignment ||
            loadingAssignableUsers
        }
    >
        {savingAssignment
            ? "Assigning..."
            : "Add Technician"}
    </button>
</div>


    <small className="text-muted">
        Technicians already assigned to this Job Card
        are excluded from the list.
    </small>
</div>


            </div>
        </div>
    )}




{/*----------------------------------------------
    Complete Paper Job Card
----------------------------------------------*/}

{jobCard.jobCardType === 1 &&
    jobCard.status?.toLowerCase() !== "completed" &&
    (role === "Admin" || role === "Technician") && (

        <div className="d-flex justify-content-end mt-3">
            <button
                type="button"
                className="btn btn-success"
                onClick={completeJobCard}
            >
                Complete Job Card
            </button>
        </div>

)}


            <ul className="nav nav-tabs mt-4">
        
                <li className="nav-item">
                    <button
                        className={`nav-link ${activeTab === "information" ? "active" : ""}`}
                        onClick={() => setActiveTab("information")}
                    >
                        Information
                    </button>
                </li>

                <li className="nav-item">
                    <button
                        className={`nav-link ${activeTab === "work" ? "active" : ""}`}
                        onClick={() => setActiveTab("work")}
                    >
                        Work Details
                    </button>
                </li>

                <li className="nav-item">
                    <button
                        className={`nav-link ${activeTab === "labour" ? "active" : ""}`}
                        onClick={() => setActiveTab("labour")}
                    >
                        Labour
                    </button>
                </li>

                <li className="nav-item">
                    <button
                        className={`nav-link ${activeTab === "parts" ? "active" : ""}`}
                        onClick={() => setActiveTab("parts")}
                    >
                        Parts
                    </button>
                </li>

                <li className="nav-item">
                    <button
                        className={`nav-link ${activeTab === "history" ? "active" : ""}`}
                        onClick={() => setActiveTab("history")}
                    >
                        History
                    </button>
                </li>
            </ul>

            <div className="mt-4">
               
{activeTab === "information" && (

    <>
        <JobCardInformation jobCard={jobCard} />

        {jobCard.jobCardType === 1 && (
            <PaperJobCardTime
                jobCard={jobCard}
                onSaved={loadJobCard}
            />
        )}
    </>

)}


                
{activeTab === "work" && (

    jobCard.jobCardType === 1 ? (

        <PaperJobCardWork
            jobCard={jobCard}
            onSaved={loadJobCard}
            role={role}
        />

    ) : (

        <JobCardWorkDetails
            jobCard={jobCard}
            setJobCard={setJobCard}
            onSave={saveJobCard}
            onComplete={completeJobCard}
            role={role}
        />

    )

)}


                {activeTab === "labour" && (
    <LabourEntries
        jobCardId={jobCard.jobCardId}
        status={jobCard.status}
        onChangeParent={loadJobCard}
    />
)}

                
{activeTab === "parts" && (
    jobCard.jobCardType === 1 ? (
        <PaperJobCardParts
            jobCard={jobCard}
            onSaved={loadJobCard}
            role={role}
        />
    ) : (
        <JobCardParts
            jobCardId={jobCard.jobCardId}
            parts={jobCard.parts || []}
            onAdd={addPart}
            onDelete={deletePart}
            role={role}
            status={jobCard.status}
        />
    )
)}


                {activeTab === "history" && (
                    <JobCardHistory jobCardId={jobCard.jobCardId} />
                )}
            </div>
        </div>
    );
}

export default JobCardDetails;