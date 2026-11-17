import { api } from "./client";
import type { Project } from "../types";

export async function getProjects(): Promise<Project[]> {
    const { data } = await api.get<Project[]>('/projects');
    return data;
}

export async function createProject(name: string, description?: string): Promise<Project>{
    const { data } = await api.post<Project>('/projects', { name, description});
    return data;
}

export async function deleteProject(id: number): Promise<void> {
    await api.delete(`/projects/${id}`);
}

export async function getProject(id: number): Promise<Project>{
    const { data} = await api.get<Project>(`/projects/${id}`);
    return data;
}