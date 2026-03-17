/**
 * ماء الذهب - Maa Al Dhahab Perfumes
 * Service Worker for PWA Offline Support
 */

const CACHE_NAME = 'maa-al-dhahab-v1.2';
const STATIC_CACHE = 'mad-static-v1.2';
const DYNAMIC_CACHE = 'mad-dynamic-v1.2';

// Core assets to cache immediately on install
const STATIC_ASSETS = [
    '/',
    '/Shop',
    '/Home/About',
    '/Cart',
    '/css/luxury.css',
    '/js/app.js',
    '/js/cart.js',
    '/js/pwa.js',
    '/manifest.json',
    '/images/product-placeholder.svg',
    // Tailwind CDN fallback not possible offline - use custom CSS
    'https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.0/css/all.min.css'
];

// Install: cache static assets
self.addEventListener('install', (event) => {
    console.log('[SW] Installing Maa Al Dhahab Service Worker...');
    event.waitUntil(
        caches.open(STATIC_CACHE)
            .then(cache => {
                console.log('[SW] Caching static assets');
                return cache.addAll(STATIC_ASSETS.filter(url => !url.startsWith('http') || url.includes('cdnjs')));
            })
            .then(() => self.skipWaiting())
            .catch(err => console.log('[SW] Cache failed:', err))
    );
});

// Activate: cleanup old caches
self.addEventListener('activate', (event) => {
    console.log('[SW] Activating...');
    event.waitUntil(
        caches.keys().then(cacheNames => {
            return Promise.all(
                cacheNames
                    .filter(name => name !== STATIC_CACHE && name !== DYNAMIC_CACHE)
                    .map(name => {
                        console.log('[SW] Deleting old cache:', name);
                        return caches.delete(name);
                    })
            );
        }).then(() => self.clients.claim())
    );
});

// Fetch: Network-first for API/dynamic, Cache-first for static
self.addEventListener('fetch', (event) => {
    const { request } = event;
    const url = new URL(request.url);

    // Skip non-GET requests
    if (request.method !== 'GET') return;

    // Skip admin panel (always online)
    if (url.pathname.startsWith('/admin')) return;

    // Skip checkout (must be online)
    if (url.pathname.startsWith('/Checkout')) return;

    // API requests: Network first, no cache
    if (url.pathname.startsWith('/Cart/') || url.pathname.startsWith('/Product/Search')) {
        event.respondWith(
            fetch(request).catch(() => new Response('{"error":"offline"}', {
                headers: { 'Content-Type': 'application/json' }
            }))
        );
        return;
    }

    // Images: Cache first, then network
    if (request.destination === 'image') {
        event.respondWith(
            caches.match(request).then(cached => {
                if (cached) return cached;
                return fetch(request)
                    .then(response => {
                        const clone = response.clone();
                        caches.open(DYNAMIC_CACHE).then(cache => cache.put(request, clone));
                        return response;
                    })
                    .catch(() => caches.match('/images/product-placeholder.svg'));
            })
        );
        return;
    }

    // HTML pages: Network first, fall back to cache
    if (request.destination === 'document') {
        event.respondWith(
            fetch(request)
                .then(response => {
                    const clone = response.clone();
                    caches.open(DYNAMIC_CACHE).then(cache => cache.put(request, clone));
                    return response;
                })
                .catch(() => {
                    return caches.match(request)
                        .then(cached => cached || caches.match('/'))
                        .then(cached => cached || offlinePage());
                })
        );
        return;
    }

    // Static assets: Cache first
    event.respondWith(
        caches.match(request)
            .then(cached => {
                if (cached) return cached;
                return fetch(request).then(response => {
                    const clone = response.clone();
                    caches.open(STATIC_CACHE).then(cache => cache.put(request, clone));
                    return response;
                });
            })
    );
});

function offlinePage() {
    return new Response(`
        <!DOCTYPE html>
        <html lang="ar" dir="rtl">
        <head>
            <meta charset="utf-8">
            <meta name="viewport" content="width=device-width, initial-scale=1">
            <title>ماء الذهب - غير متصل</title>
            <style>
                * { margin: 0; padding: 0; box-sizing: border-box; font-family: Cairo, Arial, sans-serif; }
                body { background: #0A0A0A; color: #E8E0D0; display: flex; align-items: center; justify-content: center; min-height: 100vh; text-align: center; padding: 2rem; }
                .gold { color: #D4AF37; }
                h1 { font-size: 2rem; margin-bottom: 1rem; }
                p { color: #9CA3AF; margin-bottom: 2rem; }
                button { background: linear-gradient(135deg, #D4AF37, #C9A227); color: #0A0A0A; border: none; padding: 1rem 2rem; border-radius: 12px; font-weight: 700; font-size: 1rem; cursor: pointer; }
                button:hover { opacity: 0.9; }
                .icon { font-size: 4rem; margin-bottom: 1rem; }
            </style>
        </head>
        <body>
            <div>
                <div class="icon">⚡</div>
                <h1 class="gold">ماء الذهب</h1>
                <p>أنت غير متصل بالإنترنت حالياً. بعض الصفحات متاحة من الذاكرة المؤقتة.</p>
                <button onclick="location.reload()">إعادة المحاولة</button>
            </div>
        </body>
        </html>
    `, {
        headers: { 'Content-Type': 'text/html; charset=utf-8' }
    });
}

// Background sync for offline cart actions
self.addEventListener('sync', (event) => {
    if (event.tag === 'sync-cart') {
        event.waitUntil(syncCart());
    }
});

async function syncCart() {
    // Sync pending cart actions when back online
    const db = await openDB();
    const pendingActions = await db.getAll('pendingCartActions');
    for (const action of pendingActions) {
        try {
            await fetch(action.url, {
                method: action.method,
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(action.data)
            });
        } catch (e) {
            console.log('[SW] Sync failed for action:', action);
        }
    }
}
