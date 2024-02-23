export function loadSlider() {
    if ($('.owl-carousel.mentoring-course').length > 0) {
        var owl = $('.owl-carousel.mentoring-course');
        owl.owlCarousel({
            margin: 25,
            nav: false,
            nav: true,
            loop: true,
            responsive: {
                0: {
                    items: 1
                },
                768: {
                    items: 3
                },
                1170: {
                    items: 4
                }
            }
        });
    }
}