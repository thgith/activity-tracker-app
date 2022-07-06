import axios from 'axios';
import { API_URL_BASE, MIN_DATE } from '../app/constants';
import { IActivity, IActivityEdit } from '../features/activities/IActivity';
const ACTIVITY_API_URL = `${API_URL_BASE}Activity/`;
axios.defaults.withCredentials = true;

const getAllActivitiesForUser = (userId: string) => {
    return axios.get(
        `${ACTIVITY_API_URL}?userId=${userId}`, {
        headers: {
            'Referrer-Policy': 'origin-when-cross-origin',
            'Access-Control-Allow-Headers': 'Origin, Authorization, X-Requested-With, Content-Type, Accept'
        }
    }
    );
};

const getActivity = (activityId: string) => {
    return axios.get(
        `${ACTIVITY_API_URL}${activityId}`);
};

const createActivity = (newActivity: IActivity) => {
    return axios.post(
        `${ACTIVITY_API_URL}`,
        {
            'Name': newActivity.name,
            'Description': newActivity.description,
            // NOTE: We check empty string in case the user manually clears the date field (while null means they didn't fill it in).
            'StartDateUtc': newActivity.startDateUtc === '' ? MIN_DATE : newActivity.startDateUtc,
            'DueDateUtc': newActivity.dueDateUtc === '' ? MIN_DATE : newActivity.dueDateUtc,
            'CompletedDateUtc': newActivity.completedDateUtc === '' ? MIN_DATE : newActivity.completedDateUtc,
            'ColorHex': newActivity.colorHex,
            'Tags': newActivity.tags
        });
};

const editActivity = (activityId: string, updatedActivity: IActivityEdit) => {
    return axios.put(
        `${ACTIVITY_API_URL}${activityId}`,
        {
            'Name': updatedActivity.name,
            'Description': updatedActivity.description,
            // NOTE: We check empty string in case the user manually clears the date field (while null means they didn't fill it in).
            'StartDateUtc': updatedActivity.startDateUtc === '' ? MIN_DATE : updatedActivity.startDateUtc,
            'DueDateUtc': updatedActivity.dueDateUtc === '' ? MIN_DATE : updatedActivity.dueDateUtc,
            'CompletedDateUtc': updatedActivity.completedDateUtc === '' ? MIN_DATE : updatedActivity.completedDateUtc,
            'ColorHex': updatedActivity.colorHex,
            'Tags': updatedActivity.tags
        });
};

const deleteActivity = (activityId: string) => {
    return axios.delete(
        `${ACTIVITY_API_URL}${activityId}`);
};

export const ActivityService = {
    getAllActivitiesForUser,
    getActivity,
    createActivity,
    editActivity,
    deleteActivity,
}