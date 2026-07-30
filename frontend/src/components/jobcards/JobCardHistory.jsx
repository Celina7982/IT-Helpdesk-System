import { useEffect, useState } from "react";
import jobCardService from "../../services/jobCardService";



function JobCardHistory({ jobCardId }) {

    const [history, setHistory] = useState([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {

        loadHistory();

    }, [jobCardId]);

    

    const loadHistory = async () => {

        try {

            const data = await jobCardService.getAuditHistory(jobCardId);

            setHistory(data);

        }
        catch (error) {

            console.error(error);

        }
        finally {

            setLoading(false);

        }

    };

    if (loading) {

        return (
            <div className="text-center p-4">
                Loading history...
            </div>
        );

    }

    if (history.length === 0) {

        return (

            <div className="alert alert-info">

                No audit history available.

            </div>

        );

    }

    return (

        <div className="card">

            <div className="card-body">

                <table className="table table-striped">

                    <thead>

                        <tr>

                            <th>Date</th>
                            <th>User</th>
                            <th>Action</th>
                            <th>Description</th>

                        </tr>

                    </thead>

                    <tbody>

                        {history.map(item => (

                            <tr key={item.auditId}>

                                <td>
                                    {new Date(item.dateCreated).toLocaleString()}
                                </td>

                                <td>
                                    {item.performedBy}
                                </td>

                                <td>
                                    {item.action}
                                </td>

                                <td>

                                    <div>

                                        {item.description}

                                    </div>

                                    {(item.oldValue || item.newValue) && (

                                        <small className="text-muted">

                                            {item.oldValue} → {item.newValue}

                                        </small>

                                    )}

                                </td>

                            </tr>

                        ))}

                    </tbody>

                </table>

            </div>

        </div>

    );

}

export default JobCardHistory;