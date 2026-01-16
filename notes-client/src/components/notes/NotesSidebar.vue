<script setup lang="ts">
import { Plus, Search, FileText } from 'lucide-vue-next';
import Button from '@/components/ui/button/Button.vue';
import Input from '@/components/ui/input/Input.vue';
import { format } from 'date-fns';
import { cn } from '@/lib/utils';
import type { Note } from '@/types/note';

const props = defineProps<{
  notes: Note[];
  activeNoteId: string | null;
  searchQuery: string;
}>();

const emit = defineEmits<{
  (e: 'update:search-query', value: string): void;
  (e: 'select-note', id: string): void;
  (e: 'new-note'): void;
}>();

const handleSearch = (value: string) => {
  emit('update:search-query', value);
};

const handleNoteSelect = (id: string) => {
  emit('select-note', id);
};

const handleNewNote = () => {
  emit('new-note');
};
</script>

<template>
  <aside class="w-80 h-full bg-sidebar border-r text-secondary-foreground border-sidebar-border flex flex-col">
    <!-- Header -->
    <div class="p-4 border-b border-sidebar-border">
      <div class="flex items-center justify-start gap-30 md:gap-0  md:justify-between mb-4">
        <h1 class="text-xl font-serif font-semibold text-sidebar-foreground">
          Notes
        </h1>
        <Button @click="handleNewNote" size="sm" class="gap-1.5 bg-orange-500 cursor-pointer hover:bg-orange-500/90 ">
          <Plus class="w-4 h-4" />
          New
        </Button>
      </div>

      <!-- Search -->
      <div class="relative">
        <Search class="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-muted-foreground" />
        <Input
           :model-value="props.searchQuery"
           @update:modelValue="handleSearch"
          placeholder="Search notes..."
         class="pl-9 bg-sidebar-accent border-sidebar-border"
        />
      </div>
    </div>

    <!-- Notes List -->
    <div class="flex-1 overflow-y-auto p-2">
      <div v-if="props.notes.length === 0" class="flex flex-col items-center justify-center h-40 text-muted-foreground">
        <FileText class="w-10 h-10 mb-2 opacity-50" />
        <p class="text-sm">No notes yet</p>
        <p class="text-xs">Create your first note</p>
      </div>
      <div v-else class="space-y-1">
        <button
          v-for="note in props.notes"
          :key="note.id"
          @click="handleNoteSelect(note.id)"
          :class="cn(
            'w-full text-left cursor-pointer p-3 rounded-lg transition-all duration-200',
            'hover:bg-note-hover',
            props.activeNoteId === note.id && 'bg-note-active'
          )"
        >
          <h3 class="font-medium text-sidebar-foreground truncate text-sm">
            {{ note.title || 'Untitled Note' }}
          </h3>
          <p class="text-xs text-muted-foreground mt-1 line-clamp-2">
            {{ note.content || 'No content' }}
          </p>
          <p class="text-xs text-muted-foreground/70 mt-2">
            {{ format(note.updatedAt, 'MMM d, yyyy') }}
          </p>
        </button>
      </div>
    </div>

    <!-- Footer -->
    <div class="p-4 border-t border-sidebar-border">
      <p class="text-xs text-muted-foreground text-center">
        {{ props.notes.length }} {{ props.notes.length === 1 ? 'note' : 'notes' }}
      </p>
    </div>
  </aside>
</template>
