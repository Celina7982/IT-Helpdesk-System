import { useState } from "react";
import ticketService from "../../services/ticketService";

function CreateTicketModal({
    show,
    onClose,
    onTicketCreated
}) {

    const [saving, setSaving] = useState(false);

    const [formData, setFormData] = useState({

        subject: "",
        description: "",
        customerName: "",
        companyName: "",
        category: "Hardware",
        priority: "Medium"

    });

    if (!show)
        return null;

    //-------------------------------------------------------
    // Handle Input Change
    //-------------------------------------------------------

    const handleChange = (e) => {

        setFormData({

            ...formData,

            [e.target.name]: e.target.value

        });

    };

    //-------------------------------------------------------
    // Save Ticket
    //-------------------------------------------------------

    const handleSubmit = async (e) => {

        e.preventDefault();

        try {

            setSaving(true);

            await ticketService.createTicket(formData);

            alert("Ticket created successfully.");

            setFormData({

                subject: "",
                description: "",
                customerName: "",
                companyName: "",
                category: "Hardware",
                priority: "Medium"

            });

            onTicketCreated();

        }
        catch (error) {

            console.error(error);

            alert("Unable to create ticket.");

        }
        finally {

            setSaving(false);

        }

    };

    //-------------------------------------------------------

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

                    <form onSubmit={handleSubmit}>

                        <div className="modal-header bg-primary text-white">

                            <h5 className="modal-title">

                                Create New Ticket

                            </h5>

                            <button
                                type="button"
                                className="btn-close btn-close-white"
                                onClick={onClose}
                            />

                        </div>

                        <div className="modal-body">

                            <div className="row">

                                <div className="col-md-6 mb-3">

                                    <label className="form-label">
                                        Subject
                                    </label>

                                    <input
                                        type="text"
                                        className="form-control"
                                        name="subject"
                                        value={formData.subject}
                                        onChange={handleChange}
                                        required
                                    />

                                </div>

                                <div className="col-md-6 mb-3">

                                    <label className="form-label">
                                        Customer Name
                                    </label>

                                    <input
                                        type="text"
                                        className="form-control"
                                        name="customerName"
                                        value={formData.customerName}
                                        onChange={handleChange}
                                        required
                                    />

                                </div>

                            </div>

                            <div className="mb-3">

                                <label className="form-label">

                                    Company Name

                                </label>

                                <input
                                    type="text"
                                    className="form-control"
                                    name="companyName"
                                    value={formData.companyName}
                                    onChange={handleChange}
                                />

                            </div>

                            <div className="mb-3">

                                <label className="form-label">

                                    Description

                                </label>

                                <textarea
                                    className="form-control"
                                    rows="5"
                                    name="description"
                                    value={formData.description}
                                    onChange={handleChange}
                                    required
                                />

                            </div>

                            <div className="row">

                                <div className="col-md-6">

                                    <label className="form-label">

                                        Category

                                    </label>

                                    <select
                                        className="form-select"
                                        name="category"
                                        value={formData.category}
                                        onChange={handleChange}
                                    >

                                        <option>Hardware</option>
                                        <option>Software</option>
                                        <option>Network</option>
                                        <option>Email</option>
                                        <option>Printer</option>
                                        <option>Security</option>
                                        <option>Other</option>

                                    </select>

                                </div>

                                <div className="col-md-6">

                                    <label className="form-label">

                                        Priority

                                    </label>

                                    <select
                                        className="form-select"
                                        name="priority"
                                        value={formData.priority}
                                        onChange={handleChange}
                                    >

                                        <option>Low</option>
                                        <option>Medium</option>
                                        <option>High</option>

                                    </select>

                                </div>

                            </div>

                        </div>

                        <div className="modal-footer">

                            <button
                                type="button"
                                className="btn btn-secondary"
                                onClick={onClose}
                            >
                                Cancel
                            </button>

                            <button
                                type="submit"
                                className="btn btn-primary"
                                disabled={saving}
                            >

                                {saving
                                    ? "Creating..."
                                    : "Create Ticket"}

                            </button>

                        </div>

                    </form>

                </div>

            </div>

        </div>

    );

}

export default CreateTicketModal;