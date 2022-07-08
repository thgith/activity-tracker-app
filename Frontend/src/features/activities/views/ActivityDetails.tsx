import { useState } from 'react'
import { useDispatch, useSelector } from 'react-redux'
import { Link, useNavigate, useParams } from "react-router-dom"; // This is syntax that changed with v6
import moment from 'moment';
import { STANDARD_DATE_DISPLAY_FORMAT } from '../../../app/constants';
import { displayTags, getUserIdCookie, useEffectSkipInitialRender } from '../../../app/helpers/helpers';
import { SessionsList } from '../../sessions/views/SessionsList';
import { Timer } from '../../timer/Timer';
import { getUser } from '../../User/userSlice';
import { ISession } from '../../sessions/ISession';
import { getActivity } from '../activityMethods';
import { Loader } from '../../../app/views/Loader';

export const ActivityDetails = () => {
    const navigate = useNavigate();
    const dispatch = useDispatch()
    const [activityFromGet, setActivityFromGet] = useState(null);
    const [gotActivity, setGotActivity] = useState(false);
    const [detailsExpanded, setDetailsExpanded] = useState(true);

    const { user: currentUser } = useSelector((state: any) => state.userData);

    const { activityId } = useParams();

    const activityFromList = useSelector((state: any) =>
        state.activitiesData.activities.find((activity: any) => activity.id === activityId)
    )
    const activityIdToSessions = useSelector((state: any) => state.activitiesData.activityIdToSessions);

    let activity = activityFromList;
    useEffectSkipInitialRender(() => {
        const currUserId = getUserIdCookie();
        if (!currUserId) {
            // Redirect to login if not authenticated
            navigate('/login');
        } else if (!currentUser) {
            // Get user if we don't have them
            dispatch<any>(getUser(currUserId))
                .unwrap()
                .then(() => {
                })
                .catch(() => {
                });
        } else if (!activity) {
            // Get single activity if we don't have it
            dispatch<any>(getActivity(activityId as string))
                .unwrap()
                .then((res: any) => {
                    setActivityFromGet(res.activity);
                    setGotActivity(true);
                })
                .catch(() => {
                    setGotActivity(true);
                });
        }
    });

    activity = activityFromList ?? activityFromGet;

    const getTotalActivityHours = () => {
        let totalSeconds = 0;
        let sessions: any = []
        if (activityIdToSessions && activityIdToSessions[activity.id]) {
            sessions = activityIdToSessions[activity.id];
        }
        sessions.map((x: ISession) => {
            totalSeconds += x.durationSeconds
        });

        return Math.round(totalSeconds / 3600 * 10) / 10;
    }

    const isActivityOverdue = () => {
        return !activity.completedDateUtc &&
            activity.dueDateUtc !== null &&
            activity.dueDateUtc &&
            activity.dueDateUtc > new Date();
    }

    if (!activity && !gotActivity) {
        if (!gotActivity) {
            return <Loader />
        }

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
        )
    }

    return (
        <div className="activity-view container">
            <div className="row">
                <div className="col-2 d-none d-md-block">
                    <Link to={`/`} style={{ textDecoration: 'none' }}>
                        <button className="btn btn-secondary">
                            <span className="fa fa-chevron-left fa-lg"></span>
                            <span>Back</span>
                        </button>
                    </Link>
                </div>
                <div className="panel-container panel-lg col-12 col-md-8">
                    <h2 className="activity-header colored-header text-center" style={{ backgroundColor: activity.colorHex }}>{activity.name}</h2>
                    <div className="activity-panel-body-container panel-body-container ">
                        <div className="row">
                            <div className="col-2">
                                <button
                                    className="btn btn-secondary d-none d-lg-inline-block"
                                    type="button"
                                    data-bs-toggle="collapse"
                                    data-bs-target="#activity-info"
                                    aria-expanded="false"
                                    aria-controls="collapseExample"
                                    // Must check this way b/c this comes out as a boolean string not a boolean
                                    onClick={(e: any) => { setDetailsExpanded(Boolean(e.currentTarget.attributes['aria-expanded'] && e.currentTarget.attributes['aria-expanded'].value === 'true')) }}>
                                    {detailsExpanded ?
                                        <span><span className="fa fa-eye-slash" /><span>Hide</span></span> :
                                        <span><span className="fa fa-eye" /><span>Show</span></span>
                                    }
                                </button>
                            </div>
                            <div className="col-12 col-lg-8">
                                <div id="activity-info" className="collapse show">
                                    {activity.tags.length > 0 ?
                                        <div className="tags-container">
                                            {displayTags(activity.tags)}
                                        </div> : ""}
                                    <div className="col-12 text-center">
                                        <div className="activity-description text-center">{activity.description}</div>
                                    </div>
                                    <div className="date-row row text-center">
                                        <div className="col-4">
                                            <label>Start Date</label>
                                            <p className="activity-start-date">
                                                {moment(activity.startDateUtc).local().format(STANDARD_DATE_DISPLAY_FORMAT)}
                                            </p>
                                        </div>
                                        <div className="col-4">
                                            <label>Due Date
                                                {isActivityOverdue() ?? 
                                                    <span
                                                        className="alert-tooltip tooltip-bubble fa fa-exclamation-circle"
                                                        data-toggle="tooltip"
                                                        data-placement="top"
                                                        title="This activity is overdue">
                                                    </span>}
                                            </label>
                                            <p className="activity-due-date">
                                                {activity.dueDateUtc ? moment(activity.dueDateUtc).format(STANDARD_DATE_DISPLAY_FORMAT) : 'N/A'}
                                            </p>
                                        </div>
                                        <div className="col-4">
                                            <label>Completed Date</label>
                                            <p className="activity-completed-date">
                                                {activity.completedDateUtc ? moment(activity.completedDateUtc).format(STANDARD_DATE_DISPLAY_FORMAT) : 'N/A'}
                                            </p>
                                        </div>
                                    </div>
                                    <div className="row">
                                        <div className="col-2">
                                        </div>
                                        <div className="col-8">
                                            <div id="timer" className="collapse show">
                                                <Timer activityId={activityId}></Timer>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div className="row d-md-none">
                            <div className="col-6">
                                <Link to={`/`} style={{ textDecoration: 'none' }}>
                                    <button className="btn btn-secondary">
                                        <span className="fa fa-chevron-left fa-lg"></span>
                                        <span>Back</span>
                                    </button>
                                </Link>
                            </div>
                            <div className="col-6">
                                <div className="text-right">
                                    <Link to={`/activities/${activity.id}/edit`} style={{ textDecoration: 'none' }}>
                                        <button className="btn btn-primary">
                                            <span className="fa fa-pencil-square-o fa-lg"></span>
                                            <span>Edit</span>
                                        </button>
                                    </Link>
                                </div>
                            </div>
                        </div>
                        <hr></hr>
                        <div className="row">
                            <div className="col-6">
                                <h3>Sessions</h3>
                                <div>
                                    <span className="fa fa-clock-o"></span>
                                    <b>Total Time: {getTotalActivityHours()} hours</b>
                                </div>
                            </div>
                            <div className="col-6 text-right">
                                <Link to={`/sessions/new?activityId=${activity.id}&colorHex=${activity.colorHex.slice(1)}`}>
                                    <button className="btn btn-primary">
                                        <span className="fa fa-plus"></span>
                                        <span>Manually Add Session</span>
                                    </button>
                                </Link>
                            </div>
                        </div>
                        {(activityIdToSessions[activity.id] && activityIdToSessions[activity.id].length > 0) ?
                            <SessionsList sessions={activityIdToSessions[activity.id]} activityId={activity.id} colorHex={activity.colorHex}></SessionsList> :
                            <h4 className="no-sessions text-center">No sessions yet. Start the timer to create your first session.</h4>}
                    </div>

                </div>
                <div className="col-2 text-right d-none d-md-block">
                    <Link to={`/activities/${activity.id}/edit`} style={{ textDecoration: 'none' }}>
                        <button className="btn btn-primary">
                            <span className="fa fa-pencil-square-o fa-lg"></span>
                            <span>Edit</span>
                        </button>
                    </Link>
                </div>
            </div>
        </div>
    );
};