function StatusBadge({ status }) {

    let badgeClass = "bg-secondary";

    switch (status) {

        case "Open":
            badgeClass = "bg-success";
            break;

        case "In Progress":
            badgeClass = "bg-warning text-dark";
            break;

        case "Resolved":
            badgeClass = "bg-primary";
            break;

        case "Closed":
            badgeClass = "bg-dark";
            break;

        default:
            badgeClass = "bg-secondary";
            break;

    }

    return (

        <span className={`badge ${badgeClass}`}>

            {status}

        </span>

    );

}

export default StatusBadge;