import { ref, computed, onMounted, watch } from 'vue';
import type { Ref } from 'vue';
import type { Note } from '@/types/note';

const STORAGE_KEY = 'notes-app-2026';

const generateId = () => Math.random().toString(36).substring(2, 15);

export function useNotes() {
  const notes: Ref<Note[]> = ref([]);
  const activeNoteId: Ref<string | null> = ref(null);
  const searchQuery: Ref<string> = ref('');

  // Load notes from localStorage on mount
  onMounted(() => {
    const stored = localStorage.getItem(STORAGE_KEY);
    if (stored) {
      const parsed: Note[] = JSON.parse(stored).map((note: Note) => ({
        ...note,
        createdAt: new Date(note.createdAt),
        updatedAt: new Date(note.updatedAt),
      }));
      notes.value = parsed;
      if (parsed.length > 0) {
        activeNoteId.value = parsed[0].id;
      }
    }
  });

  // Save notes to localStorage whenever they change
  watch(notes, (newNotes) => {
    localStorage.setItem(STORAGE_KEY, JSON.stringify(newNotes));
  }, { deep: true });

  // Create a new note
  const createNote = (): Note => {
    const now = new Date();
    const newNote: Note = {
      id: generateId(),
      title: 'Untitled Note',
      content: '',
      createdAt: now,
      updatedAt: now,
    };
    notes.value = [newNote, ...notes.value];
    activeNoteId.value = newNote.id;
    return newNote;
  };

  // Update an existing note
  const updateNote = (id: string, updates: Partial<Pick<Note, 'title' | 'content'>>) => {
    notes.value = notes.value.map(note =>
      note.id === id
        ? { ...note, ...updates, updatedAt: new Date() }
        : note
    );
  };

  // Delete a note
  const deleteNote = (id: string) => {
    notes.value = notes.value.filter(note => note.id !== id);
    if (activeNoteId.value === id) {
      activeNoteId.value = notes.value.length > 0 ? notes.value[0].id : null;
    }
  };

  // Computed active note
  const activeNote = computed(() => {
    return notes.value.find(note => note.id === activeNoteId.value) || null;
  });

  // Computed filtered notes
  const filteredNotes = computed(() => {
    return notes.value.filter(note =>
      note.title.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
      note.content.toLowerCase().includes(searchQuery.value.toLowerCase())
    );
  });

  return {
    notes: filteredNotes,
    activeNote,
    activeNoteId,
    searchQuery,
    setActiveNoteId: (id: string) => (activeNoteId.value = id),
    setSearchQuery: (query: string) => (searchQuery.value = query),
    createNote,
    updateNote,
    deleteNote,
  };
}
