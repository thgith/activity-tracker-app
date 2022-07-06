import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import { setMessage } from '../message/messageSlice';
import AuthService from '../../services/AuthService';
import UserService from '../../services/UserService';
import { IRegister } from './IUser';

export const register = createAsyncThunk(
    'user/register',
    async ({ firstName, lastName, email, password }: IRegister, thunkAPI) => {
        try {
            const data: any = await AuthService.register(firstName, lastName, email, password);
            thunkAPI.dispatch(setMessage("Successfully registered user. Redirecting to home page..."));
            return { user: data.entity }
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

export const logIn = createAsyncThunk(
    'user/login',
    async ({ email, password }: any, thunkAPI) => {
        try {
            const data = await AuthService.logIn(email, password);
            return { user: data.entity };
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

export const logOut = createAsyncThunk(
    'user/logout',
    async () => {
        await AuthService.logOut();
    });

export const getUser = createAsyncThunk(
    'user/get',
    async (userId: string, thunkAPI) => {
        try {
            const data = await UserService.getUser(userId);
            return { user: data };
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

export const updateUser = createAsyncThunk(
    'user/update',
    async ({ firstName, lastName, email, password }: any, thunkAPI) => {
        try {
            const data = await UserService.updateUser(firstName, lastName, email, password);
            return { user: {...data.entity} };
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

export const deleteUser = createAsyncThunk(
    'user/delete',
    async (userId: string, thunkAPI) => {
        try {
            await UserService.deleteUser(userId);
            return { userId: userId };
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

const initialState = { user: null };

const userSlice = createSlice({
    name: 'user',
    initialState,
    reducers: {},
    extraReducers: {
        [(logIn as any).fulfilled]: (state: any, action: any) => {
            state.user = action.payload.user;
        },
        [(logIn as any).rejected]: (state: any, action: any) => {
            state.user = null;
        },

        [(register as any).fulfilled]: (state: any, action: any) => {
            state.user = action.payload.user;
        },
        [(register as any).rejected]: (state: any, action: any) => {
            state.user = null;
        },

        [(getUser as any).fulfilled]: (state: any, action: any) => {
            state.user = action.payload.user;
        },
        [(getUser as any).rejected]: (state: any, action: any) => {
            state.user = null;
        },

        [(updateUser as any).fulfilled]: (state: any, action: any) => {
            state.user = action.payload.user;
        },
        [(updateUser as any).rejected]: (state: any, action: any) => {
        },

        [(deleteUser as any).fulfilled]: (state: any, action: any) => {
            state.user = null;
        },
        [(deleteUser as any).rejected]: (state: any, action: any) => {
        }
    },
});

const { reducer } = userSlice;
export default reducer;
