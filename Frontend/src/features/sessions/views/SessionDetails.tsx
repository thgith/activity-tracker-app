import { useEffect, useState } from 'react'
import { useDispatch, useSelector } from 'react-redux'
import { Link, useNavigate, useParams, useSearchParams } from "react-router-dom"; // This is syntax that changed with v6
import moment from 'moment';
import { LONG_DATE_FORMAT } from '../../../app/constants';
import { getUserIdCookie, useEffectSkipInitialRender } from '../../../app/helpers/helpers';
import { ISession } from '../ISession';
import { getUser } from '../../User/userSlice';
import { listSessionsForActivity } from '../sessionMethods';
import { Loader } from '../../../app/views/Loader';
import { SessionNotFound } from './SessionNotFound';
import { clearMessage } from '../../message/messageSlice';
import { resetTimer } from '../../timer/timerSlice';

export const SessionDetails = () => {
    const dispatch = useDispatch()
    const navigate = useNavigate();
    const [searchParams] = useSearchParams();
    const activityId = searchParams.get('activityId');
    const colorHexNoHash = searchParams.get('colorHex');
    const colorHex = `#${colorHexNoHash}`;
    const [loading, setLoading] = useState(false);
    const { user: currentUser } = useSelector((state: any) => state.userData);
    const [gotSessions, setGotSessions] = useState(false);
    var activityIdToSessions = useSelector((state: any) => state.activitiesData.activityIdToSessions)
    const timerData = useSelector((state: any) => state.timer);

    useEffect(() => {
        dispatch(clearMessage());
        clearInterval(timerData.intervalId);
        dispatch(resetTimer({}));
    }, [dispatch]);

    let sessions: any = null;
    // Try to get session from dictionary
    if (activityIdToSessions[activityId as string]) {
        sessions = activityIdToSessions[activityId as string]
    }

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
        }
        // Request sessions if not in dictionary
        else if (!sessions && !gotSessions) {
            // Get single activity if we don't have it
            dispatch<any>(listSessionsForActivity(activityId as string))
                .unwrap()
                .then((res: any) => {
                    sessions = res.sessions;
                    setGotSessions(true);
                })
                .catch(() => {
                    setGotSessions(true);
                });
        }
    });

    const { sessionId } = useParams();
    let session = null;
    if (sessions) {
        session = (sessions as any).find((x: ISession) => x.id === sessionId);
    }

    if (!session) {
        if (!gotSessions) {
            return <Loader />
        }

        return <SessionNotFound />
    }

    return (
        <div className="session-view container">
            <div className="row">
                <div className="col-2 d-none d-md-block text-left">
                    <Link to={`/activities/${activityId}`}>
                        <button className="btn btn-secondary">
                            <span className="fa fa-chevron-left fa-lg"></span>
                            <span>Back</span></button>
                    </Link>
                </div>
                <div className="col-12 col-md-8">
                    <div className="panel-container">
                        <h2 className="session-start-date colored-header text-center" style={{ backgroundColor: colorHex }}>
                            {moment(session.startDateUtc.toString()).format(LONG_DATE_FORMAT)}
                        </h2>
                        <div className="panel-body-container">
                            <div className="row">
                                <h1 className="session-duration text-center">{getHourFromDurationSec(session.durationSeconds)} hr {getRemainingMinFromDurationSec(session.durationSeconds)} min {getRemainingSecFromDurationSec(session.durationSeconds)} sec</h1>
                                <p className="session-notes text-center">{session.notes}</p>
                            </div>
                        </div>
                    </div>
                </div>
                <div className="col-2 d-none d-md-block text-right">
                    <Link to={`/sessions/${session.id}/edit?activityId=${activityId}&colorHex=${colorHexNoHash}`}>
                        <button className="btn btn-primary">
                            <span className="fa fa-pencil-square-o fa-lg"></span>
                            <span>Edit</span>
                        </button>
                    </Link>
                </div>
            </div>


        </div>
    )
}

const getHourFromDurationSec = (durationSeconds: number) => {
    return Math.floor(durationSeconds / 3600);
}


const getRemainingMinFromDurationSec = (durationSeconds: number) => {
    return Math.round(durationSeconds % 3600 / 60)
}

const getRemainingSecFromDurationSec = (durationSeconds: number) => {
    return durationSeconds % 3600 % 60
}
