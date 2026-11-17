import { api } from './client';
import type { Comment } from '../types';

export async function getComments(taskId: number): Promise<Comment[]> {
  const { data } = await api.get<Comment[]>(`/tasks/${taskId}/comments`);
  return data;
}

export async function addComment(taskId: number, text: string): Promise<Comment> {
  const { data } = await api.post<Comment>(`/tasks/${taskId}/comments`, { text });
  return data;
}