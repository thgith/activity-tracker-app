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
    userId: string,
    firstName: string,
    lastName: string,
    email: string) => {
    return axios
        .put(`${USER_API_URL}${userId}`, {
            'FirstName': firstName,
            'LastName': lastName,
            'Email': email,
        })
        .then((response: any) => {
            return response.data;
        });
};

const changePassword = (userId: string, email: string, oldPassword: string, newPassword: string) => {
    return axios
        .put(`${USER_API_URL}${userId}/changePassword`, {
            'Email': email,
            'OldPassword': oldPassword,
            'NewPassword': newPassword
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

const userService = {
    getUser,
    addUser,
    updateUser,
    changePassword,
    deleteUser,
};

export default userService;