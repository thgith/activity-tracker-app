import { createAsyncThunk } from '@reduxjs/toolkit';
import { ActivityService } from '../../services/ActivityService';
import { setMessage } from '../message/messageSlice';
import { IActivity } from './IActivity';

export const listActivities = createAsyncThunk(
    'activity/list',
    async (userId: string, thunkAPI) => {
        try {
            const response: any = await ActivityService.getAllActivitiesForUser(userId);
            return { activities: response.data };
        } catch (error: any) {
            const message =
                (error.response && error.response.data && error.response.data.detail) ||
                error.message ||
                error.toString();
            thunkAPI.dispatch(setMessage(message));
            return thunkAPI.rejectWithValue(message);
        }
    }
);

export const getActivity = createAsyncThunk(
    'activity/get',
    async (activityId: string, thunkAPI) => {
        try {
            const response: any = await ActivityService.getActivity(activityId);
            return { activity: response.data };
        } catch (error: any) {
            const message =
                (error.response && error.response.data && error.response.data.detail) ||
                error.message ||
                error.toString();
            thunkAPI.dispatch(setMessage(message));
            return thunkAPI.rejectWithValue(message);
        }
    }
);

export const addActivity = createAsyncThunk(
    'activity/add',
    async (newActivity: IActivity, thunkAPI) => {
        try {
            const response: any = await ActivityService.createActivity(newActivity);
            return { activity: response.data };
        } catch (error: any) {
            const message =
                (error.response && error.response.data && error.response.data.detail) ||
                error.message ||
                error.toString();
            thunkAPI.dispatch(setMessage(message));
            return thunkAPI.rejectWithValue(message);
        }
    }
);

export const editActivity = createAsyncThunk(
    'activity/edit',
    async ({ activityId, updatedActivity }: any, thunkAPI) => {
        try {
            const response: any = await ActivityService.editActivity(activityId, updatedActivity);
            return { activity: response.data };
        } catch (error: any) {
            const message =
                (error.response && error.response.data && error.response.data.detail) ||
                error.message ||
                error.toString();
            thunkAPI.dispatch(setMessage(message));
            return thunkAPI.rejectWithValue(message);
        }
    }
);

export const deleteActivity = createAsyncThunk(
    'activity/delete',
    async (activityId: string, thunkAPI) => {
        try {
            const data: any = await ActivityService.deleteActivity(activityId);
            return { activityId: activityId };
        } catch (error: any) {
            const message =
                (error.response && error.response.data && error.response.data.detail) ||
                error.message ||
                error.toString();
            thunkAPI.dispatch(setMessage(message));
            return thunkAPI.rejectWithValue(message);
        }
    }
);