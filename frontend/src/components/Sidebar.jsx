import { Link } from "react-router-dom";

function Sidebar() {
    return (
        <div
            className="bg-dark text-white p-3"
            style={{ minHeight: "100vh", width: "250px" }}
        >
            <h3 className="text-center mb-4">IT Helpdesk</h3>

            <ul className="nav flex-column">

                <li className="nav-item mb-2">
                    <Link className="nav-link text-white" to="/client">
                        🏠 Dashboard
                    </Link>
                </li>

                <li className="nav-item mb-2">
                    <Link className="nav-link text-white" to="/create-ticket">
                        ➕ Create Ticket
                    </Link>
                </li>

                <li className="nav-item mb-2">
                    <Link className="nav-link text-white" to="/my-tickets">
                        🎫 My Tickets
                    </Link>
                </li>

                <li className="nav-item mb-2">
                    <Link className="nav-link text-white" to="/technician">
                        👨‍💻 Technician
                    </Link>
                </li>

                <hr className="text-secondary" />

                <li className="nav-item">
                    <Link className="nav-link text-danger" to="/">
                        🚪 Logout
                    </Link>
                </li>

            </ul>
        </div>
    );
}

export default Sidebar;