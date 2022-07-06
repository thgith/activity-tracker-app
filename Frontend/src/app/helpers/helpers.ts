import { useEffect, useRef, useState } from 'react';
import { CURR_USER_ID_COOKIE_NAME } from '../constants';

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
}

export const getUserIdCookie = () => {
    return getCookie(CURR_USER_ID_COOKIE_NAME);
}

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