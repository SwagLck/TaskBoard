import { PRIORITIES, STATUSES } from '../constants';
import type { Task, TaskStatus } from '../types';

interface Props {
  task: Task;
  onStatusChange: (task: Task, status: TaskStatus) => void;
  onDelete: (task: Task) => void;
  onOpenComments: (task: Task) => void;
}

export default function TaskCard({ task, onStatusChange, onDelete, onOpenComments }: Props) {
  const priorityLabel = PRIORITIES.find((p) => p.value === task.priority)?.label;

  return (
    <div style={{ border: '1px solid #ccc', borderRadius: 6, padding: 10, background: 'white', color: '#222' }}>
      <strong>{task.title}</strong>
      <div style={{ fontSize: 13, marginTop: 4 }}>Prioritetas: {priorityLabel}</div>
      {task.dueDate && (
        <div style={{ fontSize: 13 }}>Terminas: {new Date(task.dueDate).toLocaleDateString('lt-LT')}</div>
      )}
      {task.assigneeName && <div style={{ fontSize: 13 }}>Vykdytojas: {task.assigneeName}</div>}

      <div style={{ display: 'flex', gap: 6, marginTop: 8, flexWrap: 'wrap' }}>
        <select value={task.status} onChange={(e) => onStatusChange(task, e.target.value as TaskStatus)}>
          {STATUSES.map((s) => (
            <option key={s.value} value={s.value}>
              {s.label}
            </option>
          ))}
        </select>
        <button onClick={() => onOpenComments(task)}>Komentarai ({task.commentCount})</button>
        <button onClick={() => onDelete(task)}>Ištrinti</button>
      </div>
    </div>
  );
}