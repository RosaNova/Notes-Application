<template>
  <div class="flex h-screen bg-background overflow-hidden flex-col" v-if="isMobile">
    <!-- Mobile Header -->
    <header class="flex items-center gap-3 px-4 py-3 border-b border-border bg-sidebar">
      <Sheet v-model:open="sidebarOpen">
        <SheetTrigger as-child>
          <Button variant="ghost" size="icon" class="cursor-pointer shrink-0">
            <Menu class="w-5 h-5" />
          </Button>
        </SheetTrigger>
        <SheetContent side="left" class="p-0 w-80">
          <NotesSidebar
            :notes="notes"
            :active-note-id="activeNoteId"
            :search-query="searchQuery"
            @update:search-query="setSearchQuery"
            @select-note="handleNoteSelect"
            @new-note="handleNewNote"
          />
        </SheetContent>
      </Sheet>
      <h1 class="text-lg font-serif font-semibold text-foreground">
        {{ activeNote?.title || 'Notes' }}
      </h1>
    </header>

    <!-- Mobile Editor -->
    <div class="flex-1 overflow-hidden">
      <NoteEditor
        :note="activeNote"
        @update-note="updateNote"
        @delete-note="deleteNote"
        :is-mobile="true"
      />
    </div>
  </div>

  <!-- Desktop Layout -->
  <div class="flex h-screen bg-background overflow-hidden" v-else>
    <NotesSidebar
      :notes="notes"
      :active-note-id="activeNoteId"
      :search-query="searchQuery"
      @update:search-query="setSearchQuery"
      @select-note="setActiveNoteId"
      @new-note="createNote"
    />
    <NoteEditor
      :note="activeNote"
      @update-note="updateNote"
      @delete-note="deleteNote"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue';
import { useNotes } from '@/hooks/useNotes';
import { useIsMobile } from '@/hooks/useIsMobile';
import { Sheet, SheetContent, SheetTrigger } from '@/components/ui/sheet';
import { Button } from '@/components/ui/button';
import { Menu } from 'lucide-vue-next';
import NoteEditor from '@/components/notes/NoteEditor.vue';
import NotesSidebar from '@/components/notes/NotesSidebar.vue';

// Mobile sheet state
const sidebarOpen = ref(false);
const isMobile = useIsMobile();

// Notes state
const {
  notes,
  activeNote,
  activeNoteId,
  searchQuery,
  setActiveNoteId,
  setSearchQuery,
  createNote,
  updateNote,
  deleteNote,
} = useNotes();

// Handlers
const handleNoteSelect = (id: string) => {
  setActiveNoteId(id);
  if (isMobile.value) sidebarOpen.value = false;
};

const handleNewNote = () => {
  createNote();
  if (isMobile.value) sidebarOpen.value = false;
};
</script>
