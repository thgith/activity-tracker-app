import { useState } from 'react'
import { useDispatch, useSelector } from 'react-redux'
import { Link, useNavigate, useParams, useSearchParams } from 'react-router-dom'; // This is syntax that changed with v6
import { Field, Form, Formik } from 'formik';
import * as Yup from 'yup';
import moment from 'moment';
import { PICKER_DATE_DISPLAY_FORMAT, REQUIRED_FIELD_MSG } from '../../../app/constants';
import { getUserIdCookie, useEffectSkipInitialRender } from '../../../app/helpers/helpers';
import { Loader } from '../../../app/views/Loader';
import { SessionNotFound } from './SessionNotFound';
import { ISession } from '../ISession';
import { getUser } from '../../User/userSlice';
import { listSessionsForActivity, deleteSession, addSession, editSession } from '../sessionMethods';

export const SessionEdit = () => {
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
                    // sessions = res.sessions;
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

    const calculateRemainingMinutes = (durationSeconds: number) => {
        return Math.round(durationSeconds % 3600 / 60)
    }

    const onDeleteClicked = (e: any) => {
        e.preventDefault()
        console.log('delete clicked')
        setLoading(true);
        dispatch<any>(deleteSession({ activityId, sessionId } as any))
            .unwrap()
            .then(() => {
                navigate(`/activities/${activityId}`);
            })
            .catch((e: any) => {
                console.log(e)
                setLoading(false);
            });
    };

    const insertSelectOptions = () => {
        const selects = [];
        for (let i = 0; i < 60; i++)
            selects.push(<option value={i} key={i}>{i}</option>)

        return selects;
    }

    const combineDateAndTime = (dateVal: string, timeVal: string) => {
        var dateDt = moment(dateVal).format(PICKER_DATE_DISPLAY_FORMAT);
        var timeDt = moment(timeVal, ["h:mm A"]).format("HH:mm");
        let date = new Date(dateDt + ' ' + timeDt);
        return date;
    };

    const convertToDurationSeconds = (hours: number, minutes: number) => {
        return hours * 3600 + minutes * 60;
    };

    const handleEdit = (formValue: any) => {
        console.log('handling edit');
        const { startDateOnly, startTimeOnly, durationHours, durationMin, notes } = formValue;
        var session: ISession = {
            id: sessionId,
            activityId: activityId,
            startDateUtc: combineDateAndTime(startDateOnly, startTimeOnly),
            durationSeconds: convertToDurationSeconds(durationHours, durationMin),
            notes: notes
        }
        setLoading(true);
        dispatch<any>(editSession(session))
            .unwrap()
            .then(() => {
                navigate(`/sessions/${sessionId}?activityId=${session.activityId}&colorHex=${colorHexNoHash}`);
            })
            .catch((e: any) => {
                console.log(e);
                setLoading(false);
            });
    };

    const initialValues = {
        notes: session.notes ?? '',
        startDateOnly: moment(session.startDateUtc).format(PICKER_DATE_DISPLAY_FORMAT),
        startTimeOnly: moment(session.startDateUtc).format('hh:mm'),
        durationHours: Math.floor(session.durationSeconds / 3600),
        durationMin: calculateRemainingMinutes(session.durationSeconds)
    };

    const validationSchema = Yup.object().shape({
        startDateOnly: Yup.string()
            .required(REQUIRED_FIELD_MSG),
    });

    return (
        <div className="session-edit container">
            <div className="row">
                <div className="col-2 d-none d-md-block">
                    <Link to={`/sessions/${sessionId}?activityId=${activityId}&colorHex=${colorHexNoHash}`}>
                        <button className="btn btn-secondary">
                            <span className="fa fa-chevron-left fa-lg"></span>
                            <span>Back</span>
                        </button>
                    </Link>
                </div>
                <div className="col-12 col-md-8">
                    <div className="panel-container">
                        <h2 className="colored-header text-center" style={{ backgroundColor: colorHex }}>Edit Session</h2>
                        <Formik
                            initialValues={initialValues}
                            validationSchema={validationSchema}
                            onSubmit={handleEdit}
                        >
                            <Form>
                                <div className="panel-body-container">
                                    <div className="form-group row">
                                        <label className="form-label">Started At
                                            <span className="mandatory-field">*</span>
                                        </label>
                                        <div className="col-6">
                                            <Field className="form-control"
                                                type="date"
                                                id="startDateOnly"
                                                name="startDateOnly"
                                            />
                                        </div>
                                        <div className="col-6">
                                            <Field className="form-control"
                                                type="time"
                                                id="startTimeOnly"
                                                name="startTimeOnly"
                                            />
                                        </div>
                                    </div>
                                    <div className="form-group row">
                                        <label className="form-label" htmlFor="duration">Duration
                                            <span className="mandatory-field">*</span>
                                        </label>
                                        <div className="col-3">
                                            <Field
                                                as="select"
                                                className="form-control"
                                                id="durationHours"
                                                name="durationHours">
                                                {insertSelectOptions()}
                                            </Field>
                                        </div>
                                        <div className="col-3">hours</div>
                                        <div className="col-3">
                                            <Field
                                                as="select"
                                                className="form-control"
                                                id="durationMin"
                                                name="durationMin">
                                                {insertSelectOptions()}
                                            </Field>
                                        </div>
                                        <div className="col-3">minutes</div>
                                    </div>
                                    <div className="row">
                                        <div className="col-12">
                                            <div className="form-group">
                                                <label className="form-label" htmlFor="notes">Notes</label>
                                                <Field
                                                    as="textarea"
                                                    className="form-control"
                                                    id="notes"
                                                    name="notes"
                                                    placeholder="Notes..."
                                                />
                                            </div>
                                        </div>
                                    </div>
                                    <div className="row text-center">
                                        <div className="col-12">
                                            <div className="action-button-group">
                                                <button className="cancel-btn btn btn-secondary d-inline-block d-md-none">
                                                    {loading ? <span className="fa fa-spinner fa-pulse" /> :
                                                        <span>
                                                            <span className="fa fa-times fa-lg"></span>
                                                            <span>Cancel</span>
                                                        </span>}
                                                </button>
                                                <button
                                                    className="save-session-btn btn btn-primary"
                                                    type="submit">
                                                    {loading ? <span className="fa fa-spinner fa-pulse" /> :
                                                        <span>
                                                            <span className="fa fa-save fa-lg"></span>
                                                            <span>Save</span>
                                                        </span>}
                                                </button>
                                                <button
                                                    className="delete-session-btn btn btn-dark"
                                                    onClick={onDeleteClicked}>
                                                    {loading ? <span className="fa fa-spinner fa-pulse" /> :
                                                        <span>
                                                            <span className="fa fa-trash fa-lg"></span>
                                                            <span>Delete</span>
                                                        </span>}
                                                </button>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </Form>
                        </Formik>
                    </div>
                </div>
            </div>
        </div>
    );
};