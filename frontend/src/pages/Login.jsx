import { useNavigate } from "react-router-dom";

function Login() {

    const navigate = useNavigate();

    const handleLogin = () => {

    const email = document.querySelector("input[type='email']").value;

    if (email.toLowerCase().includes("tech")) {
        navigate("/technician");
    }
    else {
        navigate("/client");
    }

};

    return (

        <div className="container mt-5">

            <div className="row justify-content-center">

                <div className="col-md-5">

                    <div className="card shadow">

                        <div className="card-header bg-primary text-white text-center">

                            <h3>IT Helpdesk Login</h3>

                        </div>

                        <div className="card-body">

                            <div className="mb-3">

                                <label className="form-label">Email</label>

                                <input
                                    type="email"
                                    className="form-control"
                                    placeholder="Enter your email"
                                />

                            </div>

                            <div className="mb-3">

                                <label className="form-label">Password</label>

                                <input
                                    type="password"
                                    className="form-control"
                                    placeholder="Enter your password"
                                />

                            </div>

                            <button
                                className="btn btn-primary w-100"
                                onClick={handleLogin}
                            >
                                Login
                            </button>

                        </div>

                    </div>

                </div>

            </div>

        </div>

    );
}

export default Login;