export interface AuthResponse {
    token: string;
    expiresAt: string;
    userId: number;
    email: string;
    displayName: string;
}

export interface Project {
    id: number;
    name: string;
    description: string | null;
    createdAt: string;
    ownerId: number;
    taksCount: number;
}

export interface ProblemDetails {
    title?: string;
    detail?: string;
    status?: number;
    errors?: Record<string, string[]>;
}