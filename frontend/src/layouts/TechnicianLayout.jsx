import { Outlet, Link } from "react-router-dom";

function TechnicianLayout() {
    return (
        <div className="container-fluid">

            <div className="row">

                {/* Sidebar */}

                <div className="col-md-2 bg-dark text-white vh-100">

                    <h3 className="mt-3 text-center">
                        IT Helpdesk
                    </h3>

                    <hr />

                    <ul className="nav flex-column">

                        <li className="nav-item mb-2">
                            <Link
                                to="/technician"
                                className="nav-link text-white"
                            >
                                🏠 Dashboard
                            </Link>
                        </li>

                        <li className="nav-item mb-2">
                            <Link
                                to="/technician/jobcards"
                                className="nav-link text-white"
                            >
                                📋 My Job Cards
                            </Link>
                        </li>

                    </ul>

                </div>

                {/* Main Content */}

                <div className="col-md-10">

                    <div className="p-4">

                        <Outlet />

                    </div>

                </div>

            </div>

        </div>
    );
}

export default TechnicianLayout;