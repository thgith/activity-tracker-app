import React from 'react';
import {
    BrowserRouter,
    Routes,
    Route,
    Navigate
} from 'react-router-dom';
import 'bootstrap/dist/css/bootstrap.min.css';
import 'bootstrap/dist/js/bootstrap.min.js';
import 'font-awesome/css/font-awesome.min.css';
import './styles/App.scss';

import { ActivityAdd } from './features/activities/views/ActivityAdd';
import { ActivityEdit } from './features/activities/views/ActivityEdit';
import { ActivitiesList } from './features/activities/views/ActivitiesList';
import { ActivityDetails as ActivityDetails } from './features/activities/views/ActivityDetails';
import { SessionDetails } from './features/sessions/views/SessionDetails';
import { SessionEdit } from './features/sessions/views/SessionEdit';
import { SessionAdd } from './features/sessions/views/SessionAdd';
import { Navbar } from './app/views/Navbar'
import { Profile } from './features/User/views/Profile';
import { ForgotPassword } from './features/login/views/TBD_ForgotPassword';
import { Stats } from './features/stats/Stats';
import { Login } from './features/login/views/Login';
import Register from './features/login/views/Register';
import { ProfileEdit } from './features/User/views/ProfileEdit';
import { NotFound } from './app/views/NotFound';

function App() {
    return (
        <BrowserRouter>
            <Navbar />
            <div className="App">
                <Routes>
                    <Route path="/"
                        element={
                            <React.Fragment>
                                <ActivitiesList />
                            </React.Fragment>} />
                    <Route path="/login" element={<Login />}></Route>
                    <Route path="/resetPassword" element={<ForgotPassword />}></Route>
                    <Route path="/register" element={<Register />}></Route>
                    <Route path="/stats" element={<Stats />}></Route>

                    <Route path="/activities/new" element={<ActivityAdd />}></Route>
                    <Route path="/activities/:activityId" element={<ActivityDetails />}></Route>
                    <Route path="/activities/:activityId/edit" element={<ActivityEdit />}></Route>

                    <Route path="/sessions/new" element={<SessionAdd />}></Route>
                    <Route path="/sessions/:sessionId" element={<SessionDetails />}></Route>
                    <Route path="/sessions/:sessionId/edit" element={<SessionEdit />}></Route>

                    <Route path="/profile/:userId" element={<Profile />}></Route>
                    <Route path="/profile/:userId/edit" element={<ProfileEdit />}></Route>
                    <Route path="/404" element={<NotFound />}></Route>
                    <Route
                        path="*"
                        element={<Navigate to="/404" replace />}
                    />
                </Routes>
            </div>
        </BrowserRouter>
    );
}

export default App;
