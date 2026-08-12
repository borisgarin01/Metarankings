window.initHeader = (headerElement) => {
    const header = headerElement?.closest?.('.header-bottom') || document.querySelector('.header-bottom');
    if (!header) return;

    // Sticky header
    window.addEventListener('scroll', () => {
        const scrollY = window.scrollY;
        if (scrollY >= 90) {
            header.classList.add('sticky');
        } else {
            header.classList.remove('sticky');
        }
    });

    // Закрытие меню при клике вне
    document.addEventListener('click', (e) => {
        const headerContainer = header.closest('.header');
        const menuIcon = headerContainer?.querySelector('.menu-icon');
        const nav = headerContainer?.querySelector('.nav');

        if (headerContainer?.classList.contains('menu-open') &&
            menuIcon && nav &&
            !menuIcon.contains(e.target) &&
            !nav.contains(e.target)) {
            headerContainer.classList.remove('menu-open');
            document.body.classList.remove('menu-open');
        }
    });
};

window.initToTop = () => {
    const toTop = document.getElementById('toTop');
    if (!toTop) return;

    window.addEventListener('scroll', () => {
        if (window.scrollY > 300) {
            toTop.style.display = 'flex';
        } else {
            toTop.style.display = 'none';
        }
    });
};

window.scrollToTop = () => {
    window.scrollTo({ top: 0, behavior: 'smooth' });
};