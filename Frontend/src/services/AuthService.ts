import axios from 'axios';
import { API_URL_BASE } from '../app/constants';
const AUTH_API_URL = `${API_URL_BASE}Auth/`;
axios.defaults.withCredentials = true;
axios.defaults.headers.post['Content-Type'] ='application/json;charset=utf-8';
axios.defaults.headers.post['Access-Control-Allow-Origin'] = '*';

const register = (
    firstName: string,
    lastName: string,
    email: string,
    password: string) => {
    return axios
        .post(`${AUTH_API_URL}register`, {
            'FirstName': firstName,
            'LastName': lastName,
            'Email': email,
            'Password': password
        })
        .then((response: any) => {
            return response.data;
        });
}

const config = {
    headers: {
        'Referrer-Policy': 'origin',
        'Access-Control-Allow-Origin': '*',
        'Access-Control-Allow-Headers': 'Content-Type, Depth, User-Agent, X-File-Size, X-Requested-With, If-Modified-Since, X-File-Name, Cache-Control'
    }
}

const logIn = (email: string, password: string) => {
    return axios
        .post(`${AUTH_API_URL}login`, {
            'Email': email,
            'Password': password
        })
        .then((response: any) => {
            return response.data;
        });
};


const logOut = () => {
    return axios
    .post(`${AUTH_API_URL}logout`)
    .then((response: any) => {
    });
};

const authService = {
    register,
    logIn,
    logOut
};

export default authService;