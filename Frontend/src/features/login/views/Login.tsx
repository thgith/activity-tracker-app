import { useEffect, useState } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { Link, useNavigate } from 'react-router-dom';
import { Formik, Field, Form, ErrorMessage } from 'formik';
import * as Yup from 'yup';
import { REQUIRED_FIELD_MSG } from '../../../app/constants';
import { clearMessage } from '../../message/messageSlice';
import { getUserIdCookie } from '../../../app/helpers/helpers';
import { logIn } from '../../User/userSlice';
import { resetTimer } from '../../timer/timerSlice';

export const Login = (props: any) => {
    const dispatch = useDispatch();
    const navigate = useNavigate();
    const [loading, setLoading] = useState(false);
    const { message } = useSelector((state: any) => state.message);
    const timerData = useSelector((state: any) => state.timer);

    useEffect(() => {
        dispatch(clearMessage());
        if (timerData.intervalId) {
            clearInterval(timerData.intervalId);
            dispatch(resetTimer({}));
        }
    }, [dispatch, timerData.intervalId]);

    const initialValues = {
        username: '',
        password: '',
    };

    const validationSchema = Yup.object().shape({
        email: Yup.string()
            .email('This is not a valid email.')
            .required(REQUIRED_FIELD_MSG),
        password: Yup.string()
            .test(
                'len',
                'The password should be between 8 and 40 characters.',
                (val: any) =>
                    val &&
                    val.toString().length >= 8 &&
                    val.toString().length <= 40
            )
            .required(REQUIRED_FIELD_MSG),
    });

    const handleLogin = (formValue: any) => {
        const { email, password } = formValue;
        setLoading(true);
        dispatch<any>(logIn({ email, password }))
            .unwrap()
            .then(() => {
                props.history.push("/");
                window.location.reload();
            })
            .catch(() => {
                setLoading(false);
            });
    };

    // If a user is already logged in, just navigate them to the list page
    if (getUserIdCookie()) {
        navigate('/');
    }

    return (
        <div className="login-view container">
            <div className="panel-container">
                <h2 className="colored-header text-center">Log In</h2>
                <Formik
                    initialValues={initialValues}
                    validationSchema={validationSchema}
                    onSubmit={handleLogin}>
                    <Form>
                        <div className="panel-body-container">
                            <div className="text-center">
                                <span className="fa fa-user-circle fa-5x text-center"></span>
                            </div>
                            <div className="row">
                                <div className="col-12">
                                    <label htmlFor="email">Email</label>
                                    <Field
                                        className="form-control"
                                        id="email"
                                        name="email"
                                        type="email"
                                        placeholder="Enter email" />
                                    <ErrorMessage
                                        name="email"
                                        component="div"
                                        className="alert alert-danger"
                                    />
                                </div>
                            </div>
                            <div className="row">
                                <div className="col-12">
                                    <label htmlFor="password">Password</label>
                                    <Field
                                        className="form-control"
                                        id="password"
                                        name="password"
                                        type="password"
                                        placeholder="Enter password" />
                                    <ErrorMessage
                                        name="password"
                                        component="div"
                                        className="alert alert-danger"
                                    />
                                </div>
                            </div>
                            <div className="row text-center">
                                <div className="col-12">
                                    <div className="action-button-group">
                                        {loading ? <span className="fa fa-spinner fa-pulse fa-2x" /> :

                                            <button type="submit" className="btn btn-primary">Log In</button>
                                        }
                                    </div>
                                </div>
                            </div>
                            <hr></hr>
                            <div className="row text-center">
                                <div className="col-12">
                                    {/* <Link to="/resetPassword">Forgot password?</Link><br></br> */}
                                    <span>New? </span><Link to="/register">Register here</Link>
                                </div>
                            </div>
                        </div>
                    </Form>
                </Formik>
                {message && (
                    <div className="form-group">
                        <div className="alert alert-danger" role="alert">
                            {message}
                        </div>
                    </div>
                )}
            </div>
        </div>
    );
};