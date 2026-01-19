import { ref, computed, watch, onMounted } from "vue";
import type { Note } from "@/types/note";
import api from "@/services/api";

export function useNotes() {
  const notes = ref<Note[]>([]);
  const activeNoteId = ref<number | null>(null);
  const loading = ref(false);
  const error = ref<string | null>(null);
  const searchQuery = ref("");

  // -----------------------------
  // Computed
  // -----------------------------
  const activeNote = computed<Note | null>(() => {
    return notes.value.find(n => n.id === activeNoteId.value) ?? null;
  });

  const filteredNotes = computed(() => {
    if (!searchQuery.value) return notes.value;
    return notes.value.filter(n =>
      n.title.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
      n.content.toLowerCase().includes(searchQuery.value.toLowerCase())
    );
  });

  // -----------------------------
  // Persist active note
  // -----------------------------
  watch(activeNoteId, id => {
    if (id !== null) localStorage.setItem("activeNoteId", id.toString());
    else localStorage.removeItem("activeNoteId");
  });

  // -----------------------------
  // Fetch notes
  // -----------------------------
  const fetchNotes = async () => {
    loading.value = true;
    error.value = null;

    try {
      const res = await api.get<Note[]>("/Notes");

      notes.value = res.data.map(n => ({
        ...n,
        id: Number(n.id),
      }));

      const savedId = localStorage.getItem("activeNoteId");
      activeNoteId.value =
        savedId && notes.value.some(n => n.id === Number(savedId))
          ? Number(savedId)
          : notes.value[0]?.id ?? null;
    } catch (err: any) {
      console.error("Failed to fetch notes:", err);
      error.value =
        err?.response?.data?.message ?? "Failed to load notes";
    } finally {
      loading.value = false;
    }
  };

  // -----------------------------
  // Create note
  // -----------------------------
  const createNote = async () => {
    loading.value = true;
    error.value = null;

    try {
      const res = await api.post<Note>("/Notes", {
        title: "",
        content: "",
      });

      const note = { ...res.data, id: Number(res.data.id) };
      notes.value.unshift(note);
      activeNoteId.value = note.id;
    } catch (err: any) {
      error.value =
        err?.response?.data?.message ?? "Failed to create note";
    } finally {
      loading.value = false;
    }
  };

  // -----------------------------
  // Update note
  // -----------------------------
  const updateNote = async (
    id: number,
    updates: Partial<Pick<Note, "title" | "content">>
  ) => {
    try {
      const res = await api.put<Note>(`/Notes/${id}`, updates);
      const note = notes.value.find(n => n.id === id);
      if (note) Object.assign(note, res.data);
    } catch (err: any) {
      error.value =
        err?.response?.data?.message ?? "Failed to update note";
    }
  };

  // -----------------------------
  // Delete note
  // -----------------------------
  const deleteNote = async (id: number) => {
    try {
      await api.delete(`/Notes/${id}`);
      notes.value = notes.value.filter(n => n.id !== id);
      if (activeNoteId.value === id) {
        activeNoteId.value = notes.value[0]?.id ?? null;
      }
    } catch (err: any) {
      error.value =
        err?.response?.data?.message ?? "Failed to delete note";
    }
  };

  onMounted(fetchNotes);

  return {
    notes,
    filteredNotes,
    activeNote,
    activeNoteId,
    loading,
    error,
    searchQuery,
    setSearchQuery: (q: string) => (searchQuery.value = q),
    setActiveNoteId: (id: number | null) => (activeNoteId.value = id),
    fetchNotes,
    createNote,
    updateNote,
    deleteNote,
  };
}
