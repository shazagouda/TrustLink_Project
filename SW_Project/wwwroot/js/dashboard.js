/* ============================================================
   TrustLink — Dashboard JS
   All authenticated-page interactivity
   ============================================================ */

// ── SIDEBAR TOGGLE ──
function toggleSidebar() {
  const sidebar = document.getElementById('appSidebar');
  const overlay = document.getElementById('sidebarOverlay');
  if (!sidebar) return;
  sidebar.classList.toggle('open');
  if (overlay) overlay.classList.toggle('show');
  document.body.style.overflow = sidebar.classList.contains('open') ? 'hidden' : '';
}

function closeSidebar() {
  const sidebar = document.getElementById('appSidebar');
  const overlay = document.getElementById('sidebarOverlay');
  if (!sidebar) return;
  sidebar.classList.remove('open');
  if (overlay) overlay.classList.remove('show');
  document.body.style.overflow = '';
}

document.addEventListener('DOMContentLoaded', function () {
  // Menu toggle button
  const menuToggle = document.getElementById('menuToggle');
  if (menuToggle) menuToggle.addEventListener('click', toggleSidebar);

  const overlay = document.getElementById('sidebarOverlay');
  if (overlay) overlay.addEventListener('click', closeSidebar);

  // Close sidebar on nav item click (mobile)
  document.querySelectorAll('.sidebar-nav-item').forEach(item => {
    item.addEventListener('click', function () {
      if (window.innerWidth < 992) closeSidebar();
    });
  });

  // ── ACTIVE NAV ITEM ──
  const currentPage = window.location.pathname.split('/').pop() || 'dashboard.html';
  document.querySelectorAll('.sidebar-nav-item').forEach(item => {
    const href = item.getAttribute('href');
    item.classList.toggle('active', href === currentPage);
  });

  // ── IMAGE UPLOAD PREVIEW ──
  const uploadAreas = document.querySelectorAll('.image-upload-area');
  uploadAreas.forEach(area => {
    const input = area.querySelector('input[type="file"]');
    const previewGrid = area.parentElement.querySelector('.image-preview-grid');
    if (!input) return;

    input.addEventListener('change', function () {
      if (!previewGrid) return;
      previewGrid.innerHTML = '';
      [...this.files].forEach(file => {
        if (!file.type.startsWith('image/')) return;
        const reader = new FileReader();
        reader.onload = e => {
          const item = document.createElement('div');
          item.className = 'image-preview-item';
          item.innerHTML = `
            <img src="${e.target.result}" alt="Preview">
            <button class="image-preview-remove" type="button" title="Remove image">
              <i class="bi bi-x"></i>
            </button>
          `;
          item.querySelector('.image-preview-remove').addEventListener('click', () => item.remove());
          previewGrid.appendChild(item);
        };
        reader.readAsDataURL(file);
      });
    });

    // Drag & drop
    area.addEventListener('dragover', e => { e.preventDefault(); area.classList.add('dragover'); });
    area.addEventListener('dragleave', () => area.classList.remove('dragover'));
    area.addEventListener('drop', e => {
      e.preventDefault();
      area.classList.remove('dragover');
      if (input) {
        const dt = new DataTransfer();
        [...e.dataTransfer.files].forEach(f => dt.items.add(f));
        input.files = dt.files;
        input.dispatchEvent(new Event('change'));
      }
    });
  });

  // ── SIGNATURE PAD ──
  const canvas = document.getElementById('signature-pad');
  if (canvas) initSignaturePad(canvas);

  // ── FORM VALIDATION (dashboard forms) ──
  const dashForms = document.querySelectorAll('.dash-form');
  dashForms.forEach(form => {
    const inputs = form.querySelectorAll('[required]');
    inputs.forEach(input => {
      input.addEventListener('blur', () => validateDashInput(input));
      input.addEventListener('input', () => {
        if (input.classList.contains('error')) validateDashInput(input);
      });
    });

    form.addEventListener('submit', function (e) {
      e.preventDefault();
      let allValid = true;
      inputs.forEach(input => { if (!validateDashInput(input)) allValid = false; });
      if (allValid) {
        const btn = form.querySelector('[type="submit"]');
        if (btn) {
          const orig = btn.innerHTML;
          btn.disabled = true;
          btn.innerHTML = '<i class="bi bi-hourglass-split me-2"></i> Saving...';
          // Backend handles actual submission — this simulates UI state
          setTimeout(() => {
            btn.disabled = false;
            btn.innerHTML = orig;
            showToast('Saved successfully.');
          }, 1600);
        }
      }
    });
  });

  // ── SIGN CONTRACT AGREEMENT ──
  const agreeCheck = document.getElementById('sign-agree-check');
  const signBtn = document.getElementById('sign-submit-btn');
  if (agreeCheck && signBtn) {
    agreeCheck.addEventListener('change', function () {
      signBtn.disabled = !this.checked;
    });
  }

  // ── PROFILE AVATAR UPLOAD TRIGGER ──
  const avatarEditBtn = document.querySelector('.profile-avatar-edit-btn');
  const avatarInput = document.getElementById('avatar-input');
  if (avatarEditBtn && avatarInput) {
    avatarEditBtn.addEventListener('click', () => avatarInput.click());
    avatarInput.addEventListener('change', function () {
      const file = this.files[0];
      if (!file) return;
      const reader = new FileReader();
      reader.onload = e => {
        const avatarEl = document.querySelector('.profile-avatar-lg');
        if (!avatarEl) return;
        let img = avatarEl.querySelector('img');
        if (!img) {
          img = document.createElement('img');
          avatarEl.appendChild(img);
        }
        img.src = e.target.result;
        img.alt = 'Profile avatar';
        const initials = avatarEl.querySelector('.profile-avatar-initials');
        if (initials) initials.style.display = 'none';
      };
      reader.readAsDataURL(file);
    });
  }

  // ── CHARACTER COUNTER (textarea) ──
  document.querySelectorAll('[data-maxlength]').forEach(el => {
    const max = parseInt(el.dataset.maxlength);
    const counter = document.createElement('div');
    counter.style.cssText = 'font-size:0.72rem;color:var(--text-muted);text-align:right;margin-top:0.3rem;';
    counter.textContent = `0 / ${max}`;
    el.parentElement.appendChild(counter);
    el.addEventListener('input', function () {
      const len = this.value.length;
      counter.textContent = `${len} / ${max}`;
      counter.style.color = len > max * 0.9 ? '#dc2626' : 'var(--text-muted)';
      if (len > max) this.value = this.value.slice(0, max);
    });
  });

  // ── LISTING STATUS TOGGLE (My Listings) ──
  document.addEventListener('click', function (e) {
    const toggle = e.target.closest('[data-toggle-status]');
    if (!toggle) return;
    const listingId = toggle.dataset.listingId;
    const newStatus = toggle.dataset.toggleStatus;
    // POST to /Listings/SetStatus — backend handles this
    showToast(`Listing status updated to ${newStatus}.`);
  });

  // ── DELETE CONFIRM MODAL ──
  document.addEventListener('click', function (e) {
    const deleteBtn = e.target.closest('[data-confirm-delete]');
    if (!deleteBtn) return;
    const msg = deleteBtn.dataset.confirmDelete || 'Are you sure you want to delete this item? This action cannot be undone.';
    if (confirm(msg)) {
      // Form submit or AJAX to backend
      const form = deleteBtn.closest('form');
      if (form) form.submit();
      else showToast('Item deleted.', 'bi-trash');
    }
  });

  // ── LISTING DETAIL: GALLERY THUMBNAILS ──
  document.querySelectorAll('.listing-thumb').forEach(thumb => {
    thumb.addEventListener('click', function () {
      document.querySelectorAll('.listing-thumb').forEach(t => t.classList.remove('active'));
      this.classList.add('active');
      // Backend: swap main image src
    });
  });

  // ── BOOKING DATE PICKERS ──
  const dateFields = document.querySelectorAll('.booking-date-field');
  dateFields.forEach(field => {
    field.addEventListener('click', function () {
      // In production: open date picker
      // Placeholder: visual feedback only
      this.style.background = 'var(--ivory-dark)';
      setTimeout(() => this.style.background = '', 200);
    });
  });

  // ── TOTAL PRICE CALCULATOR (listing details booking) ──
  function recalcTotal() {
    const priceEl = document.getElementById('booking-price-per-day');
    const startEl = document.getElementById('booking-start-date');
    const endEl = document.getElementById('booking-end-date');
    const totalEl = document.getElementById('booking-total-price');
    if (!priceEl || !startEl || !endEl || !totalEl) return;
    const price = parseFloat(priceEl.value || 0);
    const start = new Date(startEl.value);
    const end = new Date(endEl.value);
    if (isNaN(start) || isNaN(end) || end <= start) return;
    const days = Math.ceil((end - start) / (1000 * 60 * 60 * 24));
    totalEl.textContent = `$${(days * price).toFixed(2)}`;
  }

  ['booking-start-date', 'booking-end-date'].forEach(id => {
    const el = document.getElementById(id);
    if (el) el.addEventListener('change', recalcTotal);
  });

  // ── CONTRACT STATUS FILTER (My Contracts) ──
  document.querySelectorAll('[data-filter-status]').forEach(btn => {
    btn.addEventListener('click', function () {
      document.querySelectorAll('[data-filter-status]').forEach(b => {
        b.style.background = 'var(--ivory)';
        b.style.borderColor = 'var(--border)';
        b.style.color = 'var(--text-body)';
      });
      this.style.background = 'var(--navy)';
      this.style.borderColor = 'var(--navy)';
      this.style.color = 'var(--white)';
      const status = this.dataset.filterStatus;
      // Backend: filter /MyContracts?status=@status
      // Frontend preview: filter visible rows
      document.querySelectorAll('[data-contract-status]').forEach(row => {
        const col = row.closest('.contract-list-item, tr');
        if (!col) return;
        col.style.display = (status === 'all' || row.dataset.contractStatus === status) ? '' : 'none';
      });
    });
  });

});

// ── VALIDATE DASHBOARD INPUT ──
function validateDashInput(input) {
  const wrapper = input.closest('.dash-form-group') || input.parentElement;
  const err = wrapper.querySelector('.dash-form-error');
  let valid = true;
  let msg = '';

  if (input.required && !input.value.trim()) {
    valid = false; msg = 'This field is required.';
  } else if (input.type === 'email' && input.value && !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(input.value)) {
    valid = false; msg = 'Enter a valid email address.';
  } else if (input.min && parseFloat(input.value) < parseFloat(input.min)) {
    valid = false; msg = `Minimum value is ${input.min}.`;
  }

  input.classList.toggle('error', !valid);
  input.classList.toggle('success', valid && !!input.value.trim());
  if (err) {
    err.textContent = msg;
    err.style.display = valid ? 'none' : 'block';
  }
  return valid;
}

// ── SIGNATURE PAD ──
function initSignaturePad(canvas) {
  const ctx = canvas.getContext('2d');
  let drawing = false;
  let lastX = 0, lastY = 0;

  function getPos(e) {
    const rect = canvas.getBoundingClientRect();
    const src = e.touches ? e.touches[0] : e;
    return [src.clientX - rect.left, src.clientY - rect.top];
  }

  function startDraw(e) {
    e.preventDefault();
    drawing = true;
    [lastX, lastY] = getPos(e);
    ctx.beginPath();
    ctx.moveTo(lastX, lastY);
  }

  function draw(e) {
    e.preventDefault();
    if (!drawing) return;
    const [x, y] = getPos(e);
    ctx.lineWidth = 2.5;
    ctx.lineCap = 'round';
    ctx.lineJoin = 'round';
    ctx.strokeStyle = '#0f1f3d';
    ctx.lineTo(x, y);
    ctx.stroke();
    ctx.beginPath();
    ctx.moveTo(x, y);
    [lastX, lastY] = [x, y];

    // Mark signature as having content
    const signedInput = document.getElementById('signature-data');
    if (signedInput) signedInput.value = canvas.toDataURL('image/png');
    const clearBtn = document.querySelector('.sig-clear-btn');
    if (clearBtn) clearBtn.style.opacity = '1';
  }

  function stopDraw() { drawing = false; ctx.beginPath(); }

  // Resize canvas to match CSS
  function resizeCanvas() {
    const data = canvas.toDataURL();
    canvas.width = canvas.offsetWidth;
    canvas.height = canvas.offsetHeight || 160;
    const img = new Image();
    img.onload = () => ctx.drawImage(img, 0, 0);
    img.src = data;
  }

  resizeCanvas();
  window.addEventListener('resize', resizeCanvas);

  canvas.addEventListener('mousedown', startDraw);
  canvas.addEventListener('mousemove', draw);
  canvas.addEventListener('mouseup', stopDraw);
  canvas.addEventListener('mouseleave', stopDraw);
  canvas.addEventListener('touchstart', startDraw, { passive: false });
  canvas.addEventListener('touchmove', draw, { passive: false });
  canvas.addEventListener('touchend', stopDraw);

  // Clear button
  const clearBtn = document.querySelector('.sig-clear-btn');
  if (clearBtn) {
    clearBtn.style.opacity = '0.4';
    clearBtn.addEventListener('click', () => {
      ctx.clearRect(0, 0, canvas.width, canvas.height);
      const signedInput = document.getElementById('signature-data');
      if (signedInput) signedInput.value = '';
      clearBtn.style.opacity = '0.4';
    });
  }
}

// Shared toast (reuse from script.js if loaded, else define)
if (typeof showToast === 'undefined') {
  function showToast(msg, icon = 'bi-check-circle-fill') {
    const existing = document.querySelector('.tl-toast');
    if (existing) existing.remove();
    const t = document.createElement('div');
    t.className = 'tl-toast';
    t.innerHTML = `<i class="bi ${icon}"></i><span>${msg}</span>`;
    document.body.appendChild(t);
    requestAnimationFrame(() => requestAnimationFrame(() => t.classList.add('show')));
    setTimeout(() => { t.classList.remove('show'); setTimeout(() => t.remove(), 400); }, 3500);
  }
}