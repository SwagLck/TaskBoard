import axios, { AxiosError } from 'axios';
import { config, STORAGE_KEYS } from '../config';
import type { ProblemDetails } from '../types';

export const api = axios.create({
    baseURL: config.apiUrl,
});

api.interceptors.request.use((request) => {
    const token = localStorage.getItem(STORAGE_KEYS.token);
    if (token)
        request.headers.Authorization = `Bearer ${token}`;

    return request;
});

api.interceptors.response.use(
    (response) => response,
    (error: AxiosError) => {
        if(error.response?.status === 401 && localStorage.getItem(STORAGE_KEYS.token)){
            localStorage.removeItem(STORAGE_KEYS.token);
            localStorage.removeItem(STORAGE_KEYS.user);
            window.location.href = '/login';
        }
        return Promise.reject(error);
    }
);

export function getErrorMessage(error: unknown): string {
    if (axios.isAxiosError<ProblemDetails>(error)) {
        const data = error.response?.data;
        if(data?.errors) {
            return Object.values(data.errors).flat().join(' ');
        }

        if(data?.detail) return data.detail;
        if (data?.title) return data.title;
        if(!error.response) return 'Nepavyko susisiekti su serveriu';
    }
    return 'Ivyko nenumatyta klaida';
}
