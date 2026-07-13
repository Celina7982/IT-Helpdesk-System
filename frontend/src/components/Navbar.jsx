import { Link } from "react-router-dom";

function Navbar() {
    return (
        <nav className="navbar navbar-expand-lg navbar-dark bg-primary shadow">

            <div className="container">

                <Link className="navbar-brand fw-bold" to="/">
                    IT Helpdesk
                </Link>

                <button
                    className="navbar-toggler"
                    type="button"
                    data-bs-toggle="collapse"
                    data-bs-target="#navbarNav"
                >
                    <span className="navbar-toggler-icon"></span>
                </button>

                <div
                    className="collapse navbar-collapse"
                    id="navbarNav"
                >

                    <ul className="navbar-nav ms-auto">

                        <li className="nav-item">
                            <Link className="nav-link" to="/client">
                                Dashboard
                            </Link>
                        </li>

                        <li className="nav-item">
                            <Link className="nav-link" to="/create-ticket">
                                Create Ticket
                            </Link>
                        </li>

                        <li className="nav-item">
                            <Link className="nav-link" to="/my-tickets">
                                My Tickets
                            </Link>
                        </li>

                        <li className="nav-item">
                            <Link className="nav-link" to="/">
                                Logout
                            </Link>
                        </li>

                    </ul>

                </div>

            </div>

        </nav>
    );
}

export default Navbar;