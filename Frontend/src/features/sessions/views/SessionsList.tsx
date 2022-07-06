import { ISession } from '../ISession';
import { Session } from './Session';

export const SessionsList = (props: any) => {
    const renderedSessions = props.sessions.map((session: ISession) => (
        <Session session={session} activityId={props.activityId} colorHex={props.colorHex} key={session.id} />
    ));
    return (
        <div className="sessions-list">
            <div className="row">
                {renderedSessions}
            </div>
        </div>
    );
};