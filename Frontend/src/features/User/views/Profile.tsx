import moment from 'moment';
import { useEffect } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { Link, useNavigate } from 'react-router-dom';
import { STANDARD_DATE_DISPLAY_FORMAT } from '../../../app/constants';
import { getUserIdCookie } from '../../../app/helpers/helpers';
import { Loader } from '../../../app/views/Loader';
import { clearMessage } from '../../message/messageSlice';
import { resetTimer } from '../../timer/timerSlice';
import { getUser } from '../userSlice';

export const Profile = (props: any) => {
    const navigate = useNavigate();
    const dispatch = useDispatch()
    const timerData = useSelector((state: any) => state.timer);

    const { user: currentUser } = useSelector((state: any) => state.userData);

    useEffect(() => {
        dispatch(clearMessage());
        if (timerData.intervalId) {
            clearInterval(timerData.intervalId);
            dispatch(resetTimer({}));
        }
    }, [dispatch, timerData.intervalId]);

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
                            <div>{currentUser.firstName}</div>
                        </div>
                        <div className="col-6">
                            <label htmlFor="lastName">Last Name</label>
                            <div>{currentUser.lastName}</div>
                        </div>
                    </div>
                    <div className="row">
                        <div className="col-12">
                            <label htmlFor="email">Email</label>
                            <div>{currentUser.email}</div>
                        </div>
                    </div>
                    <div className="row">
                        <div className="col-6">
                            <label htmlFor="role">Date Joined</label>
                            <div>{moment(currentUser.joinDateUtc).format(STANDARD_DATE_DISPLAY_FORMAT)}</div>
                        </div>
                        <div className="col-6">
                            <label htmlFor="role">Role</label>
                            <div>{currentUser.role}</div>
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