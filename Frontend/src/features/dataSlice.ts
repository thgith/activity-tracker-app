import produce from 'immer';
import { createSlice, Dictionary } from '@reduxjs/toolkit'
import { IActivity } from './activities/IActivity';
import { ISession } from './sessions/ISession';
import { listActivities, getActivity, addActivity, editActivity, deleteActivity } from './activities/activityMethods';
import { listSessionsForActivity, getSession, addSession, editSession, deleteSession } from './sessions/sessionMethods';

export interface DataState {
    activities: IActivity[];
    stringFilter: string;
    activityIdToSessions: Dictionary<ISession>;
    gotFullListBefore: boolean;
    isLoading: boolean;
}

const initialState: DataState = {
    activities: [],
    stringFilter: '',
    activityIdToSessions: {},
    gotFullListBefore: false,
    isLoading: false
};

const dataSlice = createSlice({
    name: 'data',
    initialState,
    reducers: {},
    extraReducers: {
        [(listActivities as any).fulfilled]: (state: any, action: any) => {
            console.log('list fulfilled');
            // The sessions on these activities objects don't matter,
            // since we will use the dictionary below when we need session data
            state.activities = action.payload.activities;
            state.gotFullListBefore = true;

            // Make a dictionary of activityId to sessions
            const updatedDictionary = produce(state.activityIdToSessions, (draft: any) => {
                action.payload.activities.map((x: IActivity) => {
                    draft[x.id as string] = x.sessions;
                });
            });
            state.activityIdToSessions = updatedDictionary;
        },
        [(listActivities as any).rejected]: (state: any, action: any) => {
            console.log('list rejected');
            state.activities = [];
        },
        [(getActivity as any).fulfilled]: (state: any, action: any) => {
            console.log('get fulfilled');
            // Only add to activities if we already made a call to make the list
            // Othwerise we leave it empty so that the list we be grabbed when it goes back
            // if (state.activities.length !== 0) {
            state.activities.push(action.payload.activity);
            // Add or replace it in our dictionary
            const updatedDictionary = produce(state.activityIdToSessions, (draft: any) => {
                draft[action.payload.activity.id] = action.payload.activity.sessions;
            });
            // Set the state with the updated data (immer pattern)
            state.activityIdToSessions = updatedDictionary;
            // }
        },
        [(getActivity as any).rejected]: (state: any, action: any) => {
            console.log('list rejected');
        },
        [(addActivity as any).fulfilled]: (state: any, action: any) => {
            console.log('add fulfilled');
            state.activities.push(action.payload.activity);

            // Add it in our dictionary
            const updatedDictionary = produce(state.activityIdToSessions, (draft: any) => {
                draft[action.payload.activity.id] = action.payload.activity.sessions;
            });
            // Set the state with the updated data (immer pattern)
            state.activityIdToSessions = updatedDictionary;
        },
        [(addActivity as any).rejected]: (state: any, action: any) => {
            console.log('add rejected');
        },
        [(editActivity as any).fulfilled]: (state: any, action: any) => {
            console.log('edit fulfilled');
            const filteredState = state.activities
                .filter((x: IActivity) => x.id !== action.payload.activity.id);
            state.activities = filteredState;
            state.activities.push(action.payload.activity);
        },
        [(editActivity as any).rejected]: (state: any, action: any) => {
            console.log('edit rejected');
        },
        [(deleteActivity as any).fulfilled]: (state: any, action: any) => {
            if (state.activities.length !== 0) {
                console.log('delete fulfilled');
                const updatedActivities = state.activities
                    .filter((x: IActivity) => x.id !== action.payload.activityId);
                state.activities = updatedActivities;

                // Delete it from our dictionary. Must set to const to mutate the state after.
                const updatedDictionary = produce(state.activityIdToSessions, (draft: any) => {
                    delete draft[action.payload.activityId]
                });
                // Set the state with the updated data (immer pattern)
                state.activityIdToSessions = updatedDictionary;
            }
        },
        [(deleteActivity as any).rejected]: (state: any, action: any) => {
            console.log('delete rejected');
        },
        [(listSessionsForActivity as any).fulfilled]: (state: any, action: any) => {
            console.log('list SESH fulfilled');

            // Update sessions for the activity in the dictionary
            const updatedDictionary = produce(state.activityIdToSessions, (draft: any) => {
                draft[action.payload.activityId] = action.payload.sessions;
            });
            state.activityIdToSessions = updatedDictionary;
        },
        [(listSessionsForActivity as any).rejected]: (state: any, action: any) => {
            console.log('list rejected');
        },
        [(getSession as any).fulfilled]: (state: any, action: any) => {
            console.log('get fulfilled');
            // Add or replace it in our dictionary
            const updatedDictionary = produce(state.activityIdToSessions, (draft: any) => {
                // If we have it in the dictionary, remove and add it
                if (draft[action.payload.session.activityId]) {
                    draft[action.payload.session.activityId] = draft[action.payload.session.activityId]
                        .filter((x: ISession) => x.id !== action.payload.session.id);
                    // Update the session in the dictionary
                    draft[action.payload.session.activityId].push(action.payload.session);
                }
            });
            // Set the state with the updated data (immer pattern)
            state.activityIdToSessions = updatedDictionary;
        },
        [(getSession as any).rejected]: (state: any, action: any) => {
            console.log('get SESH rejected');
        },
        [(addSession as any).fulfilled]: (state: any, action: any) => {
            // Add it in our dictionary
            const updatedDictionary = produce(state.activityIdToSessions, (draft: any) => {
                draft[action.payload.session.activityId].push(action.payload.session);
            });
            // Set the state with the updated data (immer pattern)
            state.activityIdToSessions = updatedDictionary;
        },
        [(addSession as any).rejected]: (state: any, action: any) => {
            console.log('add rejected');
        },
        [(editSession as any).fulfilled]: (state: any, action: any) => {
            // Add or replace it in our dictionary. Must set to const to mutate the state after.
            const updatedDictionary = produce(state.activityIdToSessions, (draft: any) => {
                // IMPORTANT: Must act on draft
                draft[action.payload.session.activityId] = draft[action.payload.session.activityId].filter((x: any) => x.id !== action.payload.session.id)
                draft[action.payload.session.activityId].push(action.payload.session);
            });
            // Set the state with the updated data (immer pattern)
            state.activityIdToSessions = updatedDictionary;
        },
        [(editSession as any).rejected]: (state: any, action: any) => {
            console.log('edit rejected');
        },
        [(deleteSession as any).fulfilled]: (state: any, action: any) => {
            if (state.activities.length !== 0) {
                console.log('delete SESH fulfilled');
                if (state.activityIdToSessions[action.payload.activityId]) {
                    // Delete session from list in corresonding activity in dictionary. Must set to const to mutate the state after.
                    const updatedDictionary = produce(state.activityIdToSessions, (draft: any) => {
                        draft[action.payload.activityId] = draft[action.payload.activityId]
                            .filter((x: ISession) => x.id !== action.payload.sessionId);
                    });
                    // Set the state with the updated data (immer pattern)
                    state.activityIdToSessions = updatedDictionary;
                }
            }
        },
        [(deleteSession as any).rejected]: (state: any, action: any) => {
            console.log('delete rejected');
        },
    }
})

export default dataSlice.reducer;