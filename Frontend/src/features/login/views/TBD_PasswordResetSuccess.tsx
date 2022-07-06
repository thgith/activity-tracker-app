export const PasswordReset = (props: any) => {

    return (
        <div className="password-reset-success container">
            <h2 className="text-center">Successfully Reset Password!</h2>
            <p>You can now log in with your new password.</p>
            <div className="row text-center">
                <div className="col-12">
                    <button className="login-btn btn btn-primary">
                        <span>Log In</span>
                    </button>
                </div>
            </div>
        </div>
    )
}