<script setup lang="ts">
import { ref, watch, computed } from 'vue'
import { useRouter } from 'vue-router'
import { z } from 'zod'

import { useAuth } from '@/hooks/useAuth'
import { useToast } from '@/hooks/use-toast'

import Button from '@/components/ui/button/Button.vue'
import Input from '@/components/ui/input/Input.vue'
import Label from '@/components/ui/label/Label.vue'
import {
  Card,
  CardHeader,
  CardTitle,
  CardDescription,
  CardContent,
} from '@/components/ui/card'
import {
  Tabs,
  TabsList,
  TabsTrigger,
  TabsContent,
} from '@/components/ui/tabs'

import { FileText, Loader2 } from 'lucide-vue-next'

/* ------------------------------------------------------------------
 * Validation
 * ------------------------------------------------------------------ */
const authSchema = z.object({
  email: z.string().trim().email('Please enter a valid email address'),
  password: z.string().min(6, 'Password must be at least 6 characters'),
})

const registerSchema = z.object({
  fullname: z.string().trim().min(1, 'Full name is required'),
  email: z.string().trim().email('Please enter a valid email address'),
  password: z.string().min(6, 'Password must be at least 6 characters'),
  confirmPassword: z.string().min(6, 'Password confirmation is required'),
}).refine(data => data.password === data.confirmPassword, {
  message: 'Passwords do not match',
  path: ['confirmPassword'],
})



/* ------------------------------------------------------------------
 * State
 * ------------------------------------------------------------------ */
const email = ref('')
const password = ref('')
const fullname = ref('')
const confirmPassword = ref('')
const isLoading = ref(false)
const errors = ref<{ email?: string; password?: string; fullname?: string; confirmPassword?: string }>({})

const activeTab = ref<'login' | 'register'>('login')

/* ------------------------------------------------------------------
 * Composables
 * ------------------------------------------------------------------ */
const { user, loading, register, login, resetPassword } = useAuth()
const router = useRouter()
const { toast } = useToast()

/* ------------------------------------------------------------------
 * Redirect if already authenticated
 * ------------------------------------------------------------------ */
watch(
  () => [user.value, loading.value],
  ([u, l]) => {
    if (!l && u) {
      router.push('/')
    }
  },
  { immediate: true }
)

/* ------------------------------------------------------------------
 * Helpers
 * ------------------------------------------------------------------ */
function validateForm(): boolean {
  const result = authSchema.safeParse({
    email: email.value,
    password: password.value,
  })

  if (!result.success) {
    const fieldErrors: typeof errors.value = {}
    result.error.issues.forEach(err => {
      if (err.path[0] === 'email') fieldErrors.email = err.message
      if (err.path[0] === 'password') fieldErrors.password = err.message
    })
    errors.value = fieldErrors
    return false
  }

  errors.value = {}
  return true
}

/* ------------------------------------------------------------------
 * Actions
 * ------------------------------------------------------------------ */
async function handleSignIn() {
  if (!validateForm()) return

  isLoading.value = true
  const res = await login(email.value, password.value)
  isLoading.value = false

  if (res) {
    toast({
      title: 'Login failed',
      description:
        res.message === 'Invalid login credentials'
          ? 'Invalid email or password. Please try again.'
          : res.message,
      variant: 'destructive',
    })
  } else {
    toast({
      title: 'Welcome back!',
      description: 'You have successfully logged in.',
    })
    router.push('/')
  }
}

async function handleForgotPassword() {
  if (!email.value.trim()) {
    toast({
      title: 'Email required',
      description: 'Please enter your email address to reset your password.',
      variant: 'destructive',
    })
    return
  }

  const res = await resetPassword(email.value)

  if (res) {
    toast({
      title: 'Reset failed',
      description: res.message,
      variant: 'destructive',
    })
  } else {
    toast({
      title: 'Reset email sent',
      description: 'Check your email for password reset instructions.',
    })
  }
}

async function handleSignUp() {
  const result = registerSchema.safeParse({
    fullname: fullname.value,
    email: email.value,
    password: password.value,
    confirmPassword: confirmPassword.value,
  })

  if (!result.success) {
    const fieldErrors: typeof errors.value = {}
    result.error.issues.forEach(err => {
      const path = err.path[0] as string
      fieldErrors[path as keyof typeof fieldErrors] = err.message
    })
    errors.value = fieldErrors
    return
  }

  isLoading.value = true
  const  res = await register(email.value, password.value)
  isLoading.value = false

  if (res) {
    let message = res.message
    if (res.message.includes('User already registered')) {
      message = 'This email is already registered. Please sign in instead.'
    }

    toast({
      title: 'Sign up failed',
      description: message,
      variant: 'destructive',
    })
  } else {
    toast({
      title: 'Account created!',
      description: 'You can now access your notes.',
    })
  }
}
</script>

