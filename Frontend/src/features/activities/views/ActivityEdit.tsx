import { useState } from 'react'
import { useDispatch, useSelector } from 'react-redux'
import { Link, useNavigate, useParams } from 'react-router-dom'
import { Form, Formik, Field, ErrorMessage } from 'formik';
import moment from 'moment';
import * as Yup from 'yup';
import { REQUIRED_FIELD_MSG, YELLOW, ORANGE, CORAL, PINK, PURPLE, BLUE, BLUE_GREEN, DARK_GREY, GREEN, PICKER_DATE_DISPLAY_FORMAT } from '../../../app/constants'
import { getUserIdCookie, useEffectSkipInitialRender } from '../../../app/helpers/helpers'
import { IActivity, IActivityEdit } from '../IActivity'
import { getUser } from '../../User/userSlice'
import { getActivity, editActivity, deleteActivity } from '../activityMethods';
import { Loader } from '../../../app/views/Loader';

export const ActivityEdit = (props: any) => {
    // const timezone = Intl.DateTimeFormat().resolvedOptions().timeZone;
    const [loading, setLoading] = useState(false);
    const [gotActivity, setGotActivity] = useState(false);
    const navigate = useNavigate();
    const dispatch = useDispatch()
    const [activityFromGet, setActivityFromGet] = useState(null);
    const { user: currentUser } = useSelector((state: any) => state.userData);
    const { activityId } = useParams();
    const activityFromList: IActivity = useSelector((state: any) =>
        state.activitiesData ? state.activitiesData.activities.find((activity: any) => activity.id === activityId) : null
    )
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

    const [color, setColor] = useState(activity?.colorHex ?? BLUE_GREEN);

    const getInitialStartDate = () => {
        if (activity && activity.startDateUtc) {
            return formatDate(activity.startDateUtc.toString());
        }
        return null;
    }
    const getInitialDueDate = () => {
        if (activity && activity.dueDateUtc) {
            return formatDate(activity.dueDateUtc.toString());
        }
        return null;
    }
    const getInitialCompletedDate = () => {
        if (activity && activity.completedDateUtc) {
            return formatDate(activity.completedDateUtc.toString());
        }
        return null;
    }

    const formatDate = (dateItem: string | undefined) => {
        return moment(dateItem).format(PICKER_DATE_DISPLAY_FORMAT);
    }

    const initialValues = {
        name: activity ? activity.name : '',
        description: activity ? activity.description : '',
        tags: activity && activity.tags && activity.tags.length > 0 ? activity.tags.toString() : '',
        startDate: getInitialStartDate(),
        dueDate: getInitialDueDate(),
        completedDate: getInitialCompletedDate()
    };

    const validationSchema = Yup.object().shape({
        name: Yup.string()
            .required(REQUIRED_FIELD_MSG),
    });

    const onlyUnique = (value: string, index: number, self: string[]) => {
        return self.indexOf(value) === index
    }

    const trimmedStrArray = (array: string[]) => {
        for (let i = 0; i < array.length; i++) {
            array[i] = array[i].trim();
        }
        return array;
    }

    const handleEdit = (formValue: any) => {
        const { name, description, startDate, dueDate, completedDate, tags } = formValue;

        setLoading(true);
        const activityId = activity.id;
        var editedActivity: IActivityEdit = {
            name: name,
            description: description,
            startDateUtc: startDate,
            dueDateUtc: dueDate,
            completedDateUtc: completedDate,
            colorHex: color,
            isArchived: false,
            tags: tags !== '' ? trimmedStrArray(tags
                .split(',')
                .filter((x: any) => x.length !== 0))
                .filter(onlyUnique) : [],
        }
        dispatch<any>(editActivity({ activityId: activityId, updatedActivity: editedActivity }))
            .unwrap()
            .then(() => {
                navigate(`/activities/${activity.id}`);
            })
            .catch((e: any) => {
                console.log(e);
                setLoading(false);
            });
    }

    const onArchiveClicked = () => {
        console.log('TBD');
    }

    const onDeleteClicked = (e: any) => {
        e.preventDefault()
        setLoading(true);
        dispatch<any>(deleteActivity(activity.id as string))
            .unwrap()
            .then(() => {
                navigate('/');
            })
            .catch((e: any) => {
                console.log(e)
                setLoading(false);
            });
    };

    const changeColor = (e: any) => {
        e.preventDefault();
        setColor(e.target.value)
    };

    const createColor = (selectedColor: string, colorClass: string) => {
        const className = `btn color-circle ${colorClass} col-1 ${color === selectedColor ? 'selected' : ''}`
        return (
        <button className={className} value={selectedColor} onClick={changeColor}>
            {color === selectedColor ? <div className="fa fa-check text-center"></div> : null}
        </button>);
    };

    if (!activity) {
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
        );
    }

    return (
        <div className="activity-edit container">
            <div className="row">
                <div className="col-2 d-none d-md-block">
                    <Link to={`/activities/${activity.id}`}>
                        <button className="btn btn-secondary">
                            <span className="fa fa-chevron-left fa-lg"></span>
                            <span>Back</span>
                        </button>
                    </Link>
                </div>
                <div className="col-12 col-md-8">
                    <div className="panel-container">
                        <h2 className="colored-header text-center" style={{ backgroundColor: color as string }}>Edit Activity</h2>
                        <Formik
                            initialValues={initialValues}
                            validationSchema={validationSchema}
                            onSubmit={handleEdit}
                        >
                            <Form>
                                <div className="panel-body-container">
                                    <div className="row">
                                        <div className="col-12">
                                            <div className="form-group">
                                                <label htmlFor="name">Name<span className="mandatory-field">*</span></label>
                                                <Field
                                                    className="form-control"
                                                    type="text"
                                                    id="name"
                                                    name="name"
                                                    placeholder="Name"
                                                />
                                                <ErrorMessage
                                                    name="name"
                                                    component="div"
                                                    className="alert alert-danger"
                                                />
                                            </div>
                                        </div>
                                    </div>
                                    <div className="row">
                                        <div className="col-12">
                                            <div className="form-group">
                                                <label className="form-label" htmlFor="description">Description</label>
                                                <Field
                                                    as="textarea"
                                                    className="form-control"
                                                    id="description"
                                                    name="description"
                                                    placeholder="Description"
                                                />
                                            </div>
                                        </div>
                                    </div>
                                    <div className="row">
                                        <div className="col-12">
                                            <div className="form-group">
                                                <label htmlFor="startDate">Start Date</label>
                                                <Field
                                                    className="form-control"
                                                    type="date"
                                                    id="startDate"
                                                    name="startDate"
                                                    placeholder="Start Date"
                                                />
                                            </div>
                                        </div>
                                    </div>
                                    <div className="row">
                                        <div className="col-12">
                                            <div className="form-group">
                                                <label htmlFor="dueDate">Due Date</label>
                                                <Field
                                                    className="form-control"
                                                    type="date"
                                                    id="dueDate"
                                                    name="dueDate"
                                                    placeholder="Due Date"
                                                />
                                            </div>
                                        </div>
                                    </div>
                                    <div className="row">
                                        <div className="col-12">
                                            <div className="form-group">
                                                <label htmlFor="completedDate">Completed Date</label>
                                                <Field
                                                    className="form-control"
                                                    type="date"
                                                    id="completedDate"
                                                    name="completedDate"
                                                    placeholder="Completed Date"
                                                />
                                            </div>
                                        </div>
                                    </div>
                                    <div className="row">
                                        <div className="col-12">
                                            <label>Color</label>
                                            <div role="group" className="color-picker row text-center">
                                                {createColor(YELLOW, 'color-yellow')}
                                                {createColor(ORANGE, 'color-orange')}
                                                {createColor(CORAL, 'color-red')}
                                                {createColor(PINK, 'color-pink')}
                                                {createColor(PURPLE, 'color-purple')}
                                                {createColor(BLUE, 'color-blue')}
                                                {createColor(BLUE_GREEN, 'color-blue-green')}
                                                {createColor(GREEN, 'color-green')}
                                                {createColor(DARK_GREY, 'color-grey')}
                                                {createColor('black', 'color-black')}
                                            </div>
                                        </div>
                                    </div>
                                    <div className="row">
                                        <div className="col-12">
                                            <label>Tags</label>
                                            <span
                                                className="fa fa-info"
                                                data-toggle="tooltip"
                                                data-placement="top"
                                                title="Separate tags with a comma. Beginning and ending whitespace is trimmed. Duplicates are removed">
                                            </span>
                                            <Field
                                                className="form-control"
                                                type="text"
                                                id="tags"
                                                name="tags"
                                                placeholder="Tags"
                                            >
                                            </Field>
                                        </div>
                                    </div>
                                    <div className="row text-center">
                                        <div className="col-12">
                                            <div className="action-button-group">
                                                <button
                                                    className="btn btn-primary"
                                                    type="submit">
                                                    <span className="save-activity-btn fa fa-save fa-lg"></span>
                                                    <span>Save</span>
                                                </button>
                                                <button
                                                    className="btn btn-secondary">
                                                    <span className="archive-activity-btn fa fa-archive fa-lg"></span>
                                                    <span>Archive</span>
                                                </button>
                                                <button
                                                    className="btn btn-dark"
                                                    type="button"
                                                    onClick={onDeleteClicked}>
                                                    {loading ? <span className="fa fa-spinner fa-pulse" /> :
                                                        <span>
                                                            <span className="delete-activity-btn fa fa-trash fa-lg"></span>
                                                            <span>Delete</span>
                                                        </span>
                                                    }
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