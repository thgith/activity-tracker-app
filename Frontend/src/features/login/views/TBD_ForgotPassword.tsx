import { Link } from 'react-router-dom'

export const ForgotPassword = (props: any) => {

    return (
        <div className="forgot-password container">
            <div className="panel-container">
                <h2 className="colored-header text-center">Forgot Password</h2>
                <div className="panel-body-container">
                    <p className="text-center">Enter the email associated with Activity Tracker and we'll send you instructions to reset your password</p>
                    <div className="row">
                        <div className="col-12">
                            <input
                                className="form-control"
                                type="text"
                                id="email"
                                name="email"
                                placeholder="Email"
                            ></input>
                        </div>
                    </div>
                    <div className="row text-center">
                        <div className="action-button-group">
                            <Link to="/login">
                                <button className="btn btn-secondary">
                                    <span className="fa fa-times"></span>
                                    <span>Return to Login</span>
                                </button>
                            </Link>
                            <button className="btn btn-primary">
                                <span className="fa fa-envelope"></span>
                                <span>Request Reset</span>
                            </button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    )
}