import axios from 'axios';
import { API_URL_BASE } from '../app/constants';
const USER_API_URL = `${API_URL_BASE}User/`;
axios.defaults.withCredentials = true;
axios.defaults.headers.post['Content-Type'] = 'application/json;charset=utf-8';
axios.defaults.headers.post['Access-Control-Allow-Origin'] = '*';

const getUser = (userId: string) => {
    return axios
        .get(`${USER_API_URL}${userId}`)
        .then((response: any) => {
            return response.data;
        });
};

const addUser = (
    firstName: string,
    lastName: string,
    email: string,
    password: string) => {
    return axios
        .post(`${USER_API_URL}`, {
            'FirstName': firstName,
            'LastName': lastName,
            'Email': email,
            'Password': password
        })
        .then((response: any) => {
            return response.data;
        });
};

const updateUser = (
    firstName: string,
    lastName: string,
    email: string,
    password: string) => {
    return axios
        .put(`${USER_API_URL}`, {
            'FirstName': firstName,
            'LastName': lastName,
            'Email': email,
            'Password': password
        })
        .then((response: any) => {
            return response.data;
        });
};


const deleteUser = (userId: string) => {
    return axios
        .delete(`${USER_API_URL}${userId}`)
        .then((response: any) => {
        });
};

const authService = {
    getUser,
    addUser,
    updateUser,
    deleteUser,
};

export default authService;