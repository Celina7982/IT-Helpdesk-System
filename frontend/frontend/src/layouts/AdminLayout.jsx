import { Outlet } from "react-router-dom";

function AdminLayout() {
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
                            <a href="#" className="nav-link text-white">
                                Dashboard
                            </a>
                        </li>

                        <li className="nav-item mb-2">
                            <a href="#" className="nav-link text-white">
                                Tickets
                            </a>
                        </li>

                        <li className="nav-item mb-2">
                            <a href="#" className="nav-link text-white">
                                Users
                            </a>
                        </li>

                        <li className="nav-item mb-2">
                            <a href="#" className="nav-link text-white">
                                Reports
                            </a>
                        </li>

                        <li className="nav-item mb-2">
                            <a href="#" className="nav-link text-white">
                                Settings
                            </a>
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

export default AdminLayout;