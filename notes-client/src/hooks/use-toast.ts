import { toast as sonnerToast } from 'sonner'

/* ------------------------------------------------------------------
 * Types
 * ------------------------------------------------------------------ */
export type ToastVariant = 'default' | 'destructive'

export interface ToastOptions {
  title?: string
  description?: string
  variant?: ToastVariant
}

/* ------------------------------------------------------------------
 * useToast composable
 * ------------------------------------------------------------------ */
export function useToast() {
  function toast({ title, description, variant = 'default' }: ToastOptions) {
    if (variant === 'destructive') {
      sonnerToast.error(title ?? 'Error', {
        description,
      })
    } else {
      sonnerToast(title ?? '', {
        description,
      })
    }
  }

  return {
    toast,
  }
}
