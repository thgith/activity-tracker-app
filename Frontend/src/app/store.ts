import { configureStore, ThunkAction, Action } from '@reduxjs/toolkit';
import activitiesReducer from '../features/dataSlice';
import messageReducer from '../features/message/messageSlice';
import timerReducer from '../features/timer/timerSlice';
import userReducer from '../features/User/userSlice';

export const store = configureStore({
    reducer: {
        message: messageReducer,
        activitiesData: activitiesReducer,
        userData: userReducer,
        timer: timerReducer
    },
});

export type AppDispatch = typeof store.dispatch;
export type RootState = ReturnType<typeof store.getState>;
export type AppThunk<ReturnType = void> = ThunkAction<
    ReturnType,
    RootState,
    unknown,
    Action<string>
>;