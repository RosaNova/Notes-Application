<script setup lang="ts">
import Sonner from "@/components/ui/sonner/Sonner.vue";
import TooltipProvider from "@/components/ui/tooltip/TooltipProvider.vue";
</script>

<template>
  <TooltipProvider>
    <Sonner rich-colors />
    <RouterView />
  </TooltipProvider>
</template>
