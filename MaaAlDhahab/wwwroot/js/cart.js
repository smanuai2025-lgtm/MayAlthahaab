/**
 * ماء الذهب - Cart JavaScript
 * Handles add to cart, quick add, cart sync
 */

// Quick Add to Cart (from product cards)
async function quickAddToCart(productId, productName, price) {
    await addToCart(productId, productName, '', price, 1);
}

// Full Add to Cart
async function addToCart(productId, productName, size, price, quantity = 1) {
    try {
        const res = await fetch('/Cart/Add', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ productId, size, price, quantity })
        });
        const data = await res.json();

        if (data.success) {
            updateNavCartCount(data.cartCount);
            showToast(data.message || 'تمت الإضافة للسلة! 🛒');

            // Animate cart icon
            const cartIcon = document.querySelector('.fa-shopping-bag');
            if (cartIcon) {
                cartIcon.style.transform = 'scale(1.4)';
                cartIcon.style.color = '#F5E6B2';
                setTimeout(() => {
                    cartIcon.style.transform = '';
                    cartIcon.style.color = '';
                }, 400);
            }
        } else {
            showToast(data.message || 'حدث خطأ', 'error');
        }
    } catch (e) {
        console.error('Add to cart failed:', e);
        showToast('حدث خطأ في إضافة المنتج', 'error');
    }
}

// Apply coupon from cart page
async function applyCouponOnCheckout(code) {
    if (!code) return;
    const res = await fetch('/Checkout/ValidateCoupon', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ code })
    });
    return await res.json();
}

// Store cart in localStorage as backup (for offline sync)
function backupCart() {
    // Cart is managed server-side via session
    // This is just a local UI backup
    const cartItems = [];
    document.querySelectorAll('.cart-item').forEach(item => {
        cartItems.push({
            productId: item.dataset.productId,
            size: item.dataset.size,
            qty: item.querySelector('.item-qty')?.textContent
        });
    });
    localStorage.setItem('mad_cart_backup', JSON.stringify(cartItems));
}
