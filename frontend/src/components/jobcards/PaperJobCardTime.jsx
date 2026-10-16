
import { useEffect, useState } from "react";
import jobCardService from "../../services/jobCardService";

function PaperJobCardTime({ jobCard, onSaved }) {

    const toLocalInput = (value) => {
        if (!value) return "";

        const date = new Date(value);

        if (Number.isNaN(date.getTime())) return "";

        const local = new Date(
            date.getTime() - date.getTimezoneOffset() * 60000
        );

        return local.toISOString().slice(0, 16);
    };

    const [timeIn, setTimeIn] = useState("");
    const [timeOut, setTimeOut] = useState("");
    const [isTravel, setIsTravel] = useState(false);
    const [isOnSite, setIsOnSite] = useState(false);
    const [saving, setSaving] = useState(false);

    useEffect(() => {
        setTimeIn(toLocalInput(jobCard.timeIn));
        setTimeOut(toLocalInput(jobCard.timeOut));
        setIsTravel(jobCard.isTravel ?? false);
        setIsOnSite(jobCard.isOnSite ?? false);
    }, [jobCard]);

    const isCompleted = jobCard.status === "Completed";

    const durationMinutes =
        timeIn && timeOut
            ? (new Date(timeOut) - new Date(timeIn)) / 60000
            : null;

    const durationText =
        durationMinutes !== null && durationMinutes >= 0
            ? `${Math.floor(durationMinutes / 60)}h ${Math.round(durationMinutes % 60)}m`
            : "-";

    const handleSave = async () => {

        if (isCompleted || saving) return;

        if (timeOut && !timeIn) {
            alert("Please enter Time In before Time Out.");
            return;
        }

        if (timeIn && timeOut && durationMinutes < 0) {
            alert("Time Out cannot be earlier than Time In.");
            return;
        }

        try {
            setSaving(true);

            await jobCardService.updatePaperJobTime(
                jobCard.jobCardId,
                {
                    timeIn: timeIn
                        ? new Date(timeIn).toISOString()
                        : null,
                    timeOut: timeOut
                        ? new Date(timeOut).toISOString()
                        : null,
                    isTravel,
                    isOnSite
                }
            );

            await onSaved();

            alert("Paper Job Card time updated successfully.");

        } catch (error) {
            console.error(error);

            alert(
                error.response?.data?.message ||
                "Unable to update Paper Job Card time."
            );

        } finally {
            setSaving(false);
        }
    };

    return (

        <div className="card mt-3">

            <div className="card-header">
                <strong>Paper Job Card — Time Tracking</strong>
            </div>

            <div className="card-body">

                <p className="text-muted">
                    Time is recorded once for the entire Paper Job Card,
                    not separately for each technician.
                </p>

                <div className="row g-3">

                    <div className="col-md-6">
                        <label className="form-label">Time In</label>

                        <input
                            type="datetime-local"
                            className="form-control"
                            value={timeIn}
                            onChange={(e) => setTimeIn(e.target.value)}
                            disabled={isCompleted || saving}
                        />
                    </div>

                    <div className="col-md-6">
                        <label className="form-label">Time Out</label>

                        <input
                            type="datetime-local"
                            className="form-control"
                            value={timeOut}
                            onChange={(e) => setTimeOut(e.target.value)}
                            disabled={isCompleted || saving}
                        />
                    </div>

                    <div className="col-md-6">
                        <div className="form-check">

                            <input
                                type="checkbox"
                                className="form-check-input"
                                id="paperIsTravel"
                                checked={isTravel}
                                onChange={(e) =>
                                    setIsTravel(e.target.checked)
                                }
                                disabled={isCompleted || saving}
                            />

                            <label
                                className="form-check-label"
                                htmlFor="paperIsTravel"
                            >
                                Travel
                            </label>

                        </div>
                    </div>

                    <div className="col-md-6">
                        <div className="form-check">

                            <input
                                type="checkbox"
                                className="form-check-input"
                                id="paperIsOnSite"
                                checked={isOnSite}
                                onChange={(e) =>
                                    setIsOnSite(e.target.checked)
                                }
                                disabled={isCompleted || saving}
                            />

                            <label
                                className="form-check-label"
                                htmlFor="paperIsOnSite"
                            >
                                On-Site
                            </label>

                        </div>
                    </div>

                </div>

                <div className="mt-3">
                    <strong>Total Duration: </strong>
                    {durationText}
                </div>

                {!isCompleted && (
                    <button
                        type="button"
                        className="btn btn-primary mt-3"
                        onClick={handleSave}
                        disabled={saving}
                    >
                        {saving ? "Saving..." : "Save Time"}
                    </button>
                )}

            </div>

        </div>

    );
}

export default PaperJobCardTime;
