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

export type TaskStatus = 'ToDo' | 'InProgress' | 'Done';
export type TaskPriority = 'Low' | 'Medium' | 'High';

export interface Task {
  id: number;
  title: string;
  description: string | null;
  status: TaskStatus;
  priority: TaskPriority;
  dueDate: string | null;
  createdAt: string;
  projectId: number;
  assigneeId: number | null;
  assigneeName: string | null;
  commentCount: number;
}

export interface PagedResult<T> {
  items: T[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}

export interface Comment {
  id: number;
  text: string;
  createdAt: string;
  authorId: number;
  authorName: string;
}