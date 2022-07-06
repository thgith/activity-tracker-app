import { createSlice } from '@reduxjs/toolkit'

export interface TimerState {
    startDateUtc: string | null,
    isRunning: boolean;
    secondsElapsed: number;
    intervalId: number | null;
};

const initialState: TimerState = {
    startDateUtc: null,
    isRunning: false,
    secondsElapsed: 0,
    intervalId: null
};

const timerSlice = createSlice({
    name: 'timer',
    initialState,
    reducers: {
        pauseTimer(state: TimerState, action) {
            console.log('timer paused from slice');
            clearInterval(state.intervalId as number);
            state.isRunning = false;
            state.intervalId = null;
        },
        startTimer(state: TimerState, action) {
            console.log('timer started from slice');
            state.isRunning = true;
            state.startDateUtc = action.payload.startDateUtc;
            state.intervalId = action.payload.intervalId;
        },
        incrementTimer(state, action) {
            console.log('incrementing...');
            state.secondsElapsed += 1;
        },
        resetTimer(state, action) {
            console.log('timer restarted from slice');
            state.isRunning = false;
            state.startDateUtc = null;
            state.intervalId = null;
            state.secondsElapsed = 0;

        },
    }
});

export const { pauseTimer, startTimer, incrementTimer, resetTimer } = timerSlice.actions;
export default timerSlice.reducer