<template>
  <div
    v-if="loading"
    class="min-h-screen flex items-center justify-center bg-background"
  >
    <Loader2 class="h-8 w-8 animate-spin text-primary" />
  </div>

  <div
    v-else
    class="min-h-screen flex items-center justify-center bg-background p-4"
  >
    <Card class="w-full max-w-md border-border/50 shadow-lg">
      <CardHeader class="text-center space-y-4">
        <div
          class="mx-auto w-12 h-12 bg-yellow-600/10 rounded-xl flex items-center justify-center"
        >
          <FileText class="h-6 w-6 text-yellow-600" />
        </div>

        <div>
          <CardTitle class="text-2xl font-serif">
            Notes 2026
          </CardTitle>
          <CardDescription class="mt-2">
            Your thoughts, beautifully organized
          </CardDescription>
        </div>
      </CardHeader>

      <CardContent>
        <Tabs v-model="activeTab" class="w-full">
          <TabsList class="grid w-full grid-cols-2 mb-6">
            <TabsTrigger value="login">Sign In</TabsTrigger>
            <TabsTrigger value="register">Sign Up</TabsTrigger>
          </TabsList>

          <!-- Login -->
          <TabsContent value="login">
            <form class="space-y-4" @submit.prevent="handleSignIn">
              <div class="space-y-2">
                <Label>Email</Label>
                <Input
                  type="email"
                  placeholder="you@example.com"
                  v-model="email"
                  :disabled="isLoading"
                  autocomplete="email"
                />
                <p v-if="errors.email" class="text-sm text-destructive">
                  {{ errors.email }}
                </p>
              </div>

              <div class="space-y-2">
                <Label>Password</Label>
                <Input
                  type="password"
                  placeholder="••••••••"
                  v-model="password"
                  :disabled="isLoading"
                  autocomplete="current-password"
                />
                <p v-if="errors.password" class="text-sm text-destructive">
                  {{ errors.password }}
                </p>
              </div>

              <div class="text-right">
                <button type="button" @click="handleForgotPassword" class="text-sm text-primary hover:underline">
                  Forgot password?
                </button>
              </div>

              <Button class="w-full bg-yellow-700 hover:bg-yellow-700/70 cursor-pointer" :disabled="isLoading">
                <Loader2
                  v-if="isLoading"
                  class="mr-2 h-4 w-4 animate-spin"
                />
                {{ isLoading ? 'Signing in...' : 'Sign In' }}
              </Button>
            </form>
          </TabsContent>

          <!-- Register -->
          <TabsContent value="register">
            <form class="space-y-4" @submit.prevent="handleSignUp">
                  <div class="space-y-2">
                <Label>Fullname</Label>
                <Input
                  type="text"
                  placeholder="John Doe"
                  v-model="fullname"
                  :disabled="isLoading"
                  autocomplete="name"
                />
                <p v-if="errors.fullname" class="text-sm text-destructive">
                  {{ errors.fullname }}
                </p>
              </div>

                <div class="space-y-2">
                <Label>Email</Label>
                <Input
                  type="email"
                  placeholder="you@example.com"
                  v-model="email"
                  :disabled="isLoading"
                  autocomplete="email"
                />
                <p v-if="errors.email" class="text-sm text-destructive">
                  {{ errors.email }}
                </p>
              </div>

              <div class="space-y-2">
                <Label>Password</Label>
                <Input
                  type="password"
                  placeholder="••••••••"
                  v-model="password"
                  :disabled="isLoading"
                  autocomplete="new-password"
                />
                <p v-if="errors.password" class="text-sm text-destructive">
                  {{ errors.password }}
                </p>
              </div>

                  <div class="space-y-2">
                <Label>Confirm Password</Label>
                <Input
                  type="password"
                  placeholder="••••••••"
                  v-model="confirmPassword"
                  :disabled="isLoading"
                  autocomplete="new-password"
                />
                <p v-if="errors.confirmPassword" class="text-sm text-destructive">
                  {{ errors.confirmPassword }}
                </p>
              </div>

              <Button class="w-full  bg-yellow-700 hover:bg-yellow-700/70 cursor-pointer" :disabled="isLoading">
                <Loader2
                  v-if="isLoading"
                  class="mr-2 h-4 w-4 animate-spin"
                />
                {{ isLoading ? 'Creating account...' : 'Create Account' }}
              </Button>
            </form>
          </TabsContent>
        </Tabs>
      </CardContent>
    </Card>
  </div>
</template>
