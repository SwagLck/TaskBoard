function required(name: string): string {
    const value = import.meta.env[name] as string | undefined;
    if(!value){
        throw new Error(`Missing environment variable: ${name}`);
    }
    return value;
}

export const config = {
    apiUrl: required('VITE_API_URL'),
} as const;

export const STORAGE_KEYS = {
    token: 'taskboard_token',
    user: 'taskboard_user',
} as const;

