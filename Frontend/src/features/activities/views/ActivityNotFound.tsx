import { Link } from "react-router-dom"

export const ActivityNotFound = () => {
    return (
        <div className="container">
            <div className="panel-container text-center">
                <h2 className="colored-header">Activity Not Found</h2>
                <div className="panel-body-container">
                    <h5>
                        Return to <Link to="/">activities list</Link>
                    </h5>
                </div>
            </div>
        </div>
    );
};