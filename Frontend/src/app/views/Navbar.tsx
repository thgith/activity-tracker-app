import { useDispatch } from 'react-redux';
import { Link, useNavigate } from 'react-router-dom';
import { getUserIdCookie } from '../helpers/helpers';
import { logOut } from '../../features/User/userSlice';

export const Navbar = () => {
    const dispatch = useDispatch();
    const navigate = useNavigate();
    const currUserId = getUserIdCookie();

    const handleLogout = () => {
        dispatch<any>(logOut())
            .unwrap()
            .then(() => {
                console.log("succesffuly logged out");
                navigate('/login');
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
                    data-toggle="collapse"
                    data-target="#navbarTogglerDemo01"
                    aria-controls="navbarTogglerDemo01"
                    aria-expanded="false"
                    aria-label="Toggle navigation">
                    <span className="navbar-toggler-icon"></span>
                </button>
                <div className="collapse navbar-collapse">
                    <Link to="/" className="navbar-brand">
                        Activity Tracker App
                    </Link>
                    <ul className="navbar-nav mr-auto">
                        <li className="nav-item active">
                            <Link to="/" className="nav-link">
                                <span className="fa fa-home"></span>
                                <span>Home</span>
                                <span className="sr-only">(current)</span></Link>
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