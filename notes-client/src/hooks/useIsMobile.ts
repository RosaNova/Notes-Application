// useIsMobile.ts
import { ref, onMounted, onUnmounted } from 'vue';

const MOBILE_BREAKPOINT = 768;

export function useIsMobile() {
  const isMobile = ref<boolean>(window.innerWidth < MOBILE_BREAKPOINT);

  const handleResize = () => {
    isMobile.value = window.innerWidth < MOBILE_BREAKPOINT;
  };

  onMounted(() => {
    window.addEventListener('resize', handleResize);
    handleResize(); // set initial value
  });

  onUnmounted(() => {
    window.removeEventListener('resize', handleResize);
  });

  return isMobile;
}
