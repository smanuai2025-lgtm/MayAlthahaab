/**
 * ماء الذهب - PWA Service Worker & Install Prompt
 */

let deferredPrompt = null;

// Register Service Worker
if ('serviceWorker' in navigator) {
    window.addEventListener('load', async () => {
        try {
            const registration = await navigator.serviceWorker.register('/sw.js', {
                scope: '/'
            });
            console.log('[PWA] Service Worker registered:', registration.scope);

            // Check for updates
            registration.addEventListener('updatefound', () => {
                const newWorker = registration.installing;
                newWorker?.addEventListener('statechange', () => {
                    if (newWorker.state === 'installed' && navigator.serviceWorker.controller) {
                        console.log('[PWA] New version available!');
                    }
                });
            });
        } catch (err) {
            console.log('[PWA] Service Worker registration failed:', err);
        }
    });
}

// Capture Install Prompt
window.addEventListener('beforeinstallprompt', (e) => {
    e.preventDefault();
    deferredPrompt = e;

    // Show custom install banner after 3 seconds
    setTimeout(() => {
        const banner = document.getElementById('pwa-install-banner');
        if (banner && !localStorage.getItem('mad_pwa_dismissed')) {
            banner.classList.remove('hidden');
            banner.style.animation = 'slideInUp 0.5s ease';
        }
    }, 3000);
});

// Install PWA
async function installPWA() {
    if (!deferredPrompt) return;

    deferredPrompt.prompt();
    const { outcome } = await deferredPrompt.userChoice;

    if (outcome === 'accepted') {
        console.log('[PWA] User accepted install');
        showToast('تم تثبيت التطبيق بنجاح! 🎉');
    }

    deferredPrompt = null;
    dismissPWA();
}

// Dismiss Install Banner
function dismissPWA() {
    const banner = document.getElementById('pwa-install-banner');
    if (banner) banner.classList.add('hidden');
    localStorage.setItem('mad_pwa_dismissed', '1');
}

// App installed event
window.addEventListener('appinstalled', () => {
    console.log('[PWA] App installed!');
    deferredPrompt = null;
    dismissPWA();
    showToast('مرحباً بك في تطبيق ماء الذهب! ✨');
});

// Online/Offline indicators
window.addEventListener('online', () => {
    showToast('✓ تم استعادة الاتصال بالإنترنت');
});

window.addEventListener('offline', () => {
    showToast('أنت غير متصل بالإنترنت - بعض الميزات محدودة', 'error');
});

// Push notification permission (mock)
async function requestPushPermission() {
    if ('Notification' in window) {
        const permission = await Notification.requestPermission();
        if (permission === 'granted') {
            new Notification('ماء الذهب 👑', {
                body: 'شكراً! ستتلقى أحدث العروض والعطور الجديدة',
                icon: '/icons/icon-192x192.png',
                badge: '/icons/icon-96x96.png'
            });
        }
    }
}
