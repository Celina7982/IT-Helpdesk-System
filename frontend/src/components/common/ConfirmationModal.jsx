function ConfirmationModal({

    show,
    title,
    message,
    confirmText = "Confirm",
    cancelText = "Cancel",
    confirmVariant = "success",
    onConfirm,
    onCancel

}) {

    if (!show)
        return null;

    return (

        <>

            <div
                className="modal fade show"
                style={{
                    display: "block",
                    backgroundColor: "rgba(0,0,0,0.5)"
                }}
            >

                <div className="modal-dialog modal-dialog-centered">

                    <div className="modal-content">

                        <div className="modal-header">

                            <h5 className="modal-title">

                                {title}

                            </h5>

                            <button
                                className="btn-close"
                                onClick={onCancel}
                            />

                        </div>

                        <div className="modal-body">

                            <p className="mb-0">

                                {message}

                            </p>

                        </div>

                        <div className="modal-footer">

                            <button
                                className="btn btn-secondary"
                                onClick={onCancel}
                            >

                                {cancelText}

                            </button>

                            <button
                                className={`btn btn-${confirmVariant}`}
                                onClick={onConfirm}
                            >

                                {confirmText}

                            </button>

                        </div>

                    </div>

                </div>

            </div>

        </>

    );

}

export default ConfirmationModal;