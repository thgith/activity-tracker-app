import { useDispatch } from 'react-redux';
import { Link } from 'react-router-dom';
import { getUserIdCookie } from '../helpers/helpers';
import { logOut } from '../../features/User/userSlice';

export const Navbar = () => {
    const dispatch = useDispatch();
    const currUserId = getUserIdCookie();

    const handleLogout = () => {
        dispatch<any>(logOut())
            .unwrap()
            .then(() => {
                console.log("succesffuly logged out");
                // Go ahead and refresh instead of navigate() to clear state
                window.location.href = '/login';
            })
            .catch(() => {
            });
    };

    return (
        <nav className="navbar navbar-expand-lg navbar-collapse navbar-dark bg-dark">
            <div className="container">
                <button
                    className="navbar-toggler"
                    type="button"
                    data-bs-toggle="collapse"
                    data-bs-target="#activity-tracker-navbar"
                    aria-controls="activity-tracker-navbar"
                    aria-expanded="false"
                    aria-label="Toggle navigation">
                    <span className="navbar-toggler-icon"></span>
                </button>
                <div id="activity-tracker-navbar" className="collapse navbar-collapse">
                    <Link to="/" className="navbar-brand">
                        Activity Tracker App
                    </Link>
                    <ul className="navbar-nav mr-auto">
                        <li className="nav-item active">
                            <Link to="/" className="nav-link">
                                <span className="fa fa-home"></span>
                                <span>Home</span>
                                <span className="sr-only">(current)</span>
                            </Link>
                        </li>
                        {currUserId ? <li className="nav-item">
                            <Link to="/stats" className="nav-link">
                                <span className="fa fa-bar-chart"></span>
                                <span>Stats</span>
                            </Link>
                        </li> : null}
                        {currUserId ? <li className="nav-item">
                            <Link to={`/profile/${currUserId}`} className="nav-link">
                                <span className="fa fa-user-circle"></span>
                                <span>Profile</span>
                            </Link>
                        </li> : null}
                        {!currUserId ? <li className="nav-item">
                            <Link to="/register" className="nav-link">Register</Link>
                        </li> : null}
                        {currUserId ? (<li className="nav-item" onClick={handleLogout}>
                            <Link to="/login" className="nav-link">
                                <span className="fa fa-sign-out"></span>
                                Log Out
                            </Link>
                        </li>) : null}
                    </ul>
                </div>
            </div>
        </nav>
    );
};