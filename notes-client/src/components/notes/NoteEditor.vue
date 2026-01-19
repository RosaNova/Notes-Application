<script setup lang="ts">
import type { Note } from '@/types/note';
import { Trash2, Clock } from 'lucide-vue-next';
import Button from '@/components/ui/button/Button.vue';
import { format } from 'date-fns';
import {
  AlertDialog,
  AlertDialogTrigger,
  AlertDialogContent,
  AlertDialogHeader,
  AlertDialogTitle,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogCancel,
  AlertDialogAction,
} from '@/components/ui/alert-dialog';
import { ref, watch } from 'vue';
import { debounce } from 'lodash-es';

// Props
const props = defineProps<{
  note: Note | null;
}>();

// Emits
const emit = defineEmits<{
  (e: 'update-note', id: number, updates: Partial<Pick<Note, 'title' | 'content'>>): void;
  (e: 'delete-note', id: number): void;
}>();

// Local state for title and content
const localTitle = ref('');
const localContent = ref('');
const isSyncing = ref(false);

// -----------------------------
// Sync local state when note changes
// -----------------------------
watch(
  () => props.note,
  (note) => {
    if (!note) return;

    isSyncing.value = true;
    localTitle.value = note.title ?? '';
    localContent.value = note.content ?? '';

    // allow next tick before ending syncing
    setTimeout(() => (isSyncing.value = false), 0);
  },
  { immediate: true }
);

// -----------------------------
// Auto-save debounced
// -----------------------------
const save = debounce(() => {
  if (!props.note || isSyncing.value) return;

  emit('update-note', props.note.id, {
    title: localTitle.value,
    content: localContent.value,
  });
}, 500);

// Watch local title/content for changes
watch([localTitle, localContent], () => save());

// -----------------------------
// Delete note
// -----------------------------
const deleteNote = () => {
  if (props.note) emit('delete-note', props.note.id);
};
</script>

<template>
  <!-- Empty state -->
  <div v-if="!props.note" class="flex-1 flex items-center justify-center bg-background">
    <div class="text-center text-muted-foreground">
      <div class="w-16 h-16 mx-auto mb-4 rounded-full bg-muted flex items-center justify-center">
        <Clock class="w-8 h-8 opacity-50" />
      </div>
      <p class="text-lg font-medium">Select a note</p>
      <p class="text-sm">Choose a note from the sidebar or create a new one</p>
    </div>
  </div>

  <!-- Note editor -->
  <div v-else class="flex-1 flex flex-col bg-background">
    
    <!-- Toolbar -->
    <div class="flex items-center justify-between px-6 py-3 border-b border-border">
      <div class="flex items-center gap-2 text-xs text-muted-foreground">
        <Clock class="w-3.5 h-3.5" />
        <span>
          Updated {{ format(new Date(note.updatedAt), 'MMM d, yyyy · h:mm a') }}
        </span>
      </div>

      <AlertDialog>
        <AlertDialogTrigger asChild>
          <Button
            variant="ghost"
            size="sm"
            class="text-destructive cursor-pointer bg-destructive/20 hover:text-destructive hover:bg-destructive/10"
          >
            <Trash2 class="w-4 h-4" />
          </Button>
        </AlertDialogTrigger>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Delete this note?</AlertDialogTitle>
            <AlertDialogDescription>
              This action cannot be undone. This will permanently delete your note.
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel class="cursor-pointer">Cancel</AlertDialogCancel>
            <AlertDialogAction
              @click="deleteNote"
              class="bg-destructive cursor-pointer text-destructive-foreground hover:bg-destructive/90"
            >
              Delete
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </div>

    <!-- Editor -->
    <div class="flex-1 overflow-y-auto">
      <div class="max-w-3xl mx-auto px-6 py-8">
        <!-- Title -->
        <input
          type="text"
          v-model="localTitle"
          placeholder="Note title..."
          class="w-full text-3xl font-serif font-semibold bg-transparent border-none outline-none placeholder:text-muted-foreground/50 mb-6"
        />

        <!-- Content -->
        <textarea
          v-model="localContent"
          placeholder="Start writing your note..."
          class="w-full min-h-[60vh] text-base leading-relaxed bg-transparent border-none outline-none resize-none placeholder:text-muted-foreground/50"
        />
      </div>
    </div>
  </div>
</template>
