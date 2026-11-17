import { api } from './client';
import type { PagedResult, Task, TaskPriority, TaskStatus } from '../types';

export interface TaskInput {
  title: string;
  description?: string;
  priority: TaskPriority;
  dueDate?: string | null;
  assigneeId?: number | null;
}

export async function getTasks(
  projectId: number,
  params: { status?: TaskStatus; page: number; pageSize: number }
): Promise<PagedResult<Task>> {
  const { data } = await api.get<PagedResult<Task>>(`/projects/${projectId}/tasks`, { params });
  return data;
}

export async function createTask(projectId: number, input: TaskInput): Promise<Task> {
  const { data } = await api.post<Task>(`/projects/${projectId}/tasks`, input);
  return data;
}

export async function updateTaskStatus(id: number, status: TaskStatus): Promise<Task> {
  const { data } = await api.patch<Task>(`/tasks/${id}/status`, { status });
  return data;
}

export async function deleteTask(id: number): Promise<void> {
  await api.delete(`/tasks/${id}`);
}