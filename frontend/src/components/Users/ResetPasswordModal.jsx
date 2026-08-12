import { useState } from "react";

function ResetPasswordModal({
    show,
    onClose,
    onSave,
    user
}) {

    const [password, setPassword] = useState("");

    if (!show) return null;

    const handleSubmit = (e) => {

        e.preventDefault();

        onSave(password);

        setPassword("");

    };

    return (

        <div
            className="modal d-block"
            style={{ background: "rgba(0,0,0,.5)" }}
        >

            <div className="modal-dialog">

                <div className="modal-content">

                    <form onSubmit={handleSubmit}>

                        <div className="modal-header">

                            <h5>

                                Reset Password

                            </h5>

                            <button
                                type="button"
                                className="btn-close"
                                onClick={onClose}
                            />

                        </div>

                        <div className="modal-body">

                            <p>

                                Reset password for

                                <strong>

                                    {" "}

                                    {user?.firstName} {user?.lastName}

                                </strong>

                            </p>

                            <input
                                type="password"
                                className="form-control"
                                placeholder="New Password"
                                value={password}
                                onChange={(e) =>
                                    setPassword(e.target.value)
                                }
                                required
                            />

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
                                className="btn btn-primary"
                                type="submit"
                            >

                                Reset Password

                            </button>

                        </div>

                    </form>

                </div>

            </div>

        </div>

    );

}

export default ResetPasswordModal;