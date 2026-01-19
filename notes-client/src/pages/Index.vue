<template>
  <div v-if="isMobile" class="flex h-screen bg-background overflow-hidden flex-col">
    <!-- Mobile Header -->
    <header class="flex items-center gap-3 px-4 py-3 border-b border-border bg-sidebar">
      <Sheet v-model:open="sidebarOpen">
        <SheetTrigger asChild>
          <Button variant="ghost" size="icon" class="cursor-pointer shrink-0">
            <Menu class="w-5 h-5" />
          </Button>
        </SheetTrigger>
        <SheetContent side="left" class="p-0 w-80">
          <NotesSidebar
            :notes="filteredNotes"
            :active-note-id="activeNoteId"
            :search-query="searchQuery"
            @update:search-query="setSearchQuery"
            @select-note="handleNoteSelect"
            @new-note="handleNewNote"
            @sign-out="handleSignOut"
          />
        </SheetContent>
      </Sheet>

      <h1 class="text-lg font-serif font-semibold text-foreground truncate">
        {{ activeNote?.title || 'Notes' }}
      </h1>
    </header>

    <!-- Mobile Editor -->
    <div class="flex-1 overflow-hidden">
      <NoteEditor
        v-if="activeNote"
        :note="activeNote"
        @update-note="updateNote"
        @delete-note="deleteNote"
      />
      <div v-else class="flex-1 flex items-center justify-center text-muted-foreground">
        Select or create a note
      </div>
    </div>
  </div>

  <!-- Desktop Layout -->
  <div v-else class="flex h-screen bg-background overflow-hidden">
          <NotesSidebar
            :notes="filteredNotes"
            :active-note-id="activeNoteId"
            :search-query="searchQuery"
            @update:search-query="setSearchQuery"
            @select-note="handleNoteSelect"
            @new-note="handleNewNote"
            @sign-out="handleSignOut"
          />

    <div class="flex-1 h-auto flex justify-center items-center overflow-hidden">
      <div v-if="loading" class="flex-1 flex items-center justify-center text-muted-foreground">
        Loading notes...
      </div>
      <div v-else-if="error" class="flex-1 flex items-center justify-center text-destructive">
        {{ error }}
      </div>
      <NoteEditor
        v-else-if="activeNote"
        :note="activeNote"
        @update-note="updateNote"
        @delete-note="deleteNote"
      />
      <div v-else class="flex-1  flex items-center justify-center text-muted-foreground">
        Select or create a note
      </div>
    </div>
  </div> 
</template>

<script setup lang="ts">
import { ref, computed } from 'vue';
import { useRouter } from 'vue-router';
import { useNotes } from '@/hooks/useNotes';
import { useAuth } from '@/hooks/useAuth';
import { useIsMobile } from '@/hooks/useIsMobile';
import { Sheet, SheetContent, SheetTrigger } from '@/components/ui/sheet';
import { Button } from '@/components/ui/button';
import { Menu } from 'lucide-vue-next';
import NoteEditor from '@/components/notes/NoteEditor.vue';
import NotesSidebar from '@/components/notes/NotesSidebar.vue';

// -----------------------------
// Mobile state
// -----------------------------
const sidebarOpen = ref(false);

const isMobile = useIsMobile();

// -----------------------------
// Notes composable
// -----------------------------
const {
  notes,
  activeNote,
  activeNoteId,
  loading,
  error,
  createNote,
  updateNote,
  deleteNote,
  setActiveNoteId,
  searchQuery,
  setSearchQuery,
} = useNotes();

const { signOut } = useAuth();
const router = useRouter();

// -----------------------------
// Handlers
// -----------------------------
const handleNoteSelect = (id: number) => {
  setActiveNoteId(id);
  if (isMobile.value) sidebarOpen.value = false;
};

const handleNewNote = async () => {
  await createNote();
  if (isMobile.value) sidebarOpen.value = false;
};

const handleSignOut = async () => {
  await signOut();
  router.push('/auth');
};

// -----------------------------
// Filter notes by search query
// -----------------------------
const filteredNotes = computed(() => {
  if (!searchQuery.value) return notes.value;
  return notes.value.filter(
    n =>
      n.title?.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
      n.content?.toLowerCase().includes(searchQuery.value.toLowerCase())
  );
});
</script>
