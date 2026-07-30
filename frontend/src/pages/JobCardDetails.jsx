import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";

import jobCardService from "../services/jobCardService";

import JobCardHeader from "../components/jobcards/JobCardHeader";
import JobCardInformation from "../components/jobcards/JobCardInformation";
import JobCardWorkDetails from "../components/jobcards/JobCardWorkDetails";
import LabourEntries from "../components/jobcards/LabourEntries";

import { jwtDecode } from "jwt-decode";
import JobCardHistory from "../components/jobcards/JobCardHistory";

function JobCardDetails() {
    const { id } = useParams();

    const [jobCard, setJobCard] = useState(null);
    const [loading, setLoading] = useState(true);
    const [activeTab, setActiveTab] = useState("information");

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

    useEffect(() => {
        loadJobCard();
    }, [id]);

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

            // Reload the Job Card so the new labour
            // entry is displayed immediately.
            await loadJobCard();

            alert("Labour entry added successfully.");
        } catch (error) {
            console.error(error);
            alert("Unable to add labour entry.");
        }
    };

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
                        className={`nav-link ${activeTab === "history" ? "active" : ""}`}
                        onClick={() => setActiveTab("history")}
                    >
                        History
                    </button>
                </li>
            </ul>

            <div className="mt-4">
                {activeTab === "information" && (
                    <JobCardInformation jobCard={jobCard} />
                )}

                {activeTab === "work" && (
                    <JobCardWorkDetails
                        jobCard={jobCard}
                        setJobCard={setJobCard}
                        onSave={saveJobCard}
                        onComplete={completeJobCard}
                        role={role}
                    />
                )}

                {activeTab === "labour" && (
                    <LabourEntries
                        labourEntries={jobCard.labourEntries || []}
                        onAdd={addLabourEntry}
                        role={role}
                        status={jobCard.status}
                    />
                )}

                {activeTab === "history" && (
                    <JobCardHistory jobCardId={jobCard.jobCardId} />
                )}
            </div>
        </div>
    );
}

export default JobCardDetails;
