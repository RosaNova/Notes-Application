import { createRouter, createWebHistory } from "vue-router";
import Index from "@/pages/Index.vue";
import Auth from "@/pages/Auth.vue";
import NotFound from "@/pages/NotFound.vue";

const routes = [
  {
    path: "/",
    name: "Index",
    component: Index,
    meta: { requiresAuth: true }, 
  },
  {
    path: "/auth",
    name: "Auth",
    component: Auth,
  },
  {
    path: "/:pathMatch(.*)*",
    name: "NotFound",
    component: NotFound,
  },
];

const router = createRouter({
  history: createWebHistory(),
  routes,
});

// JWT Auth Guard
router.beforeEach((to) => {
  const token = localStorage.getItem("user"); // JWT from backend

  // Not logged in → redirect to Auth
  if (to.meta.requiresAuth && !token) {
    return { name: "Auth" };
  }

  // Logged in → prevent access to Auth page
  if (to.name === "Auth" && token) {
    return { name: "Index" };
  }
});

export default router;
