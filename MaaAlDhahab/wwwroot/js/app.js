/**
 * ماء الذهب - Main Application JavaScript
 * Handles: Nav, Search, Language, Particles, Toast, Utils
 */

// ═══════════════════════════════════════
// NAV & UI INTERACTIONS
// ═══════════════════════════════════════

function toggleMobileMenu() {
    const menu = document.getElementById('mobile-menu');
    const icon = document.getElementById('menu-icon');
    if (menu.classList.contains('translate-x-full')) {
        menu.classList.remove('translate-x-full');
        menu.classList.add('translate-x-0');
        icon.classList.replace('fa-bars', 'fa-times');
        document.body.style.overflow = 'hidden';
    } else {
        menu.classList.add('translate-x-full');
        menu.classList.remove('translate-x-0');
        icon.classList.replace('fa-times', 'fa-bars');
        document.body.style.overflow = '';
    }
}

function toggleSearch() {
    const bar = document.getElementById('search-bar');
    bar.classList.toggle('hidden');
    if (!bar.classList.contains('hidden')) {
        setTimeout(() => document.getElementById('search-input')?.focus(), 100);
    }
}

// Sticky Nav Scroll Effect
window.addEventListener('scroll', () => {
    const nav = document.getElementById('main-nav');
    if (nav) {
        if (window.scrollY > 50) {
            nav.style.backgroundColor = 'rgba(10,10,10,0.97)';
            nav.style.backdropFilter = 'blur(20px)';
        } else {
            nav.style.backgroundColor = '';
            nav.style.backdropFilter = '';
        }
    }
});

// ═══════════════════════════════════════
// LANGUAGE TOGGLE (AR/EN)
// ═══════════════════════════════════════

let currentLang = localStorage.getItem('mad_lang') || 'ar';

function applyLanguage(lang) {
    const html = document.getElementById('html-root');
    const body = document.getElementById('app-body');
    const langBtn = document.getElementById('lang-btn');

    if (lang === 'en') {
        html.setAttribute('lang', 'en');
        html.setAttribute('dir', 'ltr');
        body?.classList.add('ltr');
        if (langBtn) langBtn.textContent = 'AR';
        document.querySelectorAll('.ar-text').forEach(el => el.classList.add('hidden'));
        document.querySelectorAll('.en-text').forEach(el => el.classList.remove('hidden'));
    } else {
        html.setAttribute('lang', 'ar');
        html.setAttribute('dir', 'rtl');
        body?.classList.remove('ltr');
        if (langBtn) langBtn.textContent = 'EN';
        document.querySelectorAll('.ar-text').forEach(el => el.classList.remove('hidden'));
        document.querySelectorAll('.en-text').forEach(el => el.classList.add('hidden'));
    }
    currentLang = lang;
    localStorage.setItem('mad_lang', lang);
}

function toggleLanguage() {
    applyLanguage(currentLang === 'ar' ? 'en' : 'ar');
}

// Apply stored language on load
document.addEventListener('DOMContentLoaded', () => {
    if (currentLang === 'en') applyLanguage('en');
});

// ═══════════════════════════════════════
// LIVE SEARCH
// ═══════════════════════════════════════

let searchTimeout;

async function handleSearch(query) {
    const resultsEl = document.getElementById('search-results');
    if (!resultsEl) return;

    if (!query || query.length < 2) {
        resultsEl.classList.add('hidden');
        return;
    }

    clearTimeout(searchTimeout);
    searchTimeout = setTimeout(async () => {
        try {
            const res = await fetch(`/Product/Search?q=${encodeURIComponent(query)}`);
            const products = await res.json();

            if (products.length === 0) {
                resultsEl.innerHTML = '<p class="p-4 text-gray-500 text-sm text-center">لا توجد نتائج</p>';
            } else {
                resultsEl.innerHTML = products.map(p => `
                    <a href="/Product/Detail/${p.id}" class="flex items-center gap-3 p-3 hover:bg-luxury-hover border-b border-luxury-border last:border-none transition-colors">
                        <div class="w-12 h-12 rounded-lg overflow-hidden flex-shrink-0 bg-luxury-section">
                            <img src="${p.imageUrl}" class="w-full h-full object-cover" onerror="this.src='/images/product-placeholder.svg'" />
                        </div>
                        <div class="flex-1">
                            <p class="text-white text-sm font-bold">${currentLang === 'ar' ? p.nameAr : p.nameEn}</p>
                            <p class="text-gold text-xs font-bold">${p.priceKWD.toFixed(3)} KD</p>
                        </div>
                    </a>
                `).join('');
            }
            resultsEl.classList.remove('hidden');
        } catch (e) {
            console.error('Search failed:', e);
        }
    }, 300);
}

// Close search results on outside click
document.addEventListener('click', (e) => {
    if (!e.target.closest('#search-bar')) {
        document.getElementById('search-results')?.classList.add('hidden');
    }
});

// ═══════════════════════════════════════
// TOAST NOTIFICATIONS
// ═══════════════════════════════════════

function showToast(message, type = 'success') {
    const toast = document.getElementById('toast');
    const msg = document.getElementById('toast-msg');
    const icon = document.getElementById('toast-icon');

    if (!toast || !msg) return;

    msg.textContent = message;
    if (icon) {
        icon.className = type === 'success'
            ? 'fas fa-check-circle text-gold text-xl'
            : 'fas fa-exclamation-circle text-red-400 text-xl';
    }

    toast.classList.remove('hidden');
    toast.style.animation = 'slideInUp 0.3s ease';

    clearTimeout(window._toastTimeout);
    window._toastTimeout = setTimeout(() => {
        toast.classList.add('hidden');
    }, 3500);
}

