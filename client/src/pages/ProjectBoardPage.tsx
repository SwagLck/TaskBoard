import { useCallback, useEffect, useState, type FormEvent } from 'react';
import { Link, useParams } from 'react-router-dom';
import { getProject } from '../api/projects';
import { createTask, deleteTask, getTasks, updateTaskStatus } from '../api/tasks';
import { getErrorMessage } from '../api/client';
import { PAGE_SIZE, PRIORITIES, STATUSES } from '../constants';
import TaskCard from '../components/TaskCard';
import CommentsModal from '../components/CommentsModal';
import type { PagedResult, Project, Task, TaskPriority, TaskStatus } from '../types';

export default function ProjectBoardPage() {
  const { id } = useParams();
  const projectId = Number(id);

  const [project, setProject] = useState<Project | null>(null);
  const [result, setResult] = useState<PagedResult<Task> | null>(null);
  const [statusFilter, setStatusFilter] = useState<TaskStatus | ''>('');
  const [page, setPage] = useState(1);
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);

  const [title, setTitle] = useState('');
  const [priority, setPriority] = useState<TaskPriority>('Medium');
  const [commentsTask, setCommentsTask] = useState<Task | null>(null);

  const loadTasks = useCallback(async () => {
    try {
      const data = await getTasks(projectId, {
        status: statusFilter || undefined,
        page,
        pageSize: PAGE_SIZE,
      });
      setResult(data);
    } catch (err) {
      setError(getErrorMessage(err));
    } finally {
      setLoading(false);
    }
  }, [projectId, statusFilter, page]);

  useEffect(() => {
    getProject(projectId)
      .then(setProject)
      .catch((err) => setError(getErrorMessage(err)));
  }, [projectId]);

  useEffect(() => {
    loadTasks();
  }, [loadTasks]);

  async function handleCreate(e: FormEvent) {
    e.preventDefault();
    setError(null);
    try {
      await createTask(projectId, { title, priority });
      setTitle('');
      setPage(1);
      await loadTasks();
    } catch (err) {
      setError(getErrorMessage(err));
    }
  }

  async function handleStatusChange(task: Task, status: TaskStatus) {
    setError(null);
    try {
      await updateTaskStatus(task.id, status);
      await loadTasks();
    } catch (err) {
      setError(getErrorMessage(err));
    }
  }

  async function handleDelete(task: Task) {
    if (!confirm(`Ištrinti užduotį "${task.title}"?`)) return;
    setError(null);
    try {
      await deleteTask(task.id);
      
      if (result && result.items.length === 1 && page > 1) {
        setPage(page - 1);
      } else {
        await loadTasks();
      }
    } catch (err) {
      setError(getErrorMessage(err));
    }
  }

  function handleCommentAdded(taskId: number) {
    setResult((prev) =>
      prev
        ? {
            ...prev,
            items: prev.items.map((t) =>
              t.id === taskId ? { ...t, commentCount: t.commentCount + 1 } : t
            ),
          }
        : prev
    );
    setCommentsTask((prev) => (prev ? { ...prev, commentCount: prev.commentCount + 1 } : prev));
  }

  function handleFilterChange(value: TaskStatus | '') {
    setStatusFilter(value);
    setPage(1);
  }

  const tasks = result?.items ?? [];

  return (
    <div style={{ maxWidth: 1000, margin: '30px auto', fontFamily: 'sans-serif', padding: '0 16px' }}>
      <Link to="/projects">← Projektai</Link>
      <h1>{project?.name ?? 'Kraunama...'}</h1>

      <form onSubmit={handleCreate} style={{ display: 'flex', gap: 8, marginBottom: 12 }}>
        <input
          style={{ flex: 1 }}
          placeholder="Nauja užduotis"
          value={title}
          onChange={(e) => setTitle(e.target.value)}
          required
        />
        <select value={priority} onChange={(e) => setPriority(e.target.value as TaskPriority)}>
          {PRIORITIES.map((p) => (
            <option key={p.value} value={p.value}>
              {p.label}
            </option>
          ))}
        </select>
        <button type="submit">Pridėti</button>
      </form>

      <div style={{ marginBottom: 12 }}>
        Filtras:{' '}
        <select value={statusFilter} onChange={(e) => handleFilterChange(e.target.value as TaskStatus | '')}>
          <option value="">Visos</option>
          {STATUSES.map((s) => (
            <option key={s.value} value={s.value}>
              {s.label}
            </option>
          ))}
        </select>
      </div>

      {error && <div style={{ color: 'crimson', marginBottom: 12 }}>{error}</div>}
      {loading && <p>Kraunama...</p>}
      {!loading && tasks.length === 0 && <p>Užduočių nėra.</p>}

      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(3, 1fr)', gap: 12 }}>
        {STATUSES.map((s) => (
          <div key={s.value} style={{ background: '#f2f2f2', borderRadius: 8, padding: 10, minHeight: 120 }}>
            <h3 style={{ marginTop: 0, color: '#222' }}>
              {s.label} ({tasks.filter((t) => t.status === s.value).length})
            </h3>
            <div style={{ display: 'grid', gap: 8 }}>
              {tasks
                .filter((t) => t.status === s.value)
                .map((t) => (
                  <TaskCard
                    key={t.id}
                    task={t}
                    onStatusChange={handleStatusChange}
                    onDelete={handleDelete}
                    onOpenComments={setCommentsTask}
                  />
                ))}
            </div>
          </div>
        ))}
      </div>

      {result && result.totalPages > 1 && (
        <div style={{ display: 'flex', gap: 12, justifyContent: 'center', marginTop: 16 }}>
          <button disabled={page <= 1} onClick={() => setPage(page - 1)}>
             Atgal
          </button>
          <span>
            {result.page} / {result.totalPages} (iš viso: {result.totalCount})
          </span>
          <button disabled={page >= result.totalPages} onClick={() => setPage(page + 1)}>
            Pirmyn 
          </button>
        </div>
      )}

      {commentsTask && (
        <CommentsModal
          task={commentsTask}
          onClose={() => setCommentsTask(null)}
          onCommentAdded={handleCommentAdded}
        />
      )}
    </div>
  );
}