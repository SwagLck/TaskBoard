import { useEffect, useState, type FormEvent } from 'react';
import { useAuth } from '../context/AuthContext';
import { createProject, deleteProject, getProjects } from '../api/projects';
import { getErrorMessage } from '../api/client';
import type { Project } from '../types';

export default function ProjectsPage() {
  const { user, logout } = useAuth();
  const [projects, setProjects] = useState<Project[]>([]);
  const [name, setName] = useState('');
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    getProjects()
      .then(setProjects)
      .catch((err) => setError(getErrorMessage(err)))
      .finally(() => setLoading(false));
  }, []);

  async function handleCreate(e: FormEvent) {
    e.preventDefault();
    setError(null);
    try {
      const created = await createProject(name);
      setProjects((prev) => [created, ...prev]);
      setName('');
    } catch (err) {
      setError(getErrorMessage(err));
    }
  }

  async function handleDelete(id: number) {
    if (!confirm('Ištrinti projektą su visomis jo užduotimis?')) return;
    try {
      await deleteProject(id);
      setProjects((prev) => prev.filter((p) => p.id !== id));
    } catch (err) {
      setError(getErrorMessage(err));
    }
  }

  return (
    <div style={{ maxWidth: 700, margin: '40px auto', fontFamily: 'sans-serif' }}>
      <header style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
        <h1>Projektai</h1>
        <div>
          {user?.displayName} <button onClick={logout}>Atsijungti</button>
        </div>
      </header>

      <form onSubmit={handleCreate} style={{ display: 'flex', gap: 8, marginBottom: 16 }}>
        <input
          style={{ flex: 1 }}
          placeholder="Naujo projekto pavadinimas"
          value={name}
          onChange={(e) => setName(e.target.value)}
          required
        />
        <button type="submit">Pridėti</button>
      </form>

      {error && <div style={{ color: 'crimson', marginBottom: 12 }}>{error}</div>}
      {loading && <p>Kraunama...</p>}
      {!loading && projects.length === 0 && <p>Projektų dar nėra.</p>}

      <ul style={{ listStyle: 'none', padding: 0, display: 'grid', gap: 8 }}>
        {projects.map((p) => (
          <li
            key={p.id}
            style={{
              border: '1px solid #ccc',
              borderRadius: 6,
              padding: 12,
              display: 'flex',
              justifyContent: 'space-between',
            }}
          >
            <div>
              <strong>{p.name}</strong>
              <div>Užduočių: {p.taksCount}</div>
            </div>
            <button onClick={() => handleDelete(p.id)}>Ištrinti</button>
          </li>
        ))}
      </ul>
    </div>
  );
}