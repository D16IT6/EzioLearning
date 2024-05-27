export function loadComponents() {
    // Login Slide
    if ($('.owl-carousel.login-slide').length > 0) {
        var owl = $('.owl-carousel.login-slide');
        owl.owlCarousel({
            margin: 24,
            nav: false,
            nav: true,
            loop: true,
            responsive: {
                0: {
                    items: 1
                },
                768: {
                    items: 1
                },
                1170: {
                    items: 1
                }
            }
        });
    }
}