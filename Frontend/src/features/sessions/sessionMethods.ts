import { createAsyncThunk } from '@reduxjs/toolkit';
import { SessionService } from '../../services/SessionService';
import { setMessage } from '../message/messageSlice';
import { ISession, ISessionNew } from './ISession';

export const listSessionsForActivity = createAsyncThunk(
    'session/listForActivity',
    async (activityId: string, thunkAPI) => {
        try {
            const response: any = await SessionService.getAllSessionsForActivity(activityId);
            return { activityId: activityId, sessions: response.data };
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

export const getSession = createAsyncThunk(
    'session/get',
    async (sessionId: string, thunkAPI) => {
        try {
            const response: any = await SessionService.getSession(sessionId);
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

export const addSession = createAsyncThunk(
    'session/add',
    async (newSession: ISessionNew, thunkAPI) => {
        try {
            const response: any = await SessionService.createSession(newSession);
            return { session: response.data };
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

export const editSession = createAsyncThunk(
    'session/edit',
    async (updatedSession: ISession, thunkAPI) => {
        try {
            const response: any = await SessionService.updateSession(updatedSession);
            return { session: response.data };
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

export const deleteSession = createAsyncThunk(
    'session/delete',
    async ({ activityId, sessionId }: any, thunkAPI) => {
        try {
            const data: any = await SessionService.deleteSession(sessionId);
            return { activityId: activityId, sessionId: sessionId };
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