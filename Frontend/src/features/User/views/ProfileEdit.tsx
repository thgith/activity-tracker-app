import { Field, Form, Formik } from 'formik';
import { useEffect, useState } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { Link, useNavigate } from 'react-router-dom';
import * as Yup from 'yup';
import { REQUIRED_FIELD_MSG } from '../../../app/constants';
import { getUserIdCookie, useEffectSkipInitialRender } from '../../../app/helpers/helpers';
import { Loader } from '../../../app/views/Loader';
import { clearMessage } from '../../message/messageSlice';
import { resetTimer } from '../../timer/timerSlice';
import { IUserUpdate } from '../IUser';
import { getUser, updateUser } from '../userSlice';

export const ProfileEdit = (props: any) => {
    const navigate = useNavigate();
    const dispatch = useDispatch()
    const [loading, setLoading] = useState(false);
    const timerData = useSelector((state: any) => state.timer);

    const { user: currentUser } = useSelector((state: any) => state.userData);

    useEffect(() => {
        dispatch(clearMessage());
        if (timerData.intervalId) {
            clearInterval(timerData.intervalId);
            dispatch(resetTimer({}));
        }
    });

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

    if (!currentUser) {
        return <Loader />
    }

    const initialValues = {
        firstName: currentUser.firstName,
        lastName: currentUser.lastName,
        email: currentUser.email,
    };

    const validationSchema = Yup.object().shape({
        firstName: Yup.string()
            .required(REQUIRED_FIELD_MSG),
        lastName: Yup.string()
            .required(REQUIRED_FIELD_MSG),
        email: Yup.string()
            .required(REQUIRED_FIELD_MSG),
    });

    /**
     * Edit the user.
     * @param formValue - The values from the form.
     */
    const handleEdit = (formValue: any) => {
        const { firstName, lastName, email } = formValue;
        var editedUser: IUserUpdate = {
            id: currentUser.id,
            firstName: firstName,
            lastName: lastName,
            email: email
        }
        setLoading(true);
        dispatch<any>(updateUser(editedUser))
            .unwrap()
            .then(() => {
                navigate(`/profile/${currentUser.id}`);
            })
            .catch((e: any) => {
                console.log(e);
                setLoading(false);
            });
    };

    return (
        <div className="profile-edit container">
            <div className="panel-container">
                <h2 className="colored-header text-center">Edit {currentUser.firstName}'s Profile</h2>
                <Formik
                    initialValues={initialValues}
                    validationSchema={validationSchema}
                    onSubmit={handleEdit}
                >
                    <Form>
                        <div className="panel-body-container">
                            <div className="row text-center">
                                <span className="fa fa-user-circle fa-5x"></span>
                            </div>
                            <div className="row">
                                <div className="col-6">
                                    <div className="form-group">
                                        <label htmlFor="firstName">First Name</label>
                                        <Field
                                            className="form-control"
                                            id="firstName"
                                            name="firstName"
                                            placeholder="First Name"></Field>
                                    </div>
                                </div>
                                <div className="col-6">
                                    <div className="form-group">
                                        <label htmlFor="lastName">Last Name</label>
                                        <Field
                                            className="form-control"
                                            id="lastName"
                                            name="lastName"
                                            placeholder="Last Name"></Field>
                                    </div>
                                </div>
                            </div>
                            <div className="row">
                                <div className="col-12">
                                    <div className="form-group">
                                        <label htmlFor="email">Email</label>
                                        <Field
                                            className="form-control"
                                            id="email"
                                            name="email"
                                            placeholder="Email"></Field>
                                    </div>
                                </div>
                            </div>
                            <div className="row text-center">
                                <div className="col-12">
                                    <div className="action-button-group">
                                        {loading ?
                                            <span className="fa fa-spinner fa-pulse fa-2x"></span> :
                                            <div>
                                                <Link to={`/profile/${currentUser.id}`}>
                                                    <button type="button" className="btn btn-secondary">
                                                        <span className="fa fa-times fa-lg"></span>
                                                        <span>Cancel</span>
                                                    </button>
                                                </Link>
                                                <button type="submit" className="btn btn-primary">
                                                    <span>
                                                        <span className="fa fa-save fa-lg"></span>
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
    );
};