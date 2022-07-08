import { Dictionary } from '@reduxjs/toolkit';
import moment from 'moment';
import { useEffect, useRef, useState } from 'react';
import { ISession } from '../../features/sessions/ISession';
import { CURR_USER_ID_COOKIE_NAME, MIN_DATE } from '../constants';

const getCookie = (cname: string) => {
    let name = cname + "=";
    let decodedCookie = decodeURIComponent(document.cookie);
    let ca = decodedCookie.split(';');
    for (let i = 0; i < ca.length; i++) {
        let c = ca[i];
        while (c.charAt(0) === ' ') {
            c = c.substring(1);
        }
        if (c.indexOf(name) === 0) {
            return c.substring(name.length, c.length);
        }
    }
    return null;
};

export const getUserIdCookie = () => {
    return getCookie(CURR_USER_ID_COOKIE_NAME);
};

export const useEffectSkipInitialRender = (callback: any) => {
    const [data, setData] = useState(null);
    const isInitialRender = useRef(true);// in react, when refs are changed component dont re-render 

    useEffect(() => {
        if (isInitialRender.current) {// skip initial execution of useEffect
            isInitialRender.current = false;// set it to false so subsequent changes of dependency arr will make useEffect to execute
            return;
        }
        return callback();
    });

};

export const onlyUnique = (value: string, index: number, self: string[]) => {
    return self.indexOf(value) === index
}

export const trimmedStrArray = (array: string[]) => {
    let newArr = [];
    for (let i = 0; i < array.length; i++) {
        let trimmedVal = array[i].trim();
        // trim() just replaces side white space so double check empty
        if (trimmedVal !== '') {
            newArr.push(trimmedVal);
        }
    }
    return newArr;
}

/**
 * Calculates the total hours spent on the activity.
 * @param {string} activityId - The ID of the activity.
 */
export const calculateActivityHours = (activityIdToSessions: Dictionary<ISession[]>, activityId: string) => {
    let totalSeconds = 0;
    let sessions = activityIdToSessions ? activityIdToSessions[activityId] : [];
    if (!sessions || sessions.length === 0) {
        return 0;
    }
    sessions.map((x: ISession) => {
        totalSeconds += x.durationSeconds
    });

    return Math.round(totalSeconds / 3600 * 10) / 10;
}

/**
 * Calculates the total hours spent on activities.
 */
export const calculateTotalActivityHours = (activityIdToSessions: Dictionary<ISession[]>) => {
    let totalSeconds = 0;
    if (activityIdToSessions) {
        Object.keys(activityIdToSessions).forEach((k: string) => {
            let sessions = activityIdToSessions[k];
            (sessions as ISession[]).map((x: ISession) => {
                totalSeconds += x.durationSeconds
            })
        });
    }
    return Math.round(totalSeconds / 3600 * 10) / 10;
}

/**
 * Creates the tags view.
 * @param {string[]} tags - The tags to render.
 */
export const displayTags = (tags: string[]) => {
    let tagItems: any = [];
    tags.forEach(tag => {
        tagItems.push(<span className='tag' key={tag}>{tag}</span>);
    });
    return tagItems;
}

/**
 * Takes the local date value and converts it
 * to what the API expects. 
 * Frontend display values are local. API values are in UTC.
 * @param {string | Date | null} dateLocal - The local date to convert.
 */
export const convertDateForApi = (dateLocal: string | Date | null) => {
    if (!dateLocal) {
        return null;
    }

    if (dateLocal === '') {
        return MIN_DATE;
    }

    return moment(dateLocal).utc().format();
}