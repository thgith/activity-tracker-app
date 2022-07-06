import React from 'react'

export const PasswordReset = (props: any) => {

    return (
        <div className="password-reset-view container">
            <h2 className="text-center">Reset Password</h2>
            <form className="form">
                <div className="row">
                    <div className="col-12">
                        <label>New Password</label>
                        <input
                            className="form-control"
                            type="text"
                            id="password"
                            name="password"
                            placeholder="Enter a new password"
                        ></input>
                    </div>
                </div>
                <div className="row">
                    <div className="col-12">
                        <label>Confirm New Password</label>
                        <input
                            className="form-control"
                            type="text"
                            id="confirmPassword"
                            name="confirmPassword"
                            placeholder="Confirm your new password"
                        ></input>
                    </div>
                </div>
                <div className="row text-center">
                    <div className="col-12">
                        <button className="change-password-btn btn btn-primary">
                            <span className="fa fa-envelope"></span>
                            <span>Change Password</span>
                        </button>
                    </div>
                </div>
            </form>
        </div>
    )
}