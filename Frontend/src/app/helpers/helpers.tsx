import { Dictionary } from '@reduxjs/toolkit';
import moment from 'moment';
import { useEffect, useRef } from 'react';
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
    const isInitialRender = useRef(true);// in react, when refs are changed component dont re-render 

    useEffect(() => {
        if (isInitialRender.current) {// skip initial execution of useEffect
            isInitialRender.current = false;// set it to false so subsequent changes of dependency arr will make useEffect to execute
            return;
        }
        return callback();
    });
};

const onlyUnique = (value: string, index: number, self: string[]) => {
    return self.indexOf(value) === index
};

/**
 * Creates the tags array from the tag string.
 * @param {string} tags - The string of comma-separated tags.
 */
export const createTagsArray = (tags: string) => {
    return trimStrArray(tags
        .split(',')
        .filter((x: any) => x.length !== 0))
        .filter(onlyUnique);
};

/**
 * Trims the white space in the string array.
 * @param {string[]} array - The array to trim.
 */
export const trimStrArray = (array: string[]) => {
    let newArr = [];
    for (let i = 0; i < array.length; i++) {
        let trimmedVal = array[i].trim();
        // trim() just replaces side white space so double check empty
        if (trimmedVal !== '') {
            newArr.push(trimmedVal);
        }
    }
    return newArr;
};

/**
 * Calculates the total hours spent on the activity.
 * @param {string} activityId - The ID of the activity.
 */
export const calculateActivityHours = (activityIdToSessions: Dictionary<ISession[]>, activityId: string) => {
    let sessions = activityIdToSessions ? activityIdToSessions[activityId] : [];
    if (!sessions || sessions.length === 0) {
        return 0;
    }

    let totalSeconds = 0;
    sessions.map((x: ISession) => {
        return totalSeconds += x.durationSeconds;
    });

    return calculateTotalHoursFromSeconds(totalSeconds);
};

/**
 * This only calculates seconds in hours, rounding to the closest decimal.
 * @param totalSeconds - The total seconds.
 * @returns The estimated total hours.
 */
export const calculateTotalHoursFromSeconds = (totalSeconds: number) => {
    return Math.round(totalSeconds / 3600 * 10) / 10;
}

/**
 * This only calculates the hours part of the total seconds. No decimal
 * because this doesn't include the leftover minutes or seconds.
 * @param totalSeconds - The total seconds.
 * @returns The hours portion.
 */
export const calculateHoursPortionOnly = (totalSeconds: number) => {
    return Math.floor(totalSeconds / 3600);
}

/**
 * This only calculates the remaining minutes of the total seconds. No decimal
 * because this doesn't include the leftover minutes or seconds.
 * @param totalSeconds - The total seconds.
 * @returns The minutes portion.
 */
export const calculateRemainingMinOnly = (durationSeconds: number) => {
    return Math.round(durationSeconds % 3600 / 60);
}

/**
 * This only calculates the remaining seconds of the total seconds.
 * @param totalSeconds - The total seconds.
 * @returns The seconds portion.
 */
export const calculateRemainingSecOnly = (durationSeconds: number) => {
    return durationSeconds % 3600 % 60;
};

/**
 * Calculates the total hours spent on activities.
 */
export const calculateTotalActivityHours = (activityIdToSessions: Dictionary<ISession[]>) => {
    let totalSeconds = 0;
    if (activityIdToSessions) {
        Object.keys(activityIdToSessions).forEach((k: string) => {
            let sessions = activityIdToSessions[k];
            (sessions as ISession[]).map((x: ISession) => {
                return totalSeconds += x.durationSeconds;
            })
        });
    }
    return calculateTotalHoursFromSeconds(totalSeconds);
};

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
};

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
};