import { ref, computed } from "vue";
import api from "@/services/api";

export interface AuthUser {
  id: string;
  email: string;
}

const user = ref<AuthUser | null>(null);
const loading = ref(false);

export function useAuth() {
  // -----------------------------
  // Restore user from token
  // -----------------------------
  const restoreUser = () => {
    const stored = localStorage.getItem("user");
    if (!stored) return;

    try {
      const parsed = JSON.parse(stored);
      const payload = JSON.parse(atob(parsed.token.split(".")[1]));

      user.value = {
        id: payload.nameid,
        email: payload.email,
      };
    } catch {
      logout();
    }
  };

  // -----------------------------
  // Login
  // -----------------------------
  const login = async (email: string, password: string) => {
    loading.value = true;
    console.log({email , password});
    try {
      const res = await api.post("/users/auth/login", { email, password });
      
       
      const token = res.data.token;
      localStorage.setItem("user", JSON.stringify({ token }));

      restoreUser();
      return { success: true };
    } catch (err: any) {
      return {
        success: false,
        message:
          err?.response?.data?.message ?? "Invalid email or password",
      };
    } finally {
      loading.value = false;
    }
  };

  // -----------------------------
  // Register
  // -----------------------------
  const register = async (email: string, password: string) => {
    loading.value = true;
    try {
      await api.post("/auth/register", { email, password });
      return { success: true };
    } catch (err: any) {
      return {
        success: false,
        message:
          err?.response?.data?.message ?? "Registration failed",
      };
    } finally {
      loading.value = false;
    }
  };
 
  
   // -----------------------------
  // Reset password
  // -----------------------------
  const resetPassword = async (email: string) => {
    loading.value = true;
    try {
      await api.post("/auth/reset-password", { email });
      return { success: true };
    } catch (err: any) {
      return {
        success: false,
        message: err?.response?.data?.message ?? "Reset password failed",
      };
    } finally {
      loading.value = false;
    }
  };

  // -----------------------------
  // Logout
  // -----------------------------
  const logout = () => {
    localStorage.removeItem("user");
    user.value = null;
  };

  // -----------------------------
  // Auth state
  // -----------------------------
  const isAuthenticated = computed(() => !!user.value);

  restoreUser();

  return {
    user,
    loading,
    isAuthenticated,
    login,
    resetPassword,
    register,
    logout,
  };
}
