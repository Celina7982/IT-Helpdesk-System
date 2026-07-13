import Navbar from "../components/Navbar";
function CreateTicket() {
    return (
       

    <div>

        <Navbar />

        <div className="container mt-4"></div>
            <h2>Create New Ticket</h2>

            <div className="card shadow mt-3">

                <div className="card-body">

                    {/* Subject */}
                    <div className="mb-3">
                        <label className="form-label">Subject</label>

                        <input
                            type="text"
                            className="form-control"
                            placeholder="Enter ticket subject"
                        />
                    </div>

                    {/* Category */}
                    <div className="mb-3">
                        <label className="form-label">Category</label>

                        <select className="form-select">
                            <option>Select Category</option>
                            <option>Hardware</option>
                            <option>Software</option>
                            <option>Network</option>
                            <option>Email</option>
                            <option>Other</option>
                        </select>
                    </div>

                    {/* Priority */}
                    <div className="mb-3">
                        <label className="form-label">Priority</label>

                        <select className="form-select">
                            <option>Low</option>
                            <option>Medium</option>
                            <option>High</option>
                        </select>
                    </div>

                    {/* Description */}
                    <div className="mb-3">
                        <label className="form-label">Description</label>

                        <textarea
                            className="form-control"
                            rows="5"
                            placeholder="Describe your issue..."
                        ></textarea>
                    </div>

                    <button className="btn btn-success">
                        Submit Ticket
                    </button>

                </div>

            </div>

            


        </div>
    );
}

export default CreateTicket;