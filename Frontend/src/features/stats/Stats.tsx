import { useEffect } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { useNavigate } from 'react-router-dom';
import { getUserIdCookie } from '../../app/helpers/helpers';
import { clearMessage } from '../message/messageSlice';
import { resetTimer } from '../timer/timerSlice';

export const Stats = () => {
    const dispatch = useDispatch()
    const navigate = useNavigate();
    const timerData = useSelector((state: any) => state.timer);

    useEffect(() => {
        dispatch(clearMessage());
        if (timerData.intervalId) {
            clearInterval(timerData.intervalId);
            dispatch(resetTimer({}));
        }
    }, [dispatch, timerData.intervalId]);

    useEffect(() => {
        // Redirect to login if not authenticated
        if (!getUserIdCookie()) {
            navigate('/login');
        }
    });

    return (
        <div className="stats container">
            <div className="panel-container panel-lg">
                <h2 className="colored-header text-center">Statistics</h2>
                <div className="panel-body-container">
                    Total Time
                    TBD
                </div>
            </div>

        </div>
    );
};