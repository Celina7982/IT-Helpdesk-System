import { Outlet, Link } from "react-router-dom";

function AdminLayout() {
    return (
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
                            to="/admin"
                            className="nav-link text-white"
                        >
                            🏠 Dashboard
                        </Link>
                    </li>

                    <li className="nav-item mb-2">
                        <Link
                            to="/admin/jobcards"
                            className="nav-link text-white"
                        >
                            📋 Job Cards
                        </Link>
                    </li>

                    <li className="nav-item mb-2">
                        <Link
                            to="/admin/tickets"
                            className="nav-link text-white"
                        >
                            🎫 All Tickets
                        </Link>
                    </li>

                    {/* NEW */}
                    <li className="nav-item mb-2">
                        <Link
                            to="/admin/mytickets"
                            className="nav-link text-white"
                        >
                            📝 My Tickets
                        </Link>
                    </li>

                    <li className="nav-item mb-2">
                        <Link
                            to="/admin/users"
                            className="nav-link text-white"
                        >
                            👥 Users
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
    );
}

export default AdminLayout;