// ═══════════════════════════════════════
// PARTICLES ANIMATION (Hero Section)
// ═══════════════════════════════════════

function initParticles() {
    const canvas = document.getElementById('particles-canvas');
    if (!canvas) return;

    const ctx = canvas.getContext('2d');
    let particles = [];
    let animFrame;

    function resize() {
        canvas.width = canvas.offsetWidth;
        canvas.height = canvas.offsetHeight;
    }
    resize();
    window.addEventListener('resize', resize);

    class Particle {
        constructor() {
            this.reset();
        }
        reset() {
            this.x = Math.random() * canvas.width;
            this.y = Math.random() * canvas.height;
            this.size = Math.random() * 3 + 1;
            this.speedX = (Math.random() - 0.5) * 0.5;
            this.speedY = -Math.random() * 0.8 - 0.2;
            this.opacity = Math.random() * 0.6 + 0.2;
            this.life = 0;
            this.maxLife = Math.random() * 200 + 100;
        }
        update() {
            this.x += this.speedX;
            this.y += this.speedY;
            this.life++;
            if (this.life > this.maxLife || this.y < -10) this.reset();
        }
        draw() {
            ctx.save();
            ctx.globalAlpha = this.opacity * (1 - this.life / this.maxLife);
            const grad = ctx.createRadialGradient(this.x, this.y, 0, this.x, this.y, this.size * 2);
            grad.addColorStop(0, 'rgba(212, 175, 55, 0.9)');
            grad.addColorStop(1, 'rgba(212, 175, 55, 0)');
            ctx.fillStyle = grad;
            ctx.beginPath();
            ctx.arc(this.x, this.y, this.size * 2, 0, Math.PI * 2);
            ctx.fill();
            ctx.restore();
        }
    }

    // Create particles
    for (let i = 0; i < 60; i++) {
        const p = new Particle();
        p.life = Math.random() * p.maxLife; // Stagger initial positions
        particles.push(p);
    }

    function animate() {
        ctx.clearRect(0, 0, canvas.width, canvas.height);
        particles.forEach(p => { p.update(); p.draw(); });
        animFrame = requestAnimationFrame(animate);
    }
    animate();

    // Cleanup on page unload
    window.addEventListener('beforeunload', () => cancelAnimationFrame(animFrame));
}

// ═══════════════════════════════════════
// NEWSLETTER SUBSCRIPTION
// ═══════════════════════════════════════

async function subscribeNewsletter() {
    const emailInput = document.getElementById('footer-email');
    const msgEl = document.getElementById('subscribe-msg');
    if (!emailInput || !emailInput.value.trim()) return;

    try {
        const res = await fetch('/Home/Subscribe', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ email: emailInput.value.trim() })
        });
        const data = await res.json();
        if (msgEl) {
            msgEl.textContent = data.message;
            msgEl.className = data.success ? 'text-green-400 text-sm mt-2' : 'text-red-400 text-sm mt-2';
            msgEl.classList.remove('hidden');
        }
        if (data.success) {
            emailInput.value = '';
            showToast(data.message);
        }
    } catch (e) {
        console.error('Subscribe failed:', e);
    }
}

// ═══════════════════════════════════════
// WISHLIST TOGGLE
// ═══════════════════════════════════════

async function toggleWishlist(productId, btnEl) {
    try {
        const res = await fetch('/Cart/ToggleWishlist', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ productId })
        });
        const data = await res.json();
        if (data.success) {
            const icon = btnEl.querySelector('i');
            if (icon) {
                if (data.added) {
                    icon.classList.replace('far', 'fas');
                    btnEl.style.color = '#D4AF37';
                    showToast('تمت الإضافة للمفضلة ❤️');
                } else {
                    icon.classList.replace('fas', 'far');
                    btnEl.style.color = '';
                    showToast('تمت الإزالة من المفضلة');
                }
            }
        }
    } catch (e) {
        console.error('Wishlist toggle failed:', e);
    }
}

// ═══════════════════════════════════════
// NAV CART COUNT UPDATE
// ═══════════════════════════════════════

function updateNavCartCount(count) {
    const badge = document.getElementById('cart-count');
    if (badge) {
        if (count > 0) {
            badge.textContent = count > 99 ? '99+' : count;
            badge.classList.remove('hidden');
        } else {
            badge.classList.add('hidden');
        }
    }
}

// Load cart count on page load
async function loadCartCount() {
    try {
        const res = await fetch('/Cart/GetCartCount');
        const data = await res.json();
        updateNavCartCount(data.count);
    } catch (e) {
        // Silently fail
    }
}

document.addEventListener('DOMContentLoaded', loadCartCount);

// ═══════════════════════════════════════
// SMOOTH SCROLL
// ═══════════════════════════════════════

document.querySelectorAll('a[href^="#"]').forEach(anchor => {
    anchor.addEventListener('click', function(e) {
        const target = document.querySelector(this.getAttribute('href'));
        if (target) {
            e.preventDefault();
            target.scrollIntoView({ behavior: 'smooth', block: 'start' });
        }
    });
});

// ═══════════════════════════════════════
// INTERSECTION OBSERVER for animations
// ═══════════════════════════════════════

const observer = new IntersectionObserver((entries) => {
    entries.forEach(entry => {
        if (entry.isIntersecting) {
            entry.target.style.opacity = '1';
            entry.target.style.transform = 'translateY(0)';
        }
    });
}, { threshold: 0.1, rootMargin: '0px 0px -50px 0px' });

document.addEventListener('DOMContentLoaded', () => {
    document.querySelectorAll('.luxury-card, .product-card').forEach(el => {
        el.style.opacity = '0';
        el.style.transform = 'translateY(20px)';
        el.style.transition = 'opacity 0.5s ease, transform 0.5s ease';
        observer.observe(el);
    });
});
