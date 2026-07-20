import { useState } from "react";
import clientService from "../services/clientService";

function CreateTicketModal({ show, onClose, onTicketCreated }) {

    const [ticket, setTicket] = useState({
        subject: "",
        description: "",
        customerName: "",
        companyName: "",
        category: "",
        priority: "Medium"
    });

    const handleChange = (e) => {

        setTicket({
            ...ticket,
            [e.target.name]: e.target.value
        });

    };

    const handleSubmit = async (e) => {

        e.preventDefault();

        try {

            await clientService.createTicket(ticket);

            alert("Ticket created successfully.");

            onTicketCreated();

            onClose();

            setTicket({
                subject: "",
                description: "",
                customerName: "",
                companyName: "",
                category: "",
                priority: "Medium"
            });

        }
        catch (error) {

            console.error(error);

            alert("Unable to create ticket.");

        }

    };

    if (!show) return null;

    return (

        <div
            className="modal d-block"
            style={{ backgroundColor: "rgba(0,0,0,0.5)" }}
        >

            <div className="modal-dialog modal-lg">

                <div className="modal-content">

                    <div className="modal-header">

                        <h5>Create New Ticket</h5>

                        <button
                            className="btn-close"
                            onClick={onClose}
                        />

                    </div>

                    <form onSubmit={handleSubmit}>

                        <div className="modal-body">

                            <div className="mb-3">

                                <label className="form-label">

                                    Subject

                                </label>

                                <input
                                    className="form-control"
                                    name="subject"
                                    value={ticket.subject}
                                    onChange={handleChange}
                                    required
                                />

                            </div>

                            <div className="mb-3">

                                <label className="form-label">

                                    Description

                                </label>

                                <textarea
                                    className="form-control"
                                    rows="4"
                                    name="description"
                                    value={ticket.description}
                                    onChange={handleChange}
                                    required
                                />

                            </div>

                            <div className="row">

                                <div className="col-md-6">

                                    <label className="form-label">

                                        Customer Name

                                    </label>

                                    <input
                                        className="form-control"
                                        name="customerName"
                                        value={ticket.customerName}
                                        onChange={handleChange}
                                        required
                                    />

                                </div>

                                <div className="col-md-6">

                                    <label className="form-label">

                                        Company Name

                                    </label>

                                    <input
                                        className="form-control"
                                        name="companyName"
                                        value={ticket.companyName}
                                        onChange={handleChange}
                                        required
                                    />

                                </div>

                            </div>

                            <div className="row mt-3">

                                <div className="col-md-6">

                                    <label className="form-label">

                                        Category

                                    </label>

                                    <input
                                        className="form-control"
                                        name="category"
                                        value={ticket.category}
                                        onChange={handleChange}
                                        required
                                    />

                                </div>

                                <div className="col-md-6">

                                    <label className="form-label">

                                        Priority

                                    </label>

                                    <select
                                        className="form-select"
                                        name="priority"
                                        value={ticket.priority}
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
                            >

                                Create Ticket

                            </button>

                        </div>

                    </form>

                </div>

            </div>

        </div>

    );

}

export default CreateTicketModal;