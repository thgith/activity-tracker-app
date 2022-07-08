import moment from 'moment';
import { useState } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { Loader } from '../../app/views/Loader';
import { ISessionNew } from '../sessions/ISession';
import { addSession } from '../sessions/sessionMethods';
import { incrementTimer, pauseTimer, resetTimer, startTimer } from './timerSlice';

export const Timer = (props: any) => {
    const dispatch = useDispatch()
    const [loading, setLoading] = useState(false);
    const timerData = useSelector((state: any) => state.timer);
    let intervalHandle: any;

    const handleStartTimer = () => {
        const startDateStr = new Date().toString();
        intervalHandle = setInterval(() => {
            dispatch(incrementTimer({}));
        }, 1000);
        dispatch(startTimer({ startDate: startDateStr, intervalId: intervalHandle }));
    };

    const handlePauseTimer = () => {
        dispatch(pauseTimer({}));
    };

    const handleResetTimer = () => {
        clearInterval(timerData.intervalId);
        dispatch(resetTimer({}));
    };

    const handleAddSessionFromTimer = () => {
        clearInterval(timerData.intervalId);

        setLoading(true);
        dispatch(pauseTimer({}));
        var session: ISessionNew = {
            activityId: props.activityId,
            startDate: new Date(timerData.startDate),
            durationSeconds: timerData.secondsElapsed,
            notes: ''
        };

        dispatch<any>(addSession(session))
            .unwrap()
            .then(() => {
                dispatch(resetTimer({}));
                setLoading(false);
            })
            .catch((e: any) => {
                console.log(e);
                setLoading(false);
            });
    };

    const convertSecondsToTimeDisplay = () => {
        return moment.utc(timerData.secondsElapsed * 1000).format('HH:mm:ss');
    };

    if (!timerData) {
        return <Loader />
    }

    return (
        <div className="timer-container">
            <div className="timer-display text-center">
                <h1>{convertSecondsToTimeDisplay()}</h1>
            </div>
            <br></br>
            <div className="row">
                <div className="col-6 text-right">
                    <button
                        className="btn btn-secondary"
                        onClick={handleResetTimer}>
                        <span className="fa fa-repeat"></span>
                        <span>Reset</span>
                    </button>
                </div>
                <div className="col-6 text-left">
                    {timerData.isRunning ?
                        <button
                            className="btn btn-dark"
                            onClick={handlePauseTimer}
                        >
                            <span className="fa fa-pause"></span>
                            <span>Pause</span>
                        </button>
                        :
                        <button
                            className="btn btn-dark"
                            onClick={handleStartTimer}
                        >
                            <span className="fa fa-play"></span>
                            <span>Start</span>
                        </button>
                    }
                </div>

            </div>
            <br></br>
            <div className="row text-center">
                <div className="col-12">
                    <button disabled={timerData.secondsElapsed === 0}
                        className="btn btn-primary"
                        onClick={handleAddSessionFromTimer}
                    >
                        {loading ?
                            <span className="fa fa-spinner fa-pulse"></span> :
                            <span>
                                <span className="fa fa-save"></span>
                                <span>Save Session</span>
                            </span>}
                    </button>
                </div>
            </div>
        </div>
    );
};