import { useEffect } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { Link, useNavigate } from 'react-router-dom';
import { getUserIdCookie } from '../../../app/helpers/helpers';
import { Loader } from '../../../app/views/Loader';
import { getUser } from '../userSlice';

export const Profile = (props: any) => {
    const navigate = useNavigate();
    const dispatch = useDispatch()

    const { user: currentUser } = useSelector((state: any) => state.userData);

    useEffect(() => {
        const currUserId = getUserIdCookie();
        if (!currUserId) {
            // Redirect to login if not authenticated
            navigate('/login');
        } else if (!currentUser) {
            // Try to get user if not gotten yet
            dispatch<any>(getUser(currUserId))
                .unwrap()
                .then(() => { 
                })
                .catch(() => {
                });
        }
    });

    if (!currentUser) {
        return <Loader />
    }

    return (
        <div className="profile-view container">
            <div className="panel-container">
                <h2 className="colored-header text-center">{currentUser.firstName}'s Profile</h2>
                <div className="panel-body-container">
                    <div className="row text-center">
                        <span className="fa fa-user-circle fa-5x"></span>
                    </div>
                    <div className="row">
                        <div className="col-6">
                            <label htmlFor="firstName">First Name</label>
                            <h5>{currentUser.firstName}</h5>
                        </div>
                        <div className="col-6">
                            <label htmlFor="lastName">Last Name</label>
                            <h5>{currentUser.lastName}</h5>
                        </div>
                    </div>
                    <div className="row">
                        <div className="col-12">
                            <label htmlFor="email">Email</label>
                            <h5>{currentUser.email}</h5>
                        </div>
                    </div>
                    <div className="row">
                        <div className="col-12">
                            <label htmlFor="role">Role</label>
                            <h5>{currentUser.role}</h5>
                        </div>
                    </div>
                    <div className="row text-center">
                        <div className="action-button-group">
                            <Link to={`/profile/${currentUser.id}/edit`}>
                                <button className="btn btn-primary">
                                    <span className="fa fa-pencil-square-o fa-lg"></span>
                                    <span>Edit Profile</span>
                                </button>
                            </Link>
                            <Link to={`/profile/${currentUser.id}/changePassword`}>
                                <button className="btn btn-primary">
                                    <span className="fa fa-pencil-square-o fa-lg"></span>
                                    <span>Change Password</span>
                                </button>
                            </Link>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
};