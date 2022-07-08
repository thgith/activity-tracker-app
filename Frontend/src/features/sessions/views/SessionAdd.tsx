import { useEffect, useState } from 'react'
import { useDispatch, useSelector } from 'react-redux'
import { Link, useNavigate, useSearchParams } from 'react-router-dom'
import { Form, Formik, Field, ErrorMessage } from 'formik';
import moment from 'moment'
import * as Yup from 'yup';
import { DEFAULT_COLOR, LONG_TEXT_MAX_CHAR, PICKER_DATE_DISPLAY_FORMAT, REQUIRED_FIELD_MSG } from '../../../app/constants'
import { getUserIdCookie, useEffectSkipInitialRender } from '../../../app/helpers/helpers'
import { ISessionNew } from '../ISession'
import { getUser } from '../../User/userSlice'
import { IActivity } from '../../activities/IActivity';
import { getActivity } from '../../activities/activityMethods';
import { addSession } from '../sessionMethods';
import { Loader } from '../../../app/views/Loader';
import { clearMessage } from '../../message/messageSlice';
import { resetTimer } from '../../timer/timerSlice';


export const SessionAdd = () => {
    const dispatch = useDispatch()
    const navigate = useNavigate();
    const [searchParams] = useSearchParams();
    const activityId = searchParams.get('activityId');
    const [loading, setLoading] = useState(false);
    const { user: currentUser } = useSelector((state: any) => state.userData);
    const [activityFromGet, setActivityFromGet] = useState(null);
    const [gotActivity, setGotActivity] = useState(false);
    const timerData = useSelector((state: any) => state.timer);

    useEffect(() => {
        dispatch(clearMessage());
        clearInterval(timerData.intervalId);
        dispatch(resetTimer({}));
    }, [dispatch]);

    const activityFromList: IActivity = useSelector((state: any) =>
        state.activitiesData ? state.activitiesData.activities.find((activity: any) => activity.id === activityId) : null
    );
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

    const initialValues = {
        notes: '',
        startDateOnly: moment().format(PICKER_DATE_DISPLAY_FORMAT),
        startTimeOnly: moment().format('HH:mm'),
        durationHours: 0,
        durationMin: 0
    };

    const validationSchema = Yup.object().shape({
        startDateOnly: Yup.string()
            .required(REQUIRED_FIELD_MSG),
        notes: Yup.string()
            .max(LONG_TEXT_MAX_CHAR)
    });

    const combineDateAndTime = (dateVal: string, timeVal: string) => {
        var dateDt = moment(dateVal).format(PICKER_DATE_DISPLAY_FORMAT);
        var timeDt = moment(timeVal, ["h:mm A"]).format("HH:mm");
        let date = new Date(dateDt + ' ' + timeDt);
        return date;
    };

    const convertToDurationSeconds = (hours: number, minutes: number) => {
        return hours * 3600 + minutes * 60;
    };

    const handleAdd = (formValue: any) => {
        console.log('handling add');
        const { startDateOnly, startTimeOnly, durationHours, durationMin, notes } = formValue;
        var session: ISessionNew = {
            activityId: activityId,
            startDate: combineDateAndTime(startDateOnly, startTimeOnly),
            durationSeconds: convertToDurationSeconds(durationHours, durationMin),
            notes: notes
        }
        setLoading(true);
        dispatch<any>(addSession(session))
            .unwrap()
            .then(() => {
                navigate(`/activities/${session.activityId}`);
            })
            .catch((e: any) => {
                console.log(e);
                setLoading(false);
            });
    };

    const insertSelectOptions = () => {
        const selects = [];
        for (let i = 0; i < 60; i++)
            selects.push(<option value={i} key={i}>{i}</option>)

        return selects;
    };

    if (!activity && !gotActivity) {
        return <Loader />
    }

    return (
        <div className="session-add container">
            <div className="row">
                <div className="col-2 d-none d-md-block">
                    <Link to={`/activities/${activityId}`}>
                        <button className="btn btn-secondary">
                            <span className="fa fa-chevron-left fa-lg"></span>
                            <span>Back</span>
                        </button>
                    </Link>
                </div>
                <div className="col-12 col-md-8">
                    <div className="panel-container">
                        <h2 className="colored-header text-center" style={{ backgroundColor: activity?.colorHex ?? DEFAULT_COLOR }}>New Session</h2>
                        <Formik
                            initialValues={initialValues}
                            validationSchema={validationSchema}
                            onSubmit={handleAdd}
                        >
                            <Form>
                                <div className="panel-body-container">
                                    <div className="form-group row">
                                        <label
                                            className="form-label"
                                            htmlFor="startTime">Started At
                                            <span className="mandatory-field">*</span>
                                        </label>
                                        <div className="col-6">
                                            <Field
                                                type="date"
                                                className="form-control"
                                                id="startDateOnly"
                                                name="startDateOnly"
                                            />
                                            <ErrorMessage
                                                name="startDateOnly"
                                                component="div"
                                                className="alert alert-danger"
                                            />
                                        </div>
                                        <div className="col-6">
                                            <Field
                                                type="time"
                                                className="form-control"
                                                id="startTimeOnly"
                                                name="startTimeOnly"
                                            />
                                        </div>
                                    </div>
                                    <div className="form-group row">
                                        <label
                                            className="form-label"
                                            htmlFor="duration">Duration</label>
                                        <div className="col-3">
                                            <Field
                                                as="select"
                                                className="form-control"
                                                id="durationHours"
                                                name="durationHours"
                                            >
                                                {insertSelectOptions()}
                                            </Field>
                                        </div>
                                        <div className="col-3">hours</div>
                                        <div className="col-3">
                                            <Field
                                                as="select"
                                                className="form-control"
                                                id="durationMin"
                                                name="durationMin"
                                                placeholder="Duration Minutes"
                                            >
                                                {insertSelectOptions()}
                                            </Field>
                                        </div>
                                        <div className="col-3">minutes</div>
                                    </div>
                                    <div className="row">
                                        <div className="col-12">
                                            <div className="form-group">
                                                <label
                                                    className="form-label"
                                                    htmlFor="notes">Notes</label>
                                                <Field
                                                    as="textarea"
                                                    className="form-control"
                                                    id="notes"
                                                    name="notes"
                                                    placeholder="Notes"
                                                />
                                                <ErrorMessage
                                                    name="notes"
                                                    component="div"
                                                    className="alert alert-danger"
                                                />
                                            </div>
                                        </div>
                                    </div>
                                    <div className="row text-center">
                                        <div className="col-12">
                                            <div className="action-button-group">
                                                <Link to={`/activities/${activityId}`}>
                                                    <button
                                                        className="cancel-btn btn btn-secondary d-inline-block d-md-none">
                                                        <span className="fa fa-times fa-lg"></span>
                                                        <span>Cancel</span>
                                                    </button>
                                                </Link>
                                                <button
                                                    className="save-session-btn btn btn-primary"
                                                    type="submit">
                                                    <span className="fa fa-save fa-lg"></span>
                                                    <span>Save</span>
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
    )
};