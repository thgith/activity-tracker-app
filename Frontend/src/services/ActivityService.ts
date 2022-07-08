import axios from 'axios';
import moment from 'moment';
import { API_URL_BASE, MIN_DATE } from '../app/constants';
import { convertDateForApi } from '../app/helpers/helpers';
import { IActivity, IActivityEdit } from '../features/activities/IActivity';
const ACTIVITY_API_URL = `${API_URL_BASE}Activity/`;
axios.defaults.withCredentials = true;

const getAllActivitiesForUser = (userId: string) => {
    return axios.get(
        `${ACTIVITY_API_URL}?userId=${userId}`
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
            'StartDateUtc': convertDateForApi(newActivity.startDateUtc),
            'DueDateUtc': convertDateForApi(newActivity.dueDateUtc),
            'CompletedDateUtc': convertDateForApi(newActivity.completedDateUtc),
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
            'StartDateUtc': convertDateForApi(updatedActivity.startDate),
            'DueDateUtc': convertDateForApi(updatedActivity.dueDate),
            'CompletedDateUtc': convertDateForApi(updatedActivity.completedDate),
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
};