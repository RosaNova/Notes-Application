<script setup lang="ts">
import { Plus, Search, FileText, LogOut } from 'lucide-vue-next';
import Button from '@/components/ui/button/Button.vue';
import Input from '@/components/ui/input/Input.vue';
import { format } from 'date-fns';
import { cn } from '@/lib/utils';
import type { Note } from '@/types/note';

const props = defineProps<{
  notes: Note[];
  activeNoteId: number | null;
  searchQuery: string;
}>();

const emit = defineEmits<{
  (e: 'update:search-query', value: string): void;
  (e: 'select-note', id: number): void;
  (e: 'new-note'): void;
  (e: 'sign-out'): void;
}>();

const handleSearch = (value: string) => {
  emit('update:search-query', value);
};

const handleNoteSelect = (id: number) => {
  emit('select-note', id);
};

const handleNewNote = () => {
  emit('new-note');
};
</script>


<template>
  <aside class="w-80 h-full bg-background border-r text-secondary-foreground border-sidebar-border flex flex-col border-2 ">
    
    <!-- Header -->
    <div class="p-4 border-b border-sidebar-border">
      <div class="flex items-center justify-between mb-4">
        <h1 class="text-xl font-serif font-semibold text-foreground">
          Notes
        </h1>
        <Button
          @click="handleNewNote"
          size="sm"
          class="gap-1.5 bg-orange-500 cursor-pointer hover:bg-orange-500/90"
        >
          <Plus class="w-4 h-4" />
          New
        </Button>
      </div>

      <!-- Search -->
      <div class="relative">
        <Search class="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-muted-foreground" />
      <Input
         :model-value="searchQuery"
           @update:modelValue="handleSearch"
              placeholder="Search notes..."
            aria-label="Search notes"
            class="pl-9 bg-sidebar-accent border-sidebar-border"
/>

      </div>
    </div>

    <!-- Notes List -->
    <div class="flex-1 overflow-y-auto p-2">
      <!-- Empty State -->
      <div
        v-if="notes.length === 0"
        class="flex flex-col items-center justify-center h-40 text-muted-foreground"
      >
        <FileText class="w-10 h-10 mb-2 opacity-50" />
        <p class="text-sm">No notes yet</p>
        <p class="text-xs">Create your first note</p>
      </div>

      <!-- Notes -->
      <div v-else class="space-y-1">
        <button
          v-for="note in notes"
          :key="note.id"
          @click="handleNoteSelect(note.id)"
          :class="cn(
            'w-full text-left cursor-pointer p-3 rounded-lg transition-all duration-200 hover:bg-note-hover',
            { 'bg-note-active': activeNoteId === note.id }
          )"
          :aria-selected="activeNoteId === note.id"
        >
          <h3 class="font-medium text-foreground truncate text-sm">
            {{ note.title || 'Untitled Note' }}
          </h3>
          <p class="text-xs text-muted-foreground mt-1 line-clamp-2">
            {{ note.content || 'No content' }}
          </p>
          <p class="text-xs text-muted-foreground/70 mt-2">
            {{ format(new Date(note.createdAt), 'MMM d, yyyy') }}
          </p>
        </button>
      </div>
    </div>

    <!-- Footer -->
    <div class="p-4 items-center justify-between flex border-t border-sidebar-border">
      <p class="text-xs text-muted-foreground text-center mb-2">
        {{ notes.length }} {{ notes.length === 1 ? 'note' : 'notes' }}
      </p>
      <Button
        @click="$emit('sign-out')"
        variant="ghost"
        size="sm"
        class="w-25 cursor-pointer hover:bg-orange-500 gap-1.5 text-muted-foreground hover:text-foreground"
      >
        <LogOut class="w-3.5 h-3.5" />
        Sign out
      </Button>
    </div>

  </aside>
</template>
