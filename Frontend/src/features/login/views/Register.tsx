import { useEffect, useState } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { Link, useNavigate } from 'react-router-dom';
import { Formik, Field, Form, ErrorMessage } from 'formik';
import * as Yup from 'yup';
import { REQUIRED_FIELD_MSG } from '../../../app/constants';
import { clearMessage } from '../../message/messageSlice';
import { register } from '../../User/userSlice';
import { resetTimer } from '../../timer/timerSlice';

const Register = () => {
    const dispatch = useDispatch();
    const navigate = useNavigate();
    const [successful, setSuccessful] = useState(false);
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
        firstName: '',
        lastName: '',
        email: '',
        password: ''
    }

    const validationSchema = Yup.object().shape({
        firstName: Yup.string()
            .required(REQUIRED_FIELD_MSG),
        lastName: Yup.string()
            .required(REQUIRED_FIELD_MSG),
        email: Yup.string()
            .email('This is not a valid email.')
            .required(REQUIRED_FIELD_MSG),
        password: Yup.string()
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
        passwordConfirm: Yup.string()
            .oneOf([Yup.ref('password')], 'Passwords must match.')
            .required(REQUIRED_FIELD_MSG),
    });

    const handleRegister = (formValue: any) => {
        const { firstName, lastName, email, password } = formValue;

        setSuccessful(false);
        dispatch<any>(register({ firstName, lastName, email, password }))
            .unwrap()
            .then(() => {
                setSuccessful(true);
                window.setTimeout(function(){
                    navigate('/');
                }, 3000);
            })
            .catch(() => {
                setSuccessful(false);
            });
    };

    return (
        <div className="register-view container">
            <div className="panel-container">
                <h2 className="colored-header text-center">Register</h2>
                <div className="panel-body-container">
                    <Formik
                        initialValues={initialValues}
                        validationSchema={validationSchema}
                        onSubmit={handleRegister}
                    >
                        <Form>
                            {!successful && (
                                <div>
                                    <div className="row">
                                        <div className="col-12">
                                            <div className="form-group">
                                                <label htmlFor="firstName">First Name</label>
                                                <Field
                                                    type="text"
                                                    className="form-control"
                                                    id="firstName"
                                                    name="firstName"
                                                    placeholder="First Name"
                                                    required />
                                                <ErrorMessage
                                                    name="firstName"
                                                    component="div"
                                                    className="alert alert-danger"
                                                />
                                            </div>
                                        </div>
                                    </div>
                                    <div className="row">
                                        <div className="col-12">
                                            <div className="form-group">
                                                <label htmlFor="lastName">Last Name</label>
                                                <Field
                                                    type="text"
                                                    className="form-control"
                                                    id="lastName"
                                                    name="lastName"
                                                    placeholder="Last Name"
                                                    required />
                                                <ErrorMessage
                                                    name="lastName"
                                                    component="div"
                                                    className="alert alert-danger"
                                                />
                                            </div>
                                        </div>
                                    </div>
                                    <div className="row">
                                        <div className="col-12">
                                            <div className="form-group">
                                                <label htmlFor="email">Email</label>
                                                <Field
                                                    type="text"
                                                    className="form-control"
                                                    id="email"
                                                    name="email"
                                                    placeholder="Email"
                                                    required />
                                                <ErrorMessage
                                                    name="email"
                                                    component="div"
                                                    className="alert alert-danger"
                                                />
                                            </div>
                                        </div>
                                    </div>
                                    <div className="row">
                                        <div className="col-12">
                                            <div className="form-group">
                                                <label htmlFor="password">Password</label>
                                                <Field
                                                    type="password"
                                                    className="form-control"
                                                    id="password"
                                                    name="password"
                                                    placeholder="Password"
                                                    required />
                                                <ErrorMessage
                                                    name="password"
                                                    component="div"
                                                    className="alert alert-danger"
                                                />
                                            </div>
                                        </div>
                                    </div>
                                    <div className="row">
                                        <div className="col-12">
                                            <div className="form-group">
                                                <label htmlFor="passwordConfirm">Confirm Password</label>
                                                <Field
                                                    type="password"
                                                    className="form-control"
                                                    id="passwordConfirm"
                                                    name="passwordConfirm"
                                                    placeholder="Confirm Password"
                                                    required />
                                                <ErrorMessage
                                                    name="passwordConfirm"
                                                    component="div"
                                                    className="alert alert-danger"
                                                />
                                            </div>
                                        </div>
                                    </div>
                                    <div className="row text-center">
                                        <div className="col-12">
                                            <div className="action-button-group">
                                                <button
                                                    type="submit"
                                                    className="register-btn btn btn-primary">Register</button>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            )}
                        </Form>
                    </Formik>
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
                    <hr></hr>
                    <div className="row text-center">
                        <div className="col-12">
                            Already a member? <Link to="/login">Log In</Link>
                        </div>
                    </div>
                </div>
            </div>
        </div >
    );
};

export default Register;