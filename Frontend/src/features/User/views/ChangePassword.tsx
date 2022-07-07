import { useEffect, useState } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { Link, useNavigate } from 'react-router-dom';
import { ErrorMessage, Field, Form, Formik } from 'formik';
import * as Yup from 'yup';
import { REQUIRED_FIELD_MSG } from '../../../app/constants';
import { getUserIdCookie, useEffectSkipInitialRender } from '../../../app/helpers/helpers';
import { Loader } from '../../../app/views/Loader';
import { changePassword, getUser } from '../userSlice';
import { clearMessage } from '../../message/messageSlice';
import { resetTimer } from '../../timer/timerSlice';

export const ChangePassword = (props: any) => {
    const navigate = useNavigate();
    const dispatch = useDispatch()
    const [loading, setLoading] = useState(false);
    const [successful, setSuccessful] = useState(false);
    const timerData = useSelector((state: any) => state.timer);
    const { message } = useSelector((state: any) => state.message);
    const { user: currentUser } = useSelector((state: any) => state.userData);

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
            // Try to get user if not gotten yet
            dispatch<any>(getUser(currUserId))
                .unwrap()
                .then(() => {
                })
                .catch(() => {
                });
        }
    });

    const initialValues = {
        oldPassword: '',
        newPassword: '',
        confirmNewPassword: ''
    };

    const validationSchema = Yup.object().shape({
        oldPassword: Yup.string()
            .test(
                'len',
                'The password must be between 8 and 40 characters.',
                (val: any) =>
                    val &&
                    val.toString().length >= 8 &&
                    val.toString().length <= 40
            )
            .required(REQUIRED_FIELD_MSG),
        newPassword: Yup.string()
            .test(
                'len',
                'The password must be between 8 and 40 characters.',
                (val: any) =>
                    val &&
                    val.toString().length >= 8 &&
                    val.toString().length <= 40
            )
            .required(REQUIRED_FIELD_MSG),
        // TODO: update to strengthen password requirement
        // password: Yup
        //     .string()
        //     .matches(
        //         /^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[!@#\$%\^&\*])(?=.{8,40})/,
        //         "Must be between 8 and 40 characters. Must contain one uppercase, one lowercase, one number, and one special character."
        //     ),
        confirmNewPassword: Yup.string()
            .oneOf([Yup.ref('newPassword')], 'Passwords must match.')
            .required(REQUIRED_FIELD_MSG),
    });

    const handleChangePassword = (formValue: any) => {
        const { oldPassword, newPassword } = formValue;
        setLoading(true);
        dispatch<any>(changePassword(
            {
                'userId': currentUser.id,
                'email': currentUser.email,
                oldPassword,
                newPassword
            }))
            .unwrap()
            .then(() => {
                console.log('Success changed password!');
                setSuccessful(true);
                window.setTimeout(function () {
                    navigate(`/profile/${currentUser.id}`);
                }, 3000);

            })
            .catch((e: any) => {
                console.log(e);
                setSuccessful(false);
                setLoading(false);
            });
    };

    if (!currentUser) {
        return <Loader />
    }

    return (
        <div className="profile-view container">
            <div className="panel-container">
                <h2 className="colored-header text-center">Change {currentUser.firstName}'s Password</h2>
                <Formik
                    initialValues={initialValues}
                    validationSchema={validationSchema}
                    onSubmit={handleChangePassword}
                >
                    <Form>
                        {!successful && (<div className="panel-body-container">
                            <div className="row text-center">
                                <span className="fa fa-user-circle fa-5x"></span>
                            </div>
                            <div className="row">
                                <div className="col-12">
                                    <div className="form-group">
                                        <label htmlFor="oldPassword">Old Password</label>
                                        <Field
                                            type="password"
                                            className="form-control"
                                            id="oldPassword"
                                            name="oldPassword"
                                            placeholder="Enter old password"></Field>
                                        <ErrorMessage
                                            name="oldPassword"
                                            component="div"
                                            className="alert alert-danger"
                                        />
                                    </div>
                                </div>
                            </div>
                            <div className="row">
                                <div className="col-12">
                                    <div className="form-group">
                                        <label htmlFor="lastName">New Password</label>
                                        <Field
                                            className="form-control"
                                            type="password"
                                            id="newPassword"
                                            name="newPassword"
                                            placeholder="Enter new password"></Field>
                                        <ErrorMessage
                                            name="newPassword"
                                            component="div"
                                            className="alert alert-danger"
                                        />
                                    </div>
                                </div>
                            </div>
                            <div className="row">
                                <div className="col-12">
                                    <div className="form-group">
                                        <label htmlFor="confirmNewPassword">Confirm New Password</label>
                                        <Field
                                            className="form-control"
                                            type="password"
                                            id="confirmNewPassword"
                                            name="confirmNewPassword"
                                            placeholder="Confirm new password"></Field>
                                        <ErrorMessage
                                            name="confirmNewPassword"
                                            component="div"
                                            className="alert alert-danger"
                                        />
                                    </div>
                                </div>
                            </div>
                            <div className="row text-center">
                                <div className="col-12">
                                    <div className="action-button-group">
                                        {loading ? <span className="fa fa-spinner fa-pulse fa-2x" /> :
                                            <div>
                                                <Link to={`/profile/${currentUser.id}`}>
                                                    <button type="button" className="btn btn-secondary">
                                                        <span className="fa fa-times fa-lg"></span>
                                                        <span>Cancel</span>
                                                    </button>
                                                </Link>
                                                <button type="submit" className="btn btn-primary">
                                                    <span className="fa fa-save fa-lg"></span>
                                                    <span>Change Password</span>
                                                </button>
                                            </div>
                                        }
                                    </div>
                                </div>
                            </div>
                        </div>)}
                        {message && (
                            <div className="form-group text-center form-message">
                                <div
                                    className={successful ? "alert alert-success" : "alert alert-danger"}
                                    role="alert"
                                >
                                    {message}
                                </div>
                            </div>
                        )}
                    </Form>
                </Formik>
            </div>
        </div>
    );
};