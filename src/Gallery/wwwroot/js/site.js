// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

document.addEventListener('DOMContentLoaded', () => {
  const artworks = Array.from(document.querySelectorAll('.artwork'));
  if (artworks.length === 0) return;

  const clamp = (v, min, max) => Math.max(min, Math.min(max, v));

  // Only scale while less than 70% of the artwork is visible.
  const updateScales = () => {
    const viewportHeight = window.innerHeight || document.documentElement.clientHeight;

    artworks.forEach((el) => {
      const rect = el.getBoundingClientRect();

      // Compute how much of the element is visible
      const visible = Math.max(0, Math.min(rect.bottom, viewportHeight) - Math.max(rect.top, 0));
      const visibilityRatio = clamp(visible / Math.max(1, rect.height), 0, 1);

      if (visibilityRatio >= 0.7) {
        el.style.transform = 'scale(1)';
        return;
      }

      const t = visibilityRatio / 0.7; // 0..1
      const eased = t * t; // ease-in
      const scale = 0.95 + 0.05 * eased;
      el.style.transform = `scale(${scale})`;
    });
  };

  let ticking = false;
  const requestTick = () => {
    if (ticking) return;
    ticking = true;
    requestAnimationFrame(() => {
      updateScales();
      ticking = false;
    });
  };

  updateScales();
  window.addEventListener('scroll', requestTick, { passive: true });
  window.addEventListener('resize', requestTick);
});
