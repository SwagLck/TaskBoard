import { useEffect, useState, type FormEvent } from 'react';
import { addComment, getComments } from '../api/comments';
import { getErrorMessage } from '../api/client';
import type { Comment, Task } from '../types';

interface Props {
  task: Task;
  onClose: () => void;
  onCommentAdded: (taskId: number) => void;
}

export default function CommentsModal({ task, onClose, onCommentAdded }: Props) {
  const [comments, setComments] = useState<Comment[]>([]);
  const [text, setText] = useState('');
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    getComments(task.id)
      .then(setComments)
      .catch((err) => setError(getErrorMessage(err)))
      .finally(() => setLoading(false));
  }, [task.id]);

  async function handleSubmit(e: FormEvent) {
    e.preventDefault();
    setError(null);
    try {
      const created = await addComment(task.id, text);
      setComments((prev) => [...prev, created]);
      setText('');
      onCommentAdded(task.id);
    } catch (err) {
      setError(getErrorMessage(err));
    }
  }

  return (
    <div
      onClick={onClose}
      style={{ position: 'fixed', inset: 0, background: 'rgba(0,0,0,0.5)', display: 'grid', placeItems: 'center' }}
    >
      <div
        onClick={(e) => e.stopPropagation()}
        style={{ background: 'white', color: '#222', borderRadius: 8, padding: 20, width: 460, maxHeight: '80vh', overflow: 'auto' }}
      >
        <div style={{ display: 'flex', justifyContent: 'space-between' }}>
          <h3 style={{ margin: 0 }}>{task.title}</h3>
          <button onClick={onClose}>Uždaryti</button>
        </div>

        {loading && <p>Kraunama...</p>}
        {!loading && comments.length === 0 && <p>Komentarų dar nėra.</p>}

        <ul style={{ listStyle: 'none', padding: 0, display: 'grid', gap: 8 }}>
          {comments.map((c) => (
            <li key={c.id} style={{ borderBottom: '1px solid #eee', paddingBottom: 6 }}>
              <strong>{c.authorName}</strong>{' '}
              <span style={{ fontSize: 12, color: '#777' }}>
                {new Date(c.createdAt).toLocaleString('lt-LT')}
              </span>
              <div>{c.text}</div>
            </li>
          ))}
        </ul>

        {error && <div style={{ color: 'crimson' }}>{error}</div>}

        <form onSubmit={handleSubmit} style={{ display: 'flex', gap: 8 }}>
          <input
            style={{ flex: 1 }}
            placeholder="Naujas komentaras"
            value={text}
            onChange={(e) => setText(e.target.value)}
            required
          />
          <button type="submit">Siųsti</button>
        </form>
      </div>
    </div>
  );
}