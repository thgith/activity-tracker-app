import axios from 'axios';
import { API_URL_BASE } from '../app/constants';
import { ISession, ISessionNew } from '../features/sessions/ISession';
const SESSION_API_URL = `${API_URL_BASE}Session/`;
axios.defaults.withCredentials = true;

const getAllSessionsForActivity = (activityId: string) => {
    return axios.get(
        `${SESSION_API_URL}?activityId=${activityId}`);
};

const getSession = (activityId: string) => {
    return axios.get(
        `${SESSION_API_URL}${activityId}`);
};

const createSession = (newSession: ISessionNew) => {
    return axios.post(
        `${SESSION_API_URL}`,
        {
            'ActivityId': newSession.activityId,
            'StartDateUtc': newSession.startDateUtc,
            'DurationSeconds': newSession.durationSeconds,
            'Notes': newSession.notes
        });
};

const updateSession = (updatedSession: ISession) => {
    return axios.put(
        `${SESSION_API_URL}${updatedSession.id}`,
        {
            'StartDateUtc': updatedSession.startDateUtc,
            'DurationSeconds': updatedSession.durationSeconds,
            'Notes': updatedSession.notes
        });
};

const deleteSession = (sessionId: string) => {
    return axios.delete(
        `${SESSION_API_URL}${sessionId}`);
};

export const SessionService = {
    getAllSessionsForActivity,
    getSession,
    createSession,
    updateSession,
    deleteSession,
}