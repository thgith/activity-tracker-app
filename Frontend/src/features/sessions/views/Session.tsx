import moment from 'moment';
import { ISession } from '../ISession';
import { Link } from 'react-router-dom'
import { LONG_DATE_FORMAT } from '../../../app/constants';

const sessionDurationDisplay = (seconds: number) => {
    const momentTime = moment.utc(seconds * 1000);
    return (
    <span>
        {momentTime.format('H')} hours {momentTime.format('m')} minutes {momentTime.format('s')} seconds
    </span>);
}

export const Session = (props: any) => {
    const session: ISession = props.session;
    const activityId: string = props.activityId;
    const colorHexNoHash: string = props.colorHex.substring(1);
    return (
        <div className="card-container session-item col-md-4 col-sm-6 mt-2">
            <div className="card-item">
                <Link to={`/sessions/${session.id}?activityId=${activityId}&colorHex=${colorHexNoHash}`} style={{ textDecoration: 'none' }}>
                    <div className="session-header card-header" style={{ backgroundColor: props.colorHex }}>
                        <h5>{moment(session.startDateUtc.toString()).format(LONG_DATE_FORMAT)}</h5>
                    </div>
                    <div className="session-body card-body">
                        <p className="session-duration">{sessionDurationDisplay(session.durationSeconds)}</p>
                        <p className="session-notes">{session.notes}</p>
                    </div>
                </Link>
            </div>
        </div>
    );
};