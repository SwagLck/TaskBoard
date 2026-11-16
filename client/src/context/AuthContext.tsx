import { createContext, useContext, useState, type ReactNode } from 'react';
import * as authApi from '../api/auth';
import { STORAGE_KEYS } from '../config';
import type { AuthResponse } from '../types';

interface User {
    userId: number;
    email: string;
    displayName: string;
}

interface AuthContextValue {
    user: User | null;
    login: (email: string, password: string) => Promise<void>;
    register: (email: string, password: string, displayName: string) => Promise<void>;
    logout: () => void;
}

const AuthContext = createContext<AuthContextValue | null>(null);

function loadUser(): User | null {
    const token = localStorage.getItem(STORAGE_KEYS.token);
    const raw = localStorage.getItem(STORAGE_KEYS.user);
    if(!token || !raw) return null;
    try {
        return JSON.parse(raw) as User;
    } catch {
        return null;
    }
}

export function AuthProvider ({ children}: { children: ReactNode}) {
    const [user, setUser] = useState<User | null>(loadUser);

    function saveSession(res: AuthResponse){
        const u: User = {userId: res.userId, email: res.email, displayName: res.displayName};
        localStorage.setItem(STORAGE_KEYS.token, res.token);
        localStorage.setItem(STORAGE_KEYS.user, JSON.stringify(u));
        setUser(u);
    }

    async function login(email: string, password: string){
        saveSession(await authApi.login(email, password));
    }

    async function register(email: string, password: string, displayName: string){
        saveSession(await authApi.register(email, password, displayName));
    }

    function logout() {
        localStorage.removeItem(STORAGE_KEYS.token);
        localStorage.removeItem(STORAGE_KEYS.user);
        setUser(null);
    }

    return (
        <AuthContext.Provider value={{ user, login, register, logout }}>
            { children }
        </AuthContext.Provider>
    );
}


export function useAuth(): AuthContextValue{
    const ctx = useContext(AuthContext);
    if(!ctx) throw new Error('useAuth must be used withing AuthProvider');
    return ctx;
}