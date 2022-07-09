import { useEffect, useState } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { Link, useNavigate } from 'react-router-dom';
import { Form, Formik, Field, ErrorMessage } from 'formik';
import moment from 'moment';
import * as Yup from 'yup';
import { BLUE, BLUE_GREEN, CORAL, DARK_GREY, DEFAULT_COLOR, GREEN, LONG_TEXT_MAX_CHAR, ORANGE, PICKER_DATE_DISPLAY_FORMAT, PINK, PURPLE, REQUIRED_FIELD_MSG, SHORT_TEXT_MAX_CHAR, YELLOW } from '../../../app/constants';
import { getUserIdCookie, onlyUnique, trimmedStrArray, useEffectSkipInitialRender } from '../../../app/helpers/helpers';
import { IActivity } from '../IActivity';
import { getUser } from '../../User/userSlice';
import { addActivity } from '../activityMethods';
import { clearMessage } from '../../message/messageSlice';
import { resetTimer } from '../../timer/timerSlice';

export const ActivityAdd = () => {
    const dispatch = useDispatch()
    const navigate = useNavigate();
    const [loading, setLoading] = useState(false);
    const { user: currentUser } = useSelector((state: any) => state.userData);
    const [selectedColor, setSelectedColor] = useState(DEFAULT_COLOR);
    const timerData = useSelector((state: any) => state.timer);

    useEffect(() => {
        dispatch(clearMessage());
        clearInterval(timerData.intervalId);
        dispatch(resetTimer({}));
    }, [dispatch]);

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
    });

    const initialValues = {
        name: '',
        description: '',
        tags: '2022',
        startDate: moment().format(PICKER_DATE_DISPLAY_FORMAT)
    };

    const validationSchema = Yup.object().shape({
        name: Yup.string()
            .required(REQUIRED_FIELD_MSG)
            .max(SHORT_TEXT_MAX_CHAR),
        description: Yup.string()
            .max(LONG_TEXT_MAX_CHAR),
        startDate: Yup.date()
            .required(REQUIRED_FIELD_MSG)
            .when('dueDate', {
                is: (dueDate: any) => dueDate && dueDate !== '',
                then: Yup.date().max(Yup.ref('dueDate'), 'Must be earlier than Due Date'),
            }),
        dueDate: Yup.date().min(Yup.ref('startDate'), 'Must be greater than Start Date').nullable()
    });

    /**
     * Add the activity.
     */
    const handleAdd = (formValue: any) => {
        const { name, description, startDate, dueDate, tags } = formValue;
        var activity: IActivity = {
            id: null,
            name: name,
            description: description,
            startDateUtc: startDate,
            dueDateUtc: dueDate,
            completedDateUtc: null,
            isArchived: false,
            sessions: [],
            tags: tags !== '' ? trimmedStrArray(tags
                .split(',')
                .filter((x: any) => x.length !== 0))
                .filter(onlyUnique) : [],
            colorHex: selectedColor
        }
        setLoading(true);
        dispatch<any>(addActivity(activity))
            .unwrap()
            .then(() => {
                navigate('/');
            })
            .catch(() => {
                setLoading(false);
            });
    };

    const changeColor = (e: any) => {
        e.preventDefault();
        setSelectedColor(e.target.value);
    };

    /**
     * Create the color div
     * @param {string} divColor - The name of the color for this div circle.
     * @param {string} colorClass - The class to apply to the color div.
     */
    const createColor = (divColor: string, colorClass: string) => {
        const className = `btn color-circle ${colorClass} col-1 ${selectedColor === divColor ? 'selected' : ''}`
        return <button className={className} value={divColor} onClick={changeColor}>
            {selectedColor === divColor ? <div className="fa fa-check text-center"></div> : null}
        </button>
    };

    return (
        <div className="activity-add container">
            <div className="row">
                <div className="col-2 d-none d-md-block">
                    <Link to="/">
                        <button className="btn btn-secondary">
                            <span className="fa fa-chevron-left fa-lg"></span>
                            <span>Back</span>
                        </button>
                    </Link>
                </div>
                <div className="col-12 col-md-8">
                    <div className="panel-container">
                        <h2 className="text-center colored-header" style={{ backgroundColor: selectedColor }}>New Activity</h2>
                        <Formik
                            initialValues={initialValues}
                            validationSchema={validationSchema}
                            onSubmit={handleAdd}>
                            <Form>
                                <div className="panel-body-container">
                                    <div className="row">
                                        <div className="col-12">
                                            <div className="form-group">
                                                <label className="form-label" htmlFor="name">Name<span className="mandatory-field">*</span>:</label>
                                                <Field className="form-control"
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
                                                <label className="form-label" htmlFor="description">Description:</label>
                                                <Field
                                                    as="textarea"
                                                    className="form-control"
                                                    id="description"
                                                    name="description"
                                                    placeholder="Description"
                                                />
                                                <ErrorMessage
                                                    name="description"
                                                    component="div"
                                                    className="alert alert-danger"
                                                />
                                            </div>
                                            <div className="row">
                                                <div className="col-12">
                                                    <div className="form-group">
                                                        <label htmlFor="startDate">Start Date<span className="mandatory-field">*</span></label>
                                                        <Field
                                                            className="form-control"
                                                            type="date"
                                                            id="startDate"
                                                            name="startDate"
                                                            placeholder="Start Date"
                                                        />
                                                        <ErrorMessage
                                                            name="startDate"
                                                            component="div"
                                                            className="alert alert-danger"
                                                        />
                                                    </div>
                                                </div>
                                            </div>
                                            <div className="row">
                                                <div className="col-12">
                                                    <div className="form-group">
                                                        <label htmlFor="dueDate">Due Date</label>
                                                        <span
                                                            className="info-tooltip tooltip-bubble fa fa-info-circle"
                                                            data-toggle="tooltip"
                                                            data-placement="top"
                                                            title="Must occur after Start Date.">
                                                        </span>
                                                        <Field
                                                            className="form-control"
                                                            type="date"
                                                            id="dueDate"
                                                            name="dueDate"
                                                            placeholder="Due Date"
                                                        />
                                                        <ErrorMessage
                                                            name="dueDate"
                                                            component="div"
                                                            className="alert alert-danger"
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
                                                    <label htmlFor="tags">Tags</label>
                                                    <span
                                                        className="info-tooltip tooltip-bubble fa fa-info-circle"
                                                        data-toggle="tooltip"
                                                        data-placement="top"
                                                        title="Separate tags with a comma. Beginning and ending whitespace is trimmed. Duplicates are removed.">
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
                                        </div>
                                    </div>
                                    <div className="row">
                                        <div className="col-12">
                                            <div className="form-group">
                                            </div>
                                        </div>
                                    </div>
                                    <div className="row text-center">
                                        <div className="col-12">
                                            <div className="action-button-group">
                                                {loading ? <span className="fa fa-spinner fa-pulse fa-2x" /> :
                                                    <div>
                                                        <button 
                                                            className="cancel-btn btn btn-secondary d-inline-block d-md-none"
                                                            type="button">
                                                            <span className="fa fa-times fa-lg"></span>
                                                            <span>Cancel</span>
                                                        </button>
                                                        <button
                                                            className="btn btn-primary"
                                                            type="submit">
                                                            <span>
                                                                <span className="save-activity-btn fa fa-save"></span>
                                                                <span>Save</span>
                                                            </span>
                                                        </button>
                                                    </div>
                                                }
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