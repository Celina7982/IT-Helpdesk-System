function PriorityBadge({ priority }) {

    let badgeClass = "bg-secondary";

    switch (priority) {

        case "High":
            badgeClass = "bg-danger";
            break;

        case "Medium":
            badgeClass = "bg-warning text-dark";
            break;

        case "Low":
            badgeClass = "bg-success";
            break;

        default:
            badgeClass = "bg-secondary";
            break;

    }

    return (

        <span className={`badge ${badgeClass}`}>

            {priority}

        </span>

    );

}

export default PriorityBadge;