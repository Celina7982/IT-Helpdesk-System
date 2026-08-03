

//component accepts 3 pieces of info 
function StatisticCard({ title, value, color }) {
    return (
        <div className="col-md-3 mb-4">

            <div className={`card shadow border-${color}`}>

                <div className="card-body text-center">

                    <h6 className="text-muted">
                        {title}
                    </h6>

                    <h2 className={`text-${color}`}>
                        {value}
                    </h2>

                </div>

            </div>

        </div>
    );
}

export default StatisticCard;