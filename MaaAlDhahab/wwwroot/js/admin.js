/**
 * ماء الذهب - Admin Panel JavaScript
 */

function showAdminToast(message, type = 'success') {
    const existing = document.querySelector('.admin-toast');
    if (existing) existing.remove();

    const t = document.createElement('div');
    t.className = 'admin-toast fixed bottom-4 right-4 z-50 shadow-2xl rounded-xl px-6 py-3 text-sm font-bold flex items-center gap-2';
    t.style.background = '#121212';
    t.style.border = '1px solid ' + (type === 'success' ? '#D4AF37' : '#EF4444');
    t.style.color = type === 'success' ? '#D4AF37' : '#EF4444';
    t.innerHTML = (type === 'success' ? '✓ ' : '✗ ') + message;
    document.body.appendChild(t);
    setTimeout(() => t.remove(), 3000);
}

// Confirm dialogs
function confirmDelete(message = 'هل أنت متأكد من الحذف؟') {
    return window.confirm(message);
}

// Image preview
function previewImage(input, previewId) {
    if (input.value) {
        const preview = document.getElementById(previewId);
        if (preview) {
            preview.src = input.value;
            preview.classList.remove('hidden');
        }
    }
}

// Charts (simple bar chart using canvas)
function drawRevenueChart(data, canvasId) {
    const canvas = document.getElementById(canvasId);
    if (!canvas) return;
    const ctx = canvas.getContext('2d');
    const max = Math.max(...data.values);

    canvas.width = canvas.offsetWidth;
    canvas.height = 200;

    const barWidth = (canvas.width / data.labels.length) - 10;
    const scale = 160 / (max || 1);

    data.labels.forEach((label, i) => {
        const x = i * (barWidth + 10) + 5;
        const barH = data.values[i] * scale;
        const y = 170 - barH;

        // Bar
        const grad = ctx.createLinearGradient(0, y, 0, 170);
        grad.addColorStop(0, 'rgba(212,175,55,0.9)');
        grad.addColorStop(1, 'rgba(212,175,55,0.2)');
        ctx.fillStyle = grad;
        ctx.beginPath();
        ctx.roundRect(x, y, barWidth, barH, 4);
        ctx.fill();

        // Label
        ctx.fillStyle = '#9CA3AF';
        ctx.font = '10px Cairo';
        ctx.textAlign = 'center';
        ctx.fillText(label, x + barWidth / 2, 190);
    });
}
