import axios from 'axios';
import moment from 'moment';
import { API_URL_BASE } from '../app/constants';
import { convertDateForApi } from '../app/helpers/helpers';
import { ISession, ISessionEdit, ISessionNew } from '../features/sessions/ISession';
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
            'StartDateUtc': convertDateForApi(newSession.startDate),
            'DurationSeconds': newSession.durationSeconds,
            'Notes': newSession.notes
        });
};

const updateSession = (updatedSession: ISessionEdit) => {
    return axios.put(
        `${SESSION_API_URL}${updatedSession.id}`,
        {
            'StartDateUtc': convertDateForApi(updatedSession.startDate),
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