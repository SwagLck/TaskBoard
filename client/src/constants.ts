import type { TaskPriority, TaskStatus } from './types';

export const STATUSES: { value: TaskStatus; label: string }[] = [
  { value: 'ToDo', label: 'Laukia' },
  { value: 'InProgress', label: 'Vykdoma' },
  { value: 'Done', label: 'Atlikta' },
];

export const PRIORITIES: { value: TaskPriority; label: string }[] = [
  { value: 'Low', label: 'Zemas' },
  { value: 'Medium', label: 'Vidutinis' },
  { value: 'High', label: 'Aukstas' },
];

export const PAGE_SIZE = 